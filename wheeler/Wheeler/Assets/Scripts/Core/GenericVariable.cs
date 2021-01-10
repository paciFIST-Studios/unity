using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

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

            public T GetValue() { return value; }
            public void SetValue(T val)
            {
                value = val;
                RefreshAsset();
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

    }  // core
}      // pacifist
