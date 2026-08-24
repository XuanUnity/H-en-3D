using UnityEngine;

public class HoleCollision : MonoBehaviour
{



    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Building"))
        {
            other.gameObject.layer = LayerMask.NameToLayer("NoCollider");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Building"))
        {
            other.gameObject.layer = LayerMask.NameToLayer("Default");
        }
    }
}
