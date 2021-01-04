
using UnityEngine;

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

    public void SetValue(Material m)
    {
        this.Value = m;
    }

    public static implicit operator Material(MaterialVariable m)
    {
        return m.Value;
    }

}
