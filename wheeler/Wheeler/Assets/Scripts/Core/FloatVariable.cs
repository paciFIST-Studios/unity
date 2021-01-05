using UnityEngine;

using pacifist.core;

using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "New Float Variable", menuName = "paciFIST/FloatVariable")]
[InlineEditor]
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

    // NOTE: 
    public static implicit operator float(FloatVariable fv)
    {
        return fv.value;
    }

}
