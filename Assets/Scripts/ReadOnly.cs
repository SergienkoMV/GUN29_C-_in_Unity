using System.Reflection.Emit;
using UnityEngine;
using UnityEngine.UIElements;

public class ReadOnlyAttribute : PropertyAttribute
{
    
}

#if UNITY_EDITOR
[UnityEditor.CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyPropertyDrawer : UnityEditor.PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(UnityEditor.SerializedProperty property)
    {
        var element = base.CreatePropertyGUI(property)
          ?? new UnityEditor.UIElements.PropertyField(property);

        element.SetEnabled(false);
        return element;
    }
}
#endif