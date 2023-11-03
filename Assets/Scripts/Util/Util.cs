using System;
using System.Buffers.Text;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Unity.VisualScripting;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;

public static class Util
{
    static MemoryStream memoryStream = new MemoryStream();
    static int offset = 0;
    static byte[] buffer = new byte[1024];
    public static GameObject GetGameObjectByPhysics(Vector3 position, Range range, int layerMask = -1)
    {
        RaycastHit2D hit2D;
        if (layerMask != -1) 
            hit2D = Physics2D.BoxCast(position + range.center, range.size,0,Vector2.zero,0,layerMask);
        else
            hit2D = Physics2D.BoxCast(position + range.center, range.size,0,Vector2.zero);

        if (hit2D.collider != null)
            return hit2D.collider.gameObject;

        return null;
    }
    public static T GetGameObjectByPhysics<T>(Vector3 position, Range range, int layerMask = -1) 
    {
        RaycastHit2D[] hits;
        if (layerMask != -1)
            hits = Physics2D.BoxCastAll(position + range.center, range.size, 0, Vector2.zero, 0, layerMask);
        else
            hits = Physics2D.BoxCastAll (position + range.center, range.size, 0, Vector2.zero);

        T result = default(T);


        foreach(var hit in hits)
        {
            result = hit.collider.gameObject.GetComponent<T>();

            if (result != null)
                break;
        }

        return result;
    }
    public static List<T> GetGameObjectsByPhysics<T>(Vector3 position, Range range, int layerMask = -1) 
    {
        List<T> result = new List<T>();
        RaycastHit2D[] hits;
        if (layerMask != -1)
            hits = Physics2D.BoxCastAll(position + range.center, range.size, 0, Vector2.zero, 0, layerMask);
        else
            hits = Physics2D.BoxCastAll(position + range.center, range.size, 0, Vector2.zero);

        foreach (var hit in hits)
        {
            T[] temps = hit.collider.gameObject.GetComponents<T>();

            foreach(var temp in temps)
            {
                result.Add(temp);

            }
        }

        return result;
    }
    public static void GetHItsByPhysics(Transform transform, Range range, int layerMask, out RaycastHit2D[] hits)
    {
        range.center.x = (transform.localScale.x > 0 ? range.center.x : -range.center.x);
        hits = Physics2D.BoxCastAll(transform.position + range.center, range.size, 0, Vector2.zero, 0, layerMask);
    }
    public static void GetHItsByPhysics(Transform transform, AttackData attack, int layerMask , out RaycastHit2D[] hits)
    {
        Range attackRange = attack.attackRange;
        
        attackRange.center.x = (transform.localScale.x > 0 ? attackRange.center.x : -attackRange.center.x);

        switch (attack.attacKShape)
        {
            case Define.AttacKShape.Rectagle:
                hits = Physics2D.BoxCastAll(transform.position + attackRange.center, attackRange.size, 0, Vector2.zero, 0, layerMask);
                break;
            case Define.AttacKShape.Raycast:
                hits = Physics2D.RaycastAll(transform.position + attackRange.center, transform.parent.localScale.x > 0 ? Vector2.right : Vector2.left, attackRange.size.x, layerMask);
                break;
            default:
                hits = Physics2D.BoxCastAll(transform.position + attackRange.center, attackRange.size, 0, Vector2.zero, 0, layerMask);
                break;
        }
    }
    public static bool GetIsInRange(GameObject me, GameObject you, Range range)
    {
        if(you.transform.position.x < me.transform.position.x + range.center.x + range.size.x/2)
        {
            if (you.transform.position.x > me.transform.position.x + range.center.x - range.size.x / 2)
            {
                if (you.transform.position.y < me.transform.position.y + range.center.y + range.size.y / 2)
                {
                    if (you.transform.position.y > me.transform.position.y + range.center.y - range.size.y / 2)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }
    public static GameObject Raycast(Vector3 position,string tag , int layerMask = -1)
    {
        RaycastHit2D[] hits;

        if(layerMask != -1)
            hits = Physics2D.RaycastAll(position, Vector2.zero, 0, layerMask);
        else
            hits = Physics2D.RaycastAll(position, Vector2.zero, 0);

        foreach (var hit in hits)
        {
            if(hit.collider.gameObject.tag == tag)
            {
                return hit.collider.gameObject;
            }
        }

        return null;
    }
    public static List<GameObject> Raycast(Vector3 position,Vector3 direction, float distance, int layerMask = -1 , string tag = null)
    {
        RaycastHit2D[] hits;

        if (layerMask != -1)
            hits = Physics2D.RaycastAll(position, direction, distance, layerMask);
        else
            hits = Physics2D.RaycastAll(position, direction,distance, 0);

        List<GameObject> result = new List<GameObject>();
        foreach (var hit in hits)
        {
            if(tag == null)
            {
                result.Add(hit.collider.gameObject);
            }
            else if (hit.collider.gameObject.tag == tag)
            {
                result.Add(hit.collider.gameObject);
            }
        }

        return result;
    }
    public static void DrawRangeGizmo(GameObject target, Range range, Color color)
    {
        Gizmos.color = color;

        Gizmos.DrawWireCube(target.transform.position + range.center, range.size);
    }


    public static void StartWriteSerializedData()
    {
        memoryStream.Position = 0;
        offset = 0;
    }

    public static void WriteSerializedData(int data)
    {
        byte[] buffer = BitConverter.GetBytes(data);
        memoryStream.Write(buffer, 0, sizeof(int));
    }
    public static void WriteSerializedData(float data)
    {
        byte[] buffer = BitConverter.GetBytes(data);
        memoryStream.Write(buffer, 0, sizeof(float));
        Debug.Log(buffer[0]);
        Debug.Log(buffer[1]);
        Debug.Log(buffer[2]);
        Debug.Log(buffer[3]);
    }
    public static void WriteSerializedData(bool data)
    {
        byte[] buffer = BitConverter.GetBytes(data);
        memoryStream.Write(buffer, 0, sizeof(bool));
    }
    public static string EndWriteSerializeData()
    {
        byte[] data = memoryStream.GetBuffer();
        return Convert.ToBase64String(data);
    }

    public static void StartReadSerializedData(string stringData)
    {
        offset = 0;
        buffer = Convert.FromBase64String(stringData);
    }

    public static int ReadSerializedDataToInt()
    {
        ReadOnlySpan<byte> bytes = new ReadOnlySpan<byte>(buffer, offset, sizeof(int));
        int result = BitConverter.ToInt32(bytes);
        offset += sizeof(int);
        return result;
    }
    public static float ReadSerializedDataToFloat()
    {
        
        ReadOnlySpan<byte> bytes = new ReadOnlySpan<byte>(buffer, offset, sizeof(float));
        float result = BitConverter.ToSingle(bytes);
        offset += sizeof(float);
        return result;
    }
    public static bool ReadSerializedDataToBoolean()
    {
        ReadOnlySpan<byte> bytes = new ReadOnlySpan<byte>(buffer, offset, sizeof(bool));
        bool result = BitConverter.ToBoolean(bytes);
        offset += sizeof(bool);

        return result;
    }
}

[System.Serializable]
class SerializeDictionary<T1,T2>
{
    [SerializeField]List<SerializeDictionaryData<T1,T2>> _datas;
    Dictionary<T1,T2> _dictionary = new Dictionary<T1, T2>();
    int _preCount = 0;
    public Dictionary<T1,T2> GetDictionary()
    {
        if(_dictionary.Count != _preCount)
        {
            _dictionary.Clear();
            foreach (var data in _datas)
            {
                _dictionary.Add(data.key, data.value);
            }
        }

        return _dictionary;
    }
}

[System.Serializable]
class SerializeDictionaryData<T1,T2>
{
    public T1 key;
    public T2 value;
}