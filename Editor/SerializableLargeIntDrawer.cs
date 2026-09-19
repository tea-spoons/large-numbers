namespace TeaSpoons.LargeNumbers.Editor
{
    using UnityEngine;
    using UnityEditor;

    [CustomPropertyDrawer(typeof(SerializableLargeInt))]
    public class SerializableLargeIntDrawer : PropertyDrawer
    {
        private const float StringFieldHeight = 18f;
        private const float Padding = 2f;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight + StringFieldHeight + Padding;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var digitsProperty = property.FindPropertyRelative("digits");
            var exponentProperty = property.FindPropertyRelative("exponent");

            var stringFieldRect = new Rect(position.x, position.y, position.width, StringFieldHeight);
            DrawStringField(stringFieldRect, digitsProperty, exponentProperty, label);

            var numericRowRect = new Rect(position.x, position.y + StringFieldHeight + Padding, position.width, EditorGUIUtility.singleLineHeight);
            DrawNumericFields(numericRowRect, digitsProperty, exponentProperty, label);

            EditorGUI.EndProperty();
        }

        private static void DrawStringField(Rect position, SerializedProperty digitsProperty, SerializedProperty exponentProperty, GUIContent label)
        {
            string currentString = BuildString(digitsProperty.intValue, (ushort)exponentProperty.intValue);

            var labelRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, position.height);
            var fieldRect = new Rect(position.x + EditorGUIUtility.labelWidth, position.y, position.width - EditorGUIUtility.labelWidth, position.height);

            EditorGUI.LabelField(labelRect, label);

            EditorGUI.BeginChangeCheck();
            string input = EditorGUI.TextField(fieldRect, currentString);
            if (EditorGUI.EndChangeCheck())
            {
                string cleaned = System.Text.RegularExpressions.Regex.Replace(input, @"[^0-9]", "");
                if (cleaned.Length == 0)
                {
                    digitsProperty.intValue = 0;
                    exponentProperty.intValue = 0;
                }
                else if (int.TryParse(cleaned, out int parsed))
                {
                    digitsProperty.intValue = parsed;
                    exponentProperty.intValue = cleaned.Length - 1;
                }
            }
        }

        private static void DrawNumericFields(Rect position, SerializedProperty digitsProperty, SerializedProperty exponentProperty, GUIContent label)
        {
            var indentedRect = new Rect(position.x + EditorGUIUtility.labelWidth, position.y, position.width - EditorGUIUtility.labelWidth, position.height);

            float halfWidth = indentedRect.width / 2f - 20f;

            var leftRect = new Rect(indentedRect.x, indentedRect.y, halfWidth, indentedRect.height);
            EditorGUI.PropertyField(leftRect, digitsProperty, GUIContent.none);

            var eLabelRect = new Rect(leftRect.xMax + 4, indentedRect.y, 16, indentedRect.height);
            GUI.Label(eLabelRect, "E");

            var rightRect = new Rect(eLabelRect.xMax, indentedRect.y, indentedRect.width / 2f, indentedRect.height);
            EditorGUI.PropertyField(rightRect, exponentProperty, GUIContent.none);
        }

        private static string BuildString(int digits, ushort exponent)
        {
            string digitStr = digits.ToString();
            int totalLength = exponent + 1;

            if (digitStr.Length >= totalLength)
                return digitStr.Substring(0, totalLength);

            return digitStr + new string('0', totalLength - digitStr.Length);
        }
    }
}