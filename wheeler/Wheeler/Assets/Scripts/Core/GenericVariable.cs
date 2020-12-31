using UnityEngine;

namespace pacifist
{
    namespace core
    {
        // https://github.com/roboryantron/Unite2017/blob/master/Assets/Code/Variables/FloatVariable.cs

        public abstract class GenericVariable<T> : ScriptableObject
        {
#if UNITY_EDITOR
            [Multiline(2)]
            public string DeveloperDescription = "";
#endif

            public T value;

            public T GetValue()
            {
                return value;
            }

            public void SetValue(T val)
            {
                value = val;
            }
        }

    }  // core
}      // pacifist
