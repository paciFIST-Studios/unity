// https://forum.unity.com/threads/editor-tool-better-scriptableobject-inspector-editing.484393/
// Thanks Fydar!


using System;
using UnityEngine;

#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
#endif


/// <summary>
/// Use this property on a ScriptableObject type to allow the editors drawing the field
/// to draw an expandable area, that allows for editing the values on the object, 
/// without having to change the editor
/// </summary>
public class ExpandableAttribute : PropertyAttribute
{
    public ExpandableAttribute() { }
}

#if UNITY_EDITOR
/// <summary>
/// Draws a property field for any field marked with [ExpandableAttribute]
/// </summary>
[CustomPropertyDrawer(typeof(ScriptableObject), true)]
public class ScriptableObjectCustomPropertyDrawer : PropertyDrawer
{
    private enum BackgroundStyle
    {
        None,
        HelpBox,
        Darken,
        Lighten
    }

    /// <summary>
    /// Whether the default editor's script field will be shown
    /// </summary>
    private static bool SHOW_SCRIPT_FIELD = false;

    /// <summary>
    /// Spacing inside a background rect
    /// </summary>
    private static float INNER_SPACING = 6.0f;

    /// <summary>
    /// Spacing outside a background rect
    /// </summary>
    private static float OUTER_SPACING = 4.0f;

    /// <summary>
    /// The style used when drawing backgrounds
    /// </summary>
    private static BackgroundStyle BACKGROUND_STYLE = BackgroundStyle.HelpBox;
    private static Color DARKEN_COLOR = new Color(0.0f, 0.0f, 0.0f, 0.2f);
    private static Color LIGHTEN_COLOR = new Color(1.0f, 1.0f, 1.0f, 0.2f);

    // cached reference to the editor which will work on scriptable objects
    private Editor editor = null;



    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float totalHeight = 0.0f;

        totalHeight += EditorGUIUtility.singleLineHeight;

        if (property.objectReferenceValue == null)
            return totalHeight;

        if (!property.isExpanded)
            return totalHeight;

        var targetObject = new SerializedObject(property.objectReferenceValue);
        if (targetObject == null)
            return totalHeight;

        SerializedProperty field = targetObject.GetIterator();
        field.NextVisible(true);

        if(SHOW_SCRIPT_FIELD)
        {
            totalHeight += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        }

        while(field.NextVisible(false))
        {
            totalHeight += EditorGUI.GetPropertyHeight(field, true) + EditorGUIUtility.standardVerticalSpacing;
        }

        totalHeight += INNER_SPACING * 2;
        totalHeight += OUTER_SPACING * 2;

        return totalHeight;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var fieldRect = new Rect(position);
        fieldRect.height = EditorGUIUtility.singleLineHeight;

        EditorGUI.PropertyField(fieldRect, property, label, true);

        if(property.objectReferenceValue == null)
            return;

        property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, GUIContent.none, false);
        if (!property.isExpanded)
            return;

        var targetObject = new SerializedObject(property.objectReferenceValue);
        if (targetObject == null)
            return;

        var propertyRects = new List<Rect>();
        var marchingRect = new Rect(fieldRect);
        var bodyRect     = new Rect(fieldRect);

        bodyRect.xMin += EditorGUI.indentLevel * 14;
        bodyRect.yMin += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing + OUTER_SPACING;

        SerializedProperty field = targetObject.GetIterator();
        field.NextVisible(true);

        marchingRect.y += INNER_SPACING + OUTER_SPACING;

        if (SHOW_SCRIPT_FIELD)
        {
            propertyRects.Add(marchingRect);
            marchingRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        }

        while(field.NextVisible(false))
        {
            marchingRect.y += marchingRect.height + EditorGUIUtility.standardVerticalSpacing;
            marchingRect.height = EditorGUI.GetPropertyHeight(field, true);
            propertyRects.Add(marchingRect);
        }

        marchingRect.y += INNER_SPACING;
        bodyRect.yMax = marchingRect.yMax;

        DrawBackground(bodyRect);

        EditorGUI.indentLevel++;

        int index = 0;
        field = targetObject.GetIterator();
        field.NextVisible(true);

        if(SHOW_SCRIPT_FIELD)
        {
            // draw the script field as a disabled group
            EditorGUI.BeginDisabledGroup(true);
            EditorGUI.PropertyField(propertyRects[index], field, true);
            EditorGUI.EndDisabledGroup();
            index++;
        }

        // this replaces editor.OnInspectorGUI, and provides more granular control of how the editor is drawn
        while(field.NextVisible(false))
        {
            try
            {
                EditorGUI.PropertyField(propertyRects[index], field, true);
            }
            catch(StackOverflowException)
            {
                field.objectReferenceValue = null;
                Debug.LogError("Detected a self-nested reference, which caused StackOverflowException.  Do not nest an object inside of itself");
            }

            index++;
        }

        targetObject.ApplyModifiedProperties();

        EditorGUI.indentLevel--;
    }

    private void DrawBackground(Rect rect)
    {
        switch(BACKGROUND_STYLE)
        {
            case BackgroundStyle.HelpBox:
                EditorGUI.HelpBox(rect, "", MessageType.None);
                break;

            case BackgroundStyle.Darken:
                EditorGUI.DrawRect(rect, DARKEN_COLOR);
                break;

            case BackgroundStyle.Lighten:
                EditorGUI.DrawRect(rect, LIGHTEN_COLOR);
                break;


            case BackgroundStyle.None:
                break;
        }
    }
}
#endif
