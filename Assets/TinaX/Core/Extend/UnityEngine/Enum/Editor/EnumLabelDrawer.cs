using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TinaX;


namespace TinaXEditor
{
    [CustomPropertyDrawer(typeof(EnumLabelAttribute))]
    public class EnumLabelDrawer : PropertyDrawer
    {
        private readonly List<string> m_displayNames = new List<string>();

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var att = (EnumLabelAttribute)attribute;
            var type = property.serializedObject.targetObject.GetType();
            var field = type.GetField(property.name);
            var enumtype = field.FieldType;
            foreach (var enumName in property.enumNames)
            {
                var enumfield = enumtype.GetField(enumName);
                var hds = enumfield.GetCustomAttributes(typeof(HeaderAttribute), false);
                m_displayNames.Add(hds.Length <= 0 ? enumName : ((HeaderAttribute)hds[0]).header);
            }
            EditorGUI.BeginChangeCheck();
            var value = EditorGUI.Popup(position, att.header, property.enumValueIndex, m_displayNames.ToArray());
            if (EditorGUI.EndChangeCheck())
            {
                property.enumValueIndex = value;
            }
        }
    }
}

