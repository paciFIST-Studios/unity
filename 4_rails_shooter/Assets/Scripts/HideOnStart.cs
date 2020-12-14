using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideOnStart : MonoBehaviour
{
    [SerializeField] private bool HideOnSceneStart = true;

    void Start()
    {
        GetComponent<MeshRenderer>().enabled = !HideOnSceneStart;
    }

}
