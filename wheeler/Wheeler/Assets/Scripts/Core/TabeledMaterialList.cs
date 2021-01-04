using System.Collections.Generic;
using UnityEngine;

using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "Tabled Material List", menuName = "paciFIST/TabledMaterialList")]
public class TabeledMaterialList : ScriptableObject
{
    [TableList][SerializeField]
    List<MaterialVariable> materials = new List<MaterialVariable>();
}
