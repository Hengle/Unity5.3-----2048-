using UnityEngine;
using UnityEngine.UI;
using UnityEditorInternal;
using UnityEditor.AnimatedValues;

namespace UnityEditor.UI
{

	[CustomEditor(typeof(AutoGridLayout), true)]
    [CanEditMultipleObjects]
    /// <summary>
    /// AutoGridLayout µÄ±à¼­Æ÷ ÏÔÊ¾
    /// </summary>
    public class AutoGridLayoutEditor : Editor
    {
		SerializedProperty m_IsColumn;
		SerializedProperty m_Column;
		SerializedProperty m_Row;

        SerializedProperty m_Padding;
        SerializedProperty m_Spacing;
        SerializedProperty m_StartCorner;
        SerializedProperty m_StartAxis;
        SerializedProperty m_ChildAlignment;

        protected virtual void OnEnable()
        {
			m_IsColumn = serializedObject.FindProperty("m_IsColumn");
			m_Column = serializedObject.FindProperty("m_Column");
			m_Row = serializedObject.FindProperty("m_Row");


            m_Padding = serializedObject.FindProperty("m_Padding");
            m_Spacing = serializedObject.FindProperty("m_Spacing");
            m_StartCorner = serializedObject.FindProperty("m_StartCorner");
            m_StartAxis = serializedObject.FindProperty("m_StartAxis");
            m_ChildAlignment = serializedObject.FindProperty("m_ChildAlignment");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

			EditorGUILayout.PropertyField(m_IsColumn, true);
			if (!m_IsColumn.boolValue) {
				EditorGUILayout.PropertyField(m_Row, true);
			} else {
				EditorGUILayout.PropertyField(m_Column, true);
			}
				

            EditorGUILayout.PropertyField(m_Padding, true);
            EditorGUILayout.PropertyField(m_Spacing, true);
            EditorGUILayout.PropertyField(m_StartCorner, true);
            EditorGUILayout.PropertyField(m_StartAxis, true);
            EditorGUILayout.PropertyField(m_ChildAlignment, true);
            serializedObject.ApplyModifiedProperties();
        }
    }
}
