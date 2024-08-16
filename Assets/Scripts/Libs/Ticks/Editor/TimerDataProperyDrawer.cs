using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(Timer.TimerData))]
public class TimerDataDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // Set the indent level
        int indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        // Calculate the rects
        Rect startOnAwakeRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        Rect oneShotRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + 2, position.width, EditorGUIUtility.singleLineHeight);
        Rect durationRect = new Rect(position.x, position.y + (EditorGUIUtility.singleLineHeight + 2) * 2, position.width, EditorGUIUtility.singleLineHeight);

        // Draw the fields
        EditorGUI.PropertyField(startOnAwakeRect, property.FindPropertyRelative("startOnAwake"));
        EditorGUI.PropertyField(oneShotRect, property.FindPropertyRelative("oneShot"));
        EditorGUI.PropertyField(durationRect, property.FindPropertyRelative("duration"));

        // Reset the indent level
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return (EditorGUIUtility.singleLineHeight + 2) * 3;
    }
}
