using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class GUIScripts
{
    public static void GuiLine( int i_height = 1 )
    {

        Rect rect = EditorGUILayout.GetControlRect(false, i_height );

        rect.height = i_height;

        EditorGUI.DrawRect(rect, new Color ( 0.5f,0.5f,0.5f, 1 ) );
    }

    public static string TextField(string text, string placeholder) {
    return TextInput(text, placeholder);
    }

    public static string TextArea(string text, string placeholder) {
        return TextInput(text, placeholder, area: true);
    }

    private static string TextInput(string text, string placeholder, bool area = false) {
        var newText = area ? EditorGUILayout.TextArea(text) : EditorGUILayout.TextField(text);
        if (System.String.IsNullOrEmpty(text.Trim())) {
            const int textMargin = 2;
            var guiColor = GUI.color;
            GUI.color = Color.grey;
            var textRect = GUILayoutUtility.GetLastRect();
            var position = new Rect(textRect.x + textMargin, textRect.y, textRect.width, textRect.height);
            EditorGUI.LabelField(position, placeholder);
            GUI.color = guiColor;
        }
        return newText;
    }
}
