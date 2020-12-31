using System;

using Sirenix.OdinInspector;

namespace pacifist
{
    namespace core
    {
        // https://github.com/roboryantron/Unite2017/blob/master/Assets/Code/Variables/FloatReference.cs
        
        public abstract class GenericVariableReference<T>
        {
            [FoldoutGroup("$GuiTitle")]
            [VerticalGroup("$GuiTitle/Column")]
            [HorizontalGroup("$GuiTitle/Column/Top", width: 100)]
            [OnValueChanged("UpdateValueDisplay")]
            [LabelWidth(85)]
            public bool UseOverride = true;

            [FoldoutGroup("$GuiTitle")]
            [HorizontalGroup("$GuiTitle/Column/Top", width: 1200)]
            [LabelWidth(105)]
            [ShowIf("UseOverride")]
            public T OverrideValue;

            [FoldoutGroup("$GuiTitle")]
            [HideIf("UseOverride")]
            [HorizontalGroup("$GuiTitle/Column/Top", marginLeft: -1200, width: 1200)]
            [LabelWidth(105)]
            [ReadOnly]
            public T ReferencedValue;

            [HideReferenceObjectPicker]
            [HideLabel]
            [FoldoutGroup("$GuiTitle")]
            [LabelWidth(100)]
            public GenericVariable<T> Reference;

            public T Value
            {
                get { return UseOverride ? OverrideValue : Reference.value; }
            }

            public static implicit operator T(GenericVariableReference<T> reference)
            {
                return reference.Value;
            }

            protected string GuiTitle = "GenericVariableReference<T>";

            private void UpdateValueDisplay()
            {
                if (Reference == null) { return; }

                this.ReferencedValue = Reference.value;

            }
        }


    }
}
