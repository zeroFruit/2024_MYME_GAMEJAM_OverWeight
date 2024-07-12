using System.Reflection;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(InspectorButtonAttribute))]
public class InspectorButtonPropertyDrawer : PropertyDrawer
{
    MethodInfo _eventMethodInfo = null;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        InspectorButtonAttribute inspectorButtonAttribute = (InspectorButtonAttribute)attribute;
            
        float buttonLength = position.width;
        Rect buttonRect = new Rect(position.x, position.y, buttonLength, position.height);
        GUI.skin.button.alignment = TextAnchor.MiddleLeft;
            
        if (GUI.Button(buttonRect, inspectorButtonAttribute.MethodName))
        {
            System.Type eventOwnerType = property.serializedObject.targetObject.GetType();
            string eventName = inspectorButtonAttribute.MethodName;

            if (_eventMethodInfo == null)
            {
                _eventMethodInfo = eventOwnerType.GetMethod(eventName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            }

            if (_eventMethodInfo != null)
            {
                _eventMethodInfo.Invoke(property.serializedObject.targetObject, null);
            }
            else
            {
                Debug.LogWarning(string.Format("InspectorButton: Unable to find method {0} in {1}", eventName, eventOwnerType));
            }
        }
    }
}
#endif