
using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[Serializable]
public class CityObjectData
{
    public string name;
    public string type;
    public float[] loc;
    public float yaw_deg;
    public float[] scale;
    public string asset_key;
    public string fbx_path;
}

[Serializable]
public class CityData
{
    public int count;
    public CityObjectData[] objects;
}

public static class CityImporter
{
    public static string BakeAxisConversion(string jsonAbsolutePath)
    {
        string json = File.ReadAllText(jsonAbsolutePath);
        CityData data = JsonUtility.FromJson<CityData>(json);

        HashSet<string> distinctPaths = new HashSet<string>();
        foreach (var o in data.objects)
            if (!string.IsNullOrEmpty(o.fbx_path)) distinctPaths.Add(o.fbx_path);

        int changed = 0, alreadyOk = 0, failed = 0;
        List<string> failedList = new List<string>();

        foreach (var path in distinctPaths)
        {
            ModelImporter mi = AssetImporter.GetAtPath(path) as ModelImporter;
            if (mi == null) { failed++; failedList.Add(path); continue; }
            if (!mi.bakeAxisConversion)
            {
                mi.bakeAxisConversion = true;
                mi.SaveAndReimport();
                changed++;
            }
            else
            {
                alreadyOk++;
            }
        }

        return "distinctFbx=" + distinctPaths.Count + " changed=" + changed + " alreadyOk=" + alreadyOk +
               " failed=" + failed + " sampleFailed=" + string.Join(",", failedList.GetRange(0, Mathf.Min(5, failedList.Count)));
    }

    public static string Import(string jsonAbsolutePath)
    {
        if (!File.Exists(jsonAbsolutePath))
            return "ERROR: file not found: " + jsonAbsolutePath;

        string json = File.ReadAllText(jsonAbsolutePath);
        CityData data = JsonUtility.FromJson<CityData>(json);
        if (data == null || data.objects == null)
            return "ERROR: failed to parse JSON";

        GameObject root = GameObject.Find("ImportedCity_Blender");
        if (root == null)
        {
            root = new GameObject("ImportedCity_Blender");
            Undo.RegisterCreatedObjectUndo(root, "Create City Root");
        }

        Dictionary<string, GameObject> prefabCache = new Dictionary<string, GameObject>();
        Dictionary<string, Transform> groupCache = new Dictionary<string, Transform>();

        int created = 0;
        int failedLoad = 0;
        List<string> failedPaths = new List<string>();

        foreach (var obj in data.objects)
        {
            if (obj.fbx_path == null) { failedLoad++; continue; }

            GameObject prefab;
            if (!prefabCache.TryGetValue(obj.fbx_path, out prefab))
            {
                prefab = AssetDatabase.LoadAssetAtPath<GameObject>(obj.fbx_path);
                prefabCache[obj.fbx_path] = prefab;
            }

            if (prefab == null)
            {
                failedLoad++;
                if (failedPaths.Count < 20) failedPaths.Add(obj.fbx_path);
                continue;
            }

            string groupName = obj.asset_key ?? "misc";
            Transform groupT;
            if (!groupCache.TryGetValue(groupName, out groupT))
            {
                GameObject g = new GameObject(groupName);
                g.transform.SetParent(root.transform, false);
                groupT = g.transform;
                groupCache[groupName] = groupT;
            }

            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            if (instance == null)
                instance = GameObject.Instantiate(prefab);

            instance.name = obj.name;
            instance.transform.SetParent(groupT, false);

            float[] l = obj.loc;
            float[] s = obj.scale;

            Vector3 pos = new Vector3(l[0], l[2], l[1]);
            Quaternion rot = Quaternion.Euler(0f, obj.yaw_deg, 0f);
            Vector3 scl = new Vector3(s[0], s[2], s[1]);

            instance.transform.localPosition = pos;
            instance.transform.localRotation = rot;
            instance.transform.localScale = scl;

            created++;
        }

        EditorUtility.SetDirty(root);
        string msg = "Created: " + created + " / " + data.objects.Length + " | FailedLoad: " + failedLoad;
        if (failedPaths.Count > 0)
            msg += " | SampleMissing: " + string.Join(", ", failedPaths);
        return msg;
    }

    public static string ClearPrevious()
    {
        GameObject root = GameObject.Find("ImportedCity_Blender");
        if (root != null)
        {
            GameObject.DestroyImmediate(root);
            return "Cleared previous ImportedCity_Blender";
        }
        return "Nothing to clear";
    }
}
