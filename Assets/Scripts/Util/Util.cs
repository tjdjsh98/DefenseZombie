using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        Debug.Log(hits.Length);

        foreach(var hit in hits)
        {
            result = hit.collider.gameObject.GetComponent<T>();

            if (result != null)
                break;
        }

        return result;
    }

}
