using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public abstract class UIBase : MonoBehaviour
    {
        public virtual void Initialize()
        {
            
        }
        
        Dictionary<Type, Object[]> _objects = new Dictionary<Type, Object[]>();
        
        protected void Bind<C>(Type type) where C : Object
        {
            string[] names = Enum.GetNames(type);
            Object[] objects = new Object[names.Length];
            this._objects.Add(typeof(C), objects);

            for (int i = 0; i < names.Length; i++)
            {
                if (typeof(C) == typeof(GameObject))
                {
                    objects[i] = GameObjects.FindChild(gameObject, names[i], true);
                }
                else
                {
                    objects[i] = GameObjects.FindChild<C>(gameObject, names[i], true);    
                }

                if (objects[i] == null)
                {
                    Debug.Log($"Failed to bind: {typeof(C)}/{names[i]}");
                }
            }
        }

        protected T Get<T>(int idx) where T : Object
        {
            Object[] objects = null;
            if (!this._objects.TryGetValue(typeof(T), out objects))
            {
                return null;
            }

            return objects[idx] as T;
        }

        // public static void BindEventHandler(GameObject root, Action<PointerEventData, GameObject> action, Define.UIEvent type)
        // {
        //     UI_EventHandler eventHandler = Util.GetOrAddComponent<UI_EventHandler>(root);
        //
        //     switch (type)
        //     {
        //         case Define.UIEvent.Click:
        //             eventHandler.OnClickHandler -= action;
        //             eventHandler.OnClickHandler += action;
        //             break;
        //         case Define.UIEvent.Drag:
        //             eventHandler.OnDragHandler -= action;
        //             eventHandler.OnDragHandler += action;
        //             break;
        //     }
        //
        // }
    }