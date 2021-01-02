using System;
using UnityEngine;

using pacifist.core;

using Sirenix.OdinInspector;


[Serializable]
//[CreateAssetMenu(fileName = "New Float Reference", menuName = "paciFIST/Core/FloatReference")]
public class FloatReference : GenericVariableReference<float>
{
    [FoldoutGroup("$GuiTitle")]
    [HorizontalGroup("$GuiTitle/Column/Bottom")]
    [OnStateUpdate("UpdateReference")]
    [LabelWidth(85)]
    public new FloatVariable Reference;
    
    public FloatReference()
    {
        this.GuiTitle = "FloatReference";
        UpdateReference();
    }

    public FloatReference(float f)
    {
        this.UseOverride = true;
        this.OverrideValue = f;
        this.GuiTitle = "FloatReference";

        UpdateReference();
    }

    public float Value
    {
        get
        {
            if(UseOverride)
            {
                return OverrideValue;
            }
            else
            {
                if(!Reference)
                {
                    Debug.LogWarning(string.Format($"WARNING, missing reference!\n\tUseOverride: {0}, OverrideValue: {1}, ReferenceValue: {2}, Reference: {3}", this.UseOverride, this.OverrideValue, this.ReferencedValue, this.Reference));
                }

                // sometimes reference is null
                return (Reference) ? Reference.value : 0.0f;
            }
        }
    }

    public static implicit operator float(FloatReference fr)
    {
        if(fr == null) { return 0.0f; }

        return fr.Value;
    }


    public override void UpdateReference()
    {
        if(Reference)
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
