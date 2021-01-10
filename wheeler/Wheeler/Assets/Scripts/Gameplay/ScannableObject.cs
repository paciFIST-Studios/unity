
using UnityEngine;

using Sirenix.OdinInspector;

public class ScannableObject : MonoBehaviour
{
    private Material defaultMaterial;
    private MeshRenderer meshRenderer;

    public bool IsScanned = false;

    [BoxGroup("OnScanMaterial")]
    [HideLabel]
    public MaterialReference OnScanMaterial;

    private void Start()
    {
        meshRenderer = this.GetComponent<MeshRenderer>();
        defaultMaterial = meshRenderer.material;
    }


    private void OnParticleTrigger()
    {
    }

    public void OnParticleCollision(GameObject other)
    {
        if (IsScanned) { return; }

        SetIsScanned(true);                
    }
    
    private void SetIsScanned(bool val)
    {
        IsScanned = val;
        if (meshRenderer)
        {
            meshRenderer.material = (IsScanned) ? OnScanMaterial.Value : defaultMaterial;
        }
    }
}
