using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightController : MonoBehaviour
{
    [SerializeField] private Material empty;
    [SerializeField] private Material occupied;
    
    private MeshRenderer meshRenderer;

    private void Start()
    {
        meshRenderer = this.GetComponent<MeshRenderer>();    
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            meshRenderer.material = occupied;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            meshRenderer.material = empty;
        }
    }
}
