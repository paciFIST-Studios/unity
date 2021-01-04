using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct MaterialCollection
{
    [SerializeField] public Material scannedOrange;
    [SerializeField] public Material scannedBerry;
    [SerializeField] public Material scannedLime;
                     
    [SerializeField] public Material unscannedLight;
    [SerializeField] public Material unscannedDark;
}

public enum ElementType
{
      Berry     = 0
    , Orange    = 1
    , Lime      = 2
    , Grape     = 3
}


public class DepricatedScannableObject : MonoBehaviour
{
    [SerializeField] private MaterialCollection materials;
    [SerializeField] private bool isScanned = false;

    [SerializeField] private float descanAfterSeconds = 1f;
    private float scannedAtTime;


    private MeshRenderer meshRenderer => GetComponent<MeshRenderer>();


    private void Update()
    {
        if(!isScanned) { return; }

        if (scannedAtTime + descanAfterSeconds <= Time.time)
        {
            meshRenderer.material = materials.unscannedLight;
            isScanned = false;
        }
    }


    private ElementType GetParticleElementType(GameObject obj)
    {
        var ps = obj.GetComponent<ParticleSystem>();
        var customData = new List<Vector4>();
        ps.GetCustomParticleData(customData, ParticleSystemCustomData.Custom1);
        var id = (int)customData[0].x;
        return (ElementType)id;
    }

    private void OnParticleCollision(GameObject other)
    {
        if(isScanned)
        {
            return;
        }

        isScanned = true;
        scannedAtTime = Time.time;

        var type = GetParticleElementType(other);
        if (type == ElementType.Berry)
        {
            meshRenderer.material = materials.scannedBerry;
        }
        else if (type == ElementType.Orange)
        {
            meshRenderer.material = materials.scannedOrange;
        }
        else if (type == ElementType.Lime)
        {
            meshRenderer.material = materials.scannedLime;
        }
    }


}
