using System;
using UnityEngine;
using UnityEditor;

using Sirenix.OdinInspector;

// A lof of the "unused" values are used in editor,
// but only during value modification
#pragma warning disable CS0414

[Serializable]
public class MaterialReference
{
    private bool canEditValue = false;
    private bool canEditDevName = false;
    private bool canEditDescription = false;

    //private void ToggleEditValue() { canEditValue = !canEditValue; }
    //private void ToggleEditDevName() { canEditDevName = !canEditDevName; }
    //private void ToggleEditDescription() { canEditDescription = !canEditDescription; }

    [HorizontalGroup("Row", width: 110)]
    [VerticalGroup("Row/Column1")]
    [OnValueChanged("UpdateReference")]
    [LabelWidth(85)]
    public bool UseOverride = true;

    [HideLabel]
    [HorizontalGroup("Row")]
    [VerticalGroup("Row/Column1")]
    [ShowIf("UseOverride")]
    [LabelWidth(105)]
    [PreviewField(ObjectFieldAlignment.Center)]
    [CustomContextMenu("Overwrite Reference with Current", "SaveOverrideToReference")]
    public Material OverrideValue;

    [HideLabel]
    [HorizontalGroup("Row")]
    [VerticalGroup("Row/Column1")]
    [HideIf("UseOverride")]
    [LabelWidth(105)]
    [PreviewField(ObjectFieldAlignment.Center)]
    [EnableIf("canEditValue")]
    public Material ReferencedValue;
       
    [HideIf("UseOverride")]
    [HorizontalGroup("Row")]
    [VerticalGroup("Row/Column2")]
    [LabelWidth(80)]
    [OnStateUpdate("UpdateReference")]
    public MaterialVariable Reference;

    [HorizontalGroup("Row")]
    [VerticalGroup("Row/Column2")]
    [LabelWidth(80)]
    [HideIf("UseOverride")]
    [EnableIf("canEditDevName")]
    public string DevName;

    [HorizontalGroup("Row")]
    [VerticalGroup("Row/Column2")]
    [LabelWidth(100)]
    [ShowIf("UseOverride")]
    [ReadOnly]
    public string OverrideAsset;
    
    [HorizontalGroup("Row")]
    [VerticalGroup("Row/Column2")]
    [LabelWidth(80)]
    [HideIf("UseOverride")]
    [EnableIf("canEditDescription")]
    public string Description;

    public MaterialReference()
    {
        UpdateReference();
    }

    public MaterialReference(Material m)
    {
        this.UseOverride = true;
        this.OverrideValue = m;
        UpdateReference();
    }

    public Material Value
    {
        get
        {
            if (UseOverride)
            {
                return OverrideValue;
            }
            else
            {
                if (!Reference)
                {
                    Debug.LogWarning(string.Format($"WARNING, missing reference!"));
                }

                return (Reference) ? Reference.Value : null;
            }
        }
    }
    

    public static implicit operator Material(MaterialReference m)
    {
        if (m == null) { return null; }
    
        return m.Value;
    }
    
    public void UpdateReference()
    {
        //AssetDatabase.Refresh();

        if(Reference)
        {
            this.ReferencedValue = this.Reference.Value;
            this.DevName = this.Reference.DevName;
            this.Description = this.Reference.Description;
        }
        else
        {
            this.ReferencedValue = null;
            this.DevName = string.Empty;
            this.Description = string.Empty;
        }

        if(this.OverrideValue)
        {
            this.OverrideAsset = this.OverrideValue.ToString();
        }
        else
        {
            this.OverrideAsset = string.Empty;
        }

        //EditorUtility.SetDirty(this);
        //AssetDatabase.SaveAssets();
    }
    
    public void SaveOverrideToReference()
    {
        if(Reference)
        {
            this.Reference.SetValue(OverrideValue);
            UpdateReference();
        }
    }

}
