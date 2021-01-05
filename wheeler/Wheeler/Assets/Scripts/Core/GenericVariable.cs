using UnityEngine;
using UnityEditor;

using Sirenix.OdinInspector;

namespace pacifist
{
    namespace core
    {               
        // https://github.com/roboryantron/Unite2017/blob/master/Assets/Code/Variables/FloatVariable.cs

        public abstract class GenericVariable<T> : ScriptableObject
        {
            private bool canEditDevName = false;
            private bool canEditDescription = false;
            private bool canEditValue = false;

            private void ToggleEditDevName() { canEditDevName = !canEditDevName; }
            private void ToggleEditDescription() { canEditDescription = !canEditDescription; }
            private void ToggleEditValue() { canEditValue = !canEditValue; }

            [CustomContextMenu("ToggleEdit/DevName", "ToggleEditDevName")]
            [EnableIf("canEditDevName")]
            public string DevName = "";

            [CustomContextMenu("ToggleEdit/Description", "ToggleEditDescription")]
            [EnableIf("canEditDescription")]
            public string Description = "";

            [CustomContextMenu("ToggleEdit/Value", "ToggleEditValue")]
            [EnableIf("canEditValue")]
            public T value;
            
            public T GetValue() { return value; }

            public void SetValue(T val)
            {
                AssetDatabase.Refresh();
                value = val;
                EditorUtility.SetDirty(this);
                AssetDatabase.SaveAssets();
            }
        }

    }  // core
}      // pacifist
