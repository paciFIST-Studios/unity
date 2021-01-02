using System;

using UnityEngine;

using pacifist.core;

using Sirenix.OdinInspector;

[Serializable]
public class SpriteReference : GenericVariableReference<Sprite>
{
    [FoldoutGroup("$GuiTitle")]
    [HorizontalGroup("$GuiTitle/Column/Bottom")]
    [OnStateUpdate("UpdateReference")]
    [LabelWidth(85)]
    public new SpriteVariable Reference;

    public SpriteReference()
    {
        this.GuiTitle = "SpriteReference";
        UpdateReference();
    }

    public SpriteReference(Sprite s)
    {
        this.UseOverride = true;
        this.OverrideValue = s;
        this.GuiTitle = "SpriteReference";

        UpdateReference();
    }

    public Sprite Value
    {
        get { return UseOverride ? OverrideValue : Reference.Value; }
    }

    public static implicit operator Sprite(SpriteReference sr)
    {
        return sr.Value;
    }


    public override void UpdateReference()
    {
        if (Reference)
        {
            this.GuiTitle = this.Reference.DevName;
            this.ReferencedValue = this.Reference.Value;
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
