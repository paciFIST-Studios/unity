using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Triggerable Scanner", menuName = "paciFIST/TriggerableScanner")]
public class TriggerableScanner : TriggerableSkillPayloadBase
{
    public string name;
    public float cooldown;

    public ParticleSystem particleSystem;

    private float lastUsed;


    // name or enum

    // last time fired
    // cooldown
    // particle system

    public override void Invoke()
    {
        // if cooldown elapsed
            // fire
    }
}
