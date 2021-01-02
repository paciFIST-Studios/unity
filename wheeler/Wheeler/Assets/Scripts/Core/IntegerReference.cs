using System;

using UnityEngine;

using pacifist.core;

using Sirenix.OdinInspector;

[Serializable]
public class IntegerReference : GenericVariableReference<int>
{
    [FoldoutGroup("$GuiTitle")]
    [HorizontalGroup("$GuiTitle/Column/Bottom")]
    [OnStateUpdate("UpdateReference")]
    [LabelWidth(85)]
    public new IntegerVariable Reference;

    public IntegerReference()
    {
        this.GuiTitle = "IntegerReference";
        UpdateReference();
    }

    public IntegerReference(int i)
    {
        this.UseOverride = true;
        this.OverrideValue = i;
        this.GuiTitle = "IntegerReference";

        UpdateReference();
    }

    public int Value { get { return UseOverride ? OverrideValue : Reference.value; } }
    public static implicit operator int(IntegerReference ir)
    {
        if (ir == null) { return 0; }
        return ir.Value;
    }


    public override void UpdateReference()
    {
        if (Reference)
        {
            this.GuiTitle = this.Reference.DevName;
            this.ReferencedValue = this.Reference.value;
        }
    }
    public override void SaveOverrideToReference()
    {
        if (Reference)
        {
            this.Reference.SetValue(OverrideValue);
            UpdateReference();
        }
    }
}
