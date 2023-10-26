using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;

public static class Util
{
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

    public static T GetGameObjectByPhysics<T>(Vector3 position, Range range, int layerMask = -1) where T : MonoBehaviour
    {
        RaycastHit2D[] hits;
        if (layerMask != -1)
            hits = Physics2D.BoxCastAll(position + range.center, range.size, 0, Vector2.zero, 0, layerMask);
        else
            hits = Physics2D.BoxCastAll (position + range.center, range.size, 0, Vector2.zero);

        T result = null;


        foreach(var hit in hits)
        {
            result = hit.collider.gameObject.GetComponent<T>();

            if (result != null)
                break;
        }

        return result;
    }
    public static List<T> GetGameObjectsByPhysics<T>(Vector3 position, Range range, int layerMask = -1) where T : MonoBehaviour
    {
        List<T> result = new List<T>();
        RaycastHit2D[] hits;
        if (layerMask != -1)
            hits = Physics2D.BoxCastAll(position + range.center, range.size, 0, Vector2.zero, 0, layerMask);
        else
            hits = Physics2D.BoxCastAll(position + range.center, range.size, 0, Vector2.zero);

        foreach (var hit in hits)
        {
            T temp = hit.collider.gameObject.GetComponent<T>();

            if(temp != null)
            {
                result.Add(temp);
            } 
                
        }

        return result;
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