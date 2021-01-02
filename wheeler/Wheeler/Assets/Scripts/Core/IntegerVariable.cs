using UnityEngine;

using pacifist.core;

[CreateAssetMenu(fileName = "New Integer Variable", menuName = "paciFIST/IntegerVariable")]
public class IntegerVariable : GenericVariable<int>
{
    public IntegerVariable() { this.value = 0; }
    public IntegerVariable(int i) { this.value = i; }
    public IntegerVariable(IntegerVariable iv) { this.value = iv.value; }
    public void SetValue(IntegerVariable iv) { this.value = iv.value; }
    public static implicit operator int(IntegerVariable iv) { return iv.value; }
}
