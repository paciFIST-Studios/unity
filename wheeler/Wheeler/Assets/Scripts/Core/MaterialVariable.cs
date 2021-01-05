
using UnityEngine;
using UnityEditor;

using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "New Material Variable", menuName = "paciFIST/MaterialVariable")]
public class MaterialVariable : ScriptableObject
{
    [TableColumnWidth(60)]
    [PreviewField(50, ObjectFieldAlignment.Center)]
    public Material Value;

    [TableColumnWidth(100)]
    [LabelWidth(100)]
    public string DevName;

    [TableColumnWidth(200)]
    [MultiLineProperty]
    public string Description;

    public MaterialVariable() { }
    public MaterialVariable(Material m) { this.Value = m; }

    public string GetDevName() { return this.DevName; }
    public void SetDevName(string name)
    {
        AssetDatabase.Refresh();
        this.DevName = name;
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
    }

    public string GetDescription() { return this.Description; }
    public void SetDescription(string description)
    {
        AssetDatabase.Refresh();
        this.Description = description;
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
    }

    public Material GetValue() { return this.Value; }
    public void SetValue(Material m)
    {
        AssetDatabase.Refresh();
        this.Value = m;
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
    }

    public static implicit operator Material(MaterialVariable m)
    {
        return m.Value;
    }

}
