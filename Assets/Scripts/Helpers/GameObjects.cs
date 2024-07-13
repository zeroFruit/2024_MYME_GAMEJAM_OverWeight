using UnityEngine;

public static class GameObjects
{
    public static T GetOrAddComponent<T>(GameObject root) where T : Component
    {
        T component = root.GetComponent<T>();
        if (component == null)
        {
            component = root.AddComponent<T>();
        }

        return component;
    }
    
    public static T FindChild<T>(GameObject root, string name = null, bool recursive = false) where T : Object
    {
        if (root == null)
        {
            return null;
        }

        if (!recursive)
        {
            for (int i = 0; i < root.transform.childCount; i++)
            {
                Transform transform = root.transform.GetChild(i);
                if (!string.IsNullOrEmpty(name) && !transform.name.Equals(name))
                {
                    continue;
                }
                
                T component = transform.GetComponent<T>();
                if (component != null)
                {
                    return component;
                }
            }
        }

        foreach (T component in root.GetComponentsInChildren<T>(true))
        {
            if (!string.IsNullOrEmpty(name) && !component.name.Equals(name))
            {
                continue;
            }
            return component;
        }

        return null;
    }

    public static GameObject FindChild(GameObject root, string name = null, bool recursive = false)
    {
        Transform transform = FindChild<Transform>(root, name, recursive);
        if (transform == null)
        {
            return null;
        }

        return transform.gameObject;
    }
}