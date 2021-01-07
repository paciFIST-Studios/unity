
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

using Sirenix.OdinInspector;

[System.Serializable]
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
        this.DevName = name;
        RefreshAsset();
    }

    public string GetDescription() { return this.Description; }
    public void SetDescription(string description)
    {
        this.Description = description;
        RefreshAsset();
    }

    public Material GetValue() { return this.Value; }
    public void SetValue(Material m)
    {
        this.Value = m;
        RefreshAsset();
    }

    public static implicit operator Material(MaterialVariable m)
    {
        return m.Value;
    }

    private void RefreshAsset()
    {
#if UNITY_EDITOR
        AssetDatabase.Refresh();
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
#endif
    }

}
