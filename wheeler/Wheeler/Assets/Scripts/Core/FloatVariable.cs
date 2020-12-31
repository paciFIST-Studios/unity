using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using pacifist.core;

[CreateAssetMenu(fileName = "New Float Variable", menuName = "paciFIST/Core/FloatVariable")]
public class FloatVariable : GenericVariable<float>
{
    // ctors
    public FloatVariable() { this.value = 0f; }
    public FloatVariable(float f) { this.value = f; }
    public FloatVariable(FloatVariable fv) { this.value = fv.value; }

    // modifiers
    public void SetValue(FloatVariable fv)
    {
        this.value = fv.value;
    }

    public void ApplyChange(float amount)
    {
        this.value += amount;
    }

    public void ApplyChange(FloatVariable amount)
    {
        this.value += amount.value;
    }

}
