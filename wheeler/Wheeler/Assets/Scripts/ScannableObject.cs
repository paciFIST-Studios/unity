using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct MaterialCollection
{
    [SerializeField] public Material scannedOrange;
    [SerializeField] public Material scannedBerry;
                     
    [SerializeField] public Material unscannedLight;
    [SerializeField] public Material unscannedDark;
}


public class ScannableObject : MonoBehaviour
{
    [SerializeField] private MaterialCollection materials;

    private MeshRenderer meshRenderer => GetComponent<MeshRenderer>();

    private bool isScanned = false;




    private void OnParticleCollision(GameObject other)
    {

        if(isScanned)
        {
            return;
        }

        isScanned = true;

        List<Vector4> customData = new List<Vector4>();

        var ps = other.gameObject.GetComponent<ParticleSystem>();
        ps.GetCustomParticleData(customData, ParticleSystemCustomData.Custom1);


        if (customData[0].x == 0)
        {
            meshRenderer.material = materials.scannedBerry;
        }
        else if (customData[0].x == 1)
        {
            meshRenderer.material = materials.scannedOrange;
        }

        print(other.gameObject.name);

    }


}
