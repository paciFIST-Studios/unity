using UnityEngine;
using UnityEditor;

using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "New Sprite Variable", menuName = "paciFIST/SpriteVariable")]
public class SpriteVariable : ScriptableObject
{
    [TableColumnWidth(60)]
    [PreviewField(50, ObjectFieldAlignment.Center)]
    public Sprite Value;

    [TableColumnWidth(100)]
    [LabelWidth(100)]
    public string DevName;

    [TableColumnWidth(200)]
    [MultiLineProperty]
    public string Description;


    public SpriteVariable() {}
    public SpriteVariable(Sprite s) { this.Value = s; }

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

    public Sprite GetValue() { return this.Value; }
    public void SetValue(Sprite s)
    {
        this.Value = s;
    }

    public static implicit operator Sprite(SpriteVariable s)
    {
        return s.Value;
    }
}
