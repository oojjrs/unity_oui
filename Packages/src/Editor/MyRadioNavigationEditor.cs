using UnityEditor;

namespace oojjrs.oui
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(MyRadioNavigation))]
    public sealed class MyRadioNavigationEditor : UnityEditor.Editor
    {
        private SerializedProperty _navigation;
        private SerializedProperty _script;

        private void OnEnable()
        {
            _navigation = serializedObject.FindProperty("m_Navigation");
            _script = serializedObject.FindProperty("m_Script");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            using (new EditorGUI.DisabledScope(true))
                EditorGUILayout.PropertyField(_script);

            EditorGUILayout.PropertyField(_navigation);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
