using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.U2D.Sprites;
using UnityEngine;

[CustomPropertyDrawer(typeof(BuildingSize))]
public class BuildingSizeDrawer : PropertyDrawer
{
    int propertyCount;
    float propertyHeight = 20f;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        propertyCount = 0;
        EditorGUI.indentLevel = 0;
        Rect contentPosition = position;
        contentPosition.height = 18;

        EditorGUI.LabelField(contentPosition, label);
        contentPosition.y += propertyHeight;
        propertyCount++;

        
        EditorGUI.indentLevel++;

        contentPosition = EditorGUI.IndentedRect(contentPosition);

        SerializedProperty isShow = property.FindPropertyRelative("isShow");

        EditorGUI.BeginProperty(contentPosition, new GUIContent(isShow.displayName) , property);
        {
            EditorGUI.BeginChangeCheck();
            bool value = EditorGUI.Toggle(contentPosition, new GUIContent("IsShow") ,isShow.boolValue);
            if(EditorGUI.EndChangeCheck())
            {
                isShow.boolValue = value;
            }
        }
        contentPosition.y += propertyHeight;
        propertyCount++;

        SerializedProperty width = property.FindPropertyRelative("width");
        SerializedProperty height = property.FindPropertyRelative("height");

        Rect whRect = contentPosition;

        whRect.width = contentPosition.width/2;

        width.intValue = EditorGUI.IntField(whRect,new GUIContent("Width") ,width.intValue);

        whRect.x += contentPosition.width / 2;
        height.intValue = EditorGUI.IntField(whRect,new GUIContent("Height"), height.intValue);

        contentPosition.y += propertyHeight;
        propertyCount++;

        SerializedProperty pivotX = property.FindPropertyRelative("pivotX");
        SerializedProperty pivotY = property.FindPropertyRelative("pivotY");

        Rect pivotRect = contentPosition;

        pivotRect.width = contentPosition.width / 2;

        pivotX.intValue = EditorGUI.IntField(pivotRect, new GUIContent("PivotX"), pivotX.intValue);

        pivotRect.x += contentPosition.width / 2;
        pivotY.intValue = EditorGUI.IntField(pivotRect, new GUIContent("PivotY"), pivotY.intValue);

        contentPosition.y += propertyHeight;
        propertyCount++;

        float buttonWidth = contentPosition.width / width.intValue;

        SerializedProperty isPlace = property.FindPropertyRelative("isPlace");
        isPlace.arraySize = width.intValue * height.intValue;

        Rect isPlaceRect = contentPosition;

        isPlaceRect.y += propertyHeight * (height.intValue);
        isPlaceRect.width = buttonWidth;
        propertyCount+= height.intValue;
        for (int h = 0; h < height.intValue; h++)
        {
            isPlaceRect.x = contentPosition.x;
            isPlaceRect.y -= propertyHeight;
            for(int w = 0; w < width.intValue; w++)
            {
                {
                    SerializedProperty value = isPlace.GetArrayElementAtIndex(w + h * width.intValue);
                    if (GUI.Button(isPlaceRect, new GUIContent(value.boolValue ? "O" : "")))
                    {
                        value.boolValue = !value.boolValue;
                    }
                }
                isPlaceRect.x += isPlaceRect.width;
            }
        }


        EditorGUI.EndProperty();

    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return propertyHeight * propertyCount;
    }
}
