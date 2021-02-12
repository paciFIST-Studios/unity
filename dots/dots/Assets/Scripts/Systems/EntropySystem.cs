using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;


public class EntropySystem : ComponentSystem
{
    // degredation per second
    public float degradePS = 1f;

    protected override void OnUpdate()
    {
        Entities.ForEach((ref EntropyComponent entropyComponent) =>
        {
            entropyComponent.value -= degradePS * Time.DeltaTime;
        });
    }
}
