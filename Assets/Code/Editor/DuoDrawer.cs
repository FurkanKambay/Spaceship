using FK.Common;
using UnityEditor;
using UnityEngine;

namespace FK.Spaceship.Editor
{
    [CustomPropertyDrawer(typeof(Duo))]
    internal sealed class DuoDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position = EditorGUI.PrefixLabel(position, label);
            position.width /= 2f;
            position.width -= 2;

            float oldLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = position.width / 4f;

            property.NextVisible(true);
            EditorGUI.PropertyField(position, property, new GUIContent("Left"), true);

            property.NextVisible(true);
            position.x += position.width + 4;
            EditorGUI.PropertyField(position, property, new GUIContent("Right"), true);

            EditorGUIUtility.labelWidth = oldLabelWidth;
        }
    }
}
