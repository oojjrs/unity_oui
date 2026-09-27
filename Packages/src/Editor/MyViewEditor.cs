using UnityEditor;

namespace oojjrs.oui
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(MyView))]
    public sealed class MyViewEditor : UnityEditor.Editor
    {
        private SerializedProperty _prefab;
        private SerializedProperty _script;

        private void OnEnable()
        {
            _prefab = serializedObject.FindProperty("_prefab");
            _script = serializedObject.FindProperty("m_Script");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            using (new EditorGUI.DisabledScope(true))
                EditorGUILayout.PropertyField(_script);

            EditorGUILayout.HelpBox("Prefab은 간단한 초기 설정용입니다. 런타임에는 Prefab 프로퍼티로 동적으로 교체합니다.", MessageType.Info);
            EditorGUILayout.PropertyField(_prefab);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
