using UnityEngine;

using pacifist.core;

[CreateAssetMenu(fileName = "New Sprite Variable", menuName = "paciFIST/SpriteVariable")]
public class SpriteVariable : GenericVariable<Sprite>
{
    public SpriteVariable() { }
    public SpriteVariable(Sprite s) { this.value = s; }

    public new void SetValue(Sprite s)
    {
        this.value = s;
    }

    public static implicit operator Sprite(SpriteVariable s)
    {
        return s.value;
    }

}
