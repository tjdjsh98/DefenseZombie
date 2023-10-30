using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class Extension
{
    public static Transform FindOrAdd(this GameObject gameObject, string name)
    {
        Transform tr = gameObject.transform.Find(name);
        if (tr == null)
        {
            GameObject temp = new GameObject(name);
            temp.transform.parent = gameObject.transform;
            tr = temp.transform;
        }

        return tr;
    }

    public static T GetOrAddComponent<T>(this Transform transform) where T : Component
    {
        T componet = transform.GetComponent<T>();
        if (componet == null)
            componet = transform.AddComponent<T>();

        return componet;
    }
    public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
    {
        T componet = gameObject.GetComponent<T>();
        if (componet == null)
            componet = gameObject.AddComponent<T>();

        return componet;
    }

    public static T GetRandom<T>(this List<T> list)
    {
        if (list.Count <= 0) return default(T);

        T result = list[Random.Range(0, list.Count)];

        return result;
    }

    public static void Shuffle<T>(this List<T> list)
    {
        for(int i =0; i < list.Count; i++)
        {
            int random = Random.Range(0,list.Count);
            T temp = list[i];
            list[i] = list[random];
            list[random] = temp;
        }
    }
}
