using System;

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

    public new float Value
    {
        get { return UseOverride ? OverrideValue : Reference.value; }
    }

    public static implicit operator float(FloatReference fr)
    {
        return fr.Value;
    }


    public override void UpdateReference()
    {
        if(Reference)
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
