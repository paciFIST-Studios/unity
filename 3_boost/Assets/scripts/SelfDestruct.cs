using UnityEngine;

public class SelfDestruct : MonoBehaviour
{

    [SerializeField] Material destructedMaterial;

    [SerializeField] MeshRenderer[] childMeshRenderers;

    public void selfDestruct()
    {
        print("Destroying Object!");
        foreach(var child in childMeshRenderers)
        {
            child.material = destructedMaterial;
        }

        //transform.DetachChildren();
    }	
}

