using System;

using UnityEngine;

using pacifist.core;

using Sirenix.OdinInspector;

[Serializable]
public class StringReference : GenericVariableReference<string>
{
    [FoldoutGroup("$GuiTitle")]
    [HorizontalGroup("$GuiTitle/Column/Bottom")]
    [OnStateUpdate("UpdateReference")]
    [LabelWidth(85)]
    public new StringVariable Reference;

    public StringReference()
    {
        this.GuiTitle = "StringReference";
        UpdateReference();
    }

    public StringReference(string s)
    {
        this.UseOverride = true;
        this.OverrideValue = s;
        this.GuiTitle = "StringReference";
        UpdateReference();
    }

    public string Value
    {
        get { return UseOverride ? OverrideValue : Reference.value; }
    }

    public override void UpdateReference()
    {
        if (Reference)
        {
            this.GuiTitle = this.Reference.DisplayName;
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
