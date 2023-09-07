using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

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

    public static void GetHItsByPhysics(Transform transform, Attack attack, int layerMask , out RaycastHit2D[] hits)
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
}
