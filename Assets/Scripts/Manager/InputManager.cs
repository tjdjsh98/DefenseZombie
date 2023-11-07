using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    GraphicRaycaster _graphicRaycaster;
    GraphicRaycaster GraphicRaycaster { set { _graphicRaycaster = value; } get { if (_graphicRaycaster == null) GetRaycaster(); return _graphicRaycaster; } }

    Camera _mainCamera;

    Vector3 _mousePosition;
    public Vector3 MousePosition { get { _mousePosition = _mainCamera.ScreenToWorldPoint(Input.mousePosition); return _mousePosition; } }

    public Action<List<GameObject>> UIMouseDownHandler;
    public Action<List<GameObject>> UIMouseDragHandler;
    public Action<List<GameObject>> UIMouseUpHandler;
    public int _uiMouseDownSubcribeCount;
    List<GameObject> _uiRaycastGameObjects = new List<GameObject>();

    public Action<List<GameObject>> MouseDown;
    public Action<List<GameObject>> MouseDraged;
    public Action<List<GameObject>> MouseUp;
    List<GameObject> _mouseRaycastGameobjects = new List<GameObject>();

    public void Init()
    {
        _mainCamera = Camera.main;

       
    }

    public void ManagerUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (MouseDown != null)
            {
                Raycast();
                MouseDown(_mouseRaycastGameobjects);
            }
            if (UIMouseDownHandler != null)
            {
                RaycastUI();
                UIMouseDownHandler.Invoke(_uiRaycastGameObjects);
            }

        }

        if (Input.GetMouseButton(0))
        {
            if (MouseDraged != null)
            {
                Raycast();
                MouseDraged.Invoke(_mouseRaycastGameobjects);
            }
            if (UIMouseDragHandler != null)
            {
                RaycastUI();
                UIMouseDragHandler.Invoke(_uiRaycastGameObjects);
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (MouseUp != null)
            {
                Raycast();
                MouseUp.Invoke(_mouseRaycastGameobjects);
            }
            if (UIMouseUpHandler != null)
            {
                RaycastUI();
                UIMouseUpHandler.Invoke(_uiRaycastGameObjects);
            }
        }
    }

    void GetRaycaster()
    {
        if (_graphicRaycaster != null || GameObject.Find("Canvas") == null) return;

        _graphicRaycaster = GameObject.Find("Canvas").GetComponent<GraphicRaycaster>();
    }
    public void RaycastUI()
    {
        var ped = new PointerEventData(null);

        _uiRaycastGameObjects.Clear();

        ped.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        GraphicRaycaster.Raycast(ped, results);

        if (results.Count <= 0) return;

        foreach (var item in results)
        {
            _uiRaycastGameObjects.Add(item.gameObject);
        }
    }

    public void Raycast()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(MousePosition, Vector2.zero, 0);

        _mouseRaycastGameobjects.Clear();

        if (hits.Length <= 0) return;
        foreach (var hit in hits)
        {
            if (hit.collider == null) continue;
            _mouseRaycastGameobjects.Add(hit.collider.gameObject);
        }
    }
}
