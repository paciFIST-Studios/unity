using UnityEngine;

using pacifist.core;

[CreateAssetMenu(fileName = "New String Variable", menuName = "paciFIST/StringVariable")]
public class StringVariable : GenericVariable<string>
{
    public StringVariable() { this.value = string.Empty; }
    public StringVariable(string s) { this.value = s; }
    public StringVariable(StringVariable sv) { this.value = sv.value; }
    public void SetValue(StringVariable sv) { this.value = sv.value; }
    public static implicit operator string(StringVariable sv) { return sv.value; }
}
