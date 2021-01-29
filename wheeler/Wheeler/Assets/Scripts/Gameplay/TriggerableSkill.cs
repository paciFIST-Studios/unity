using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Triggerable Skill", menuName = "paciFIST/TriggerableSkill")]
public class TriggerableSkill : ScriptableObject
{
    // upgrade icon
    // equipped icon (if can be equipped)
    // invokable payload?
        // handles cooldown?


    public TriggerableSkillPayloadBase payload;
}
