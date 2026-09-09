#ifndef CITY_DITHER_SHADOW_CASTER_PASS_INCLUDED
#define CITY_DITHER_SHADOW_CASTER_PASS_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
#if defined(LOD_FADE_CROSSFADE)
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/LODCrossFade.hlsl"
#endif

CBUFFER_START(UnityPerMaterialCityDitherShadow)
    float _Fade;
    float _DitherScale;
    float _ShadowFadeScale;
CBUFFER_END

// CityDither: dither CHI dung trong Shadow Pass de lam bong do nhat dan theo _Fade.
// Vi shadow map duoc loc mem (PCF/soft shadow) nen cham dither se tan thanh
// mot lop bong nhat deu, khong bi lo cham nhu tren be mat mau (Forward Pass).
static const float CityBayer4x4Shadow[16] = {
    0.0/16.0,  8.0/16.0,  2.0/16.0, 10.0/16.0,
    12.0/16.0, 4.0/16.0, 14.0/16.0,  6.0/16.0,
    3.0/16.0, 11.0/16.0,  1.0/16.0,  9.0/16.0,
    15.0/16.0, 7.0/16.0, 13.0/16.0,  5.0/16.0
};

float CityDitherValueShadow(float2 screenPos)
{
    float scale = max(_DitherScale, 0.0001);
    uint2 pixel = uint2(screenPos / scale) % 4;
    return CityBayer4x4Shadow[pixel.y * 4 + pixel.x];
}

float3 _LightDirection;
float3 _LightPosition;

struct Attributes
{
    float4 positionOS   : POSITION;
    float3 normalOS     : NORMAL;
    float2 texcoord     : TEXCOORD0;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct Varyings
{
    #if defined(_ALPHATEST_ON)
        float2 uv       : TEXCOORD0;
    #endif
    float4 positionCS   : SV_POSITION;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

float4 GetShadowPositionHClip(Attributes input)
{
    float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);
    float3 normalWS = TransformObjectToWorldNormal(input.normalOS);

#if _CASTING_PUNCTUAL_LIGHT_SHADOW
    float3 lightDirectionWS = normalize(_LightPosition - positionWS);
#else
    float3 lightDirectionWS = _LightDirection;
#endif

    float4 positionCS = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, lightDirectionWS));
    positionCS = ApplyShadowClamping(positionCS);
    return positionCS;
}

Varyings ShadowPassVertex(Attributes input)
{
    Varyings output;
    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_TRANSFER_INSTANCE_ID(input, output);

    #if defined(_ALPHATEST_ON)
    output.uv = TRANSFORM_TEX(input.texcoord, _BaseMap);
    #endif

    output.positionCS = GetShadowPositionHClip(input);
    return output;
}

half4 ShadowPassFragment(Varyings input) : SV_TARGET
{
    UNITY_SETUP_INSTANCE_ID(input);

    // CityDither: dither theo pattern Bayer 4x4, nhung nguong xoa pixel bong
    // = _Fade * _ShadowFadeScale thay vi dung 100% _Fade.
    // => _ShadowFadeScale = 1: bong nhat dung theo ty le trong suot hinh anh.
    //    _ShadowFadeScale < 1: bong dam hon (giu lai nhieu pixel bong hon),
    //    van nhat hon building dac (fade=0 luon giu toan bo) nhung ro net hon truoc.
    float shadowThreshold = saturate(_Fade * _ShadowFadeScale);
    float ditherValue = CityDitherValueShadow(input.positionCS.xy);
    clip(ditherValue - shadowThreshold);

    #if defined(_ALPHATEST_ON)
        Alpha(SampleAlbedoAlpha(input.uv, TEXTURE2D_ARGS(_BaseMap, sampler_BaseMap)).a, _BaseColor, _Cutoff);
    #endif

    #if defined(LOD_FADE_CROSSFADE)
        LODFadeCrossFade(input.positionCS);
    #endif

    return 0;
}

#endif
