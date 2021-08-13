using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(BubbleMenu))]
public class BubbleMenuOptionDrawer : Editor
{
    private SerializedProperty item;
    private ReorderableList itemList;
    private float SPACE = 5;
    private float L_H = EditorGUIUtility.singleLineHeight;
    private void OnEnable()
    {
        item = serializedObject.FindProperty("options");
        itemList = new ReorderableList(serializedObject, item, true, true, true, true);
        itemList.drawElementCallback = DrawElements;
        itemList.drawHeaderCallback = DrawHeader;
        itemList.elementHeightCallback = DrawElementHeight;
    }

    private void DrawElements(Rect rect, int index, bool active, bool focused)
    {
        SerializedProperty element = itemList.serializedProperty.GetArrayElementAtIndex(index);
        // option text
        EditorGUI.LabelField(new Rect(rect.x,rect.y,50,L_H),$"Option {index}:");
        EditorGUI.PropertyField(new Rect(rect.x, rect.y+L_H+SPACE, rect.width, L_H),element.FindPropertyRelative("itemData"),GUIContent.none);
        
        // option menu item
        EditorGUI.PropertyField(new Rect(rect.x, rect.y+L_H*2+SPACE*2, rect.width, L_H*2),element.FindPropertyRelative("itemEvent"),GUIContent.none);
        
    }

    private float DrawElementHeight(int index)
    {
        SerializedProperty element = itemList.serializedProperty.GetArrayElementAtIndex(index);
        float h = EditorGUI.GetPropertyHeight(element) + 6;
        h += EditorGUI.GetPropertyHeight(element.FindPropertyRelative("itemEvent"));
        h += (L_H + SPACE);
        return h;
    }

    private void DrawHeader(Rect rect)
    {
        string name = "Bubble Options";
        EditorGUI.LabelField(rect,name);
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();
        itemList.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
    }
}
