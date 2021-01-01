using UnityEngine;

using pacifist.core;

using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "New Sprite Variable", menuName = "paciFIST/SpriteVariable")]
public class SpriteVariable : ScriptableObject
{
    [TableColumnWidth(60)]
    [PreviewField(50, ObjectFieldAlignment.Center)]
    public Sprite Value;

    [TableColumnWidth(100)]
    [LabelWidth(100)]
    public string DisplayName;

    [TableColumnWidth(200)]
    [MultiLineProperty]
    public string Description;


    public SpriteVariable() {}
    public SpriteVariable(Sprite s) { this.Value = s; }

    public void SetValue(Sprite s)
    {
        this.Value = s;
    }

    public static implicit operator Sprite(SpriteVariable s)
    {
        return s.Value;
    }
}
