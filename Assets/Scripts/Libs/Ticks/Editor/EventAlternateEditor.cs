using UnityEditor;

[CustomEditor(typeof(EventAlternator))]
[CanEditMultipleObjects]
public class EventTimerAlternatorEditor : Editor
{
    SerializedProperty startOnAwakeProp;
    SerializedProperty oneShotProp;
    SerializedProperty durationProp;
    SerializedProperty onTimerAProp;
    SerializedProperty onTimerBProp;
    SerializedProperty alternationDuration;

    void OnEnable()
    {
        startOnAwakeProp = serializedObject.FindProperty("timerData.startOnAwake");
        oneShotProp = serializedObject.FindProperty("timerData.oneShot");
        durationProp = serializedObject.FindProperty("timerData.duration");
        onTimerAProp = serializedObject.FindProperty("OnTimerA");
        onTimerBProp = serializedObject.FindProperty("OnTimerB");
        alternationDuration = serializedObject.FindProperty("alternationDuration");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(startOnAwakeProp);
        EditorGUILayout.PropertyField(oneShotProp);
        EditorGUILayout.PropertyField(durationProp);
        EditorGUILayout.PropertyField(alternationDuration);
        EditorGUILayout.PropertyField(onTimerAProp);
        EditorGUILayout.PropertyField(onTimerBProp);

        serializedObject.ApplyModifiedProperties();
    }
}
