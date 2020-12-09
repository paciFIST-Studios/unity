using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallDestructOnRootParent : MonoBehaviour
{
    [SerializeField] private Transform rootParent;

    public void Destruct()
    {
        print("Calling Destroy!");
        rootParent.GetComponent<SelfDestruct>().selfDestruct();
    }
}
