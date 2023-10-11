using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_Commander : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _textMeshPro;

    Rect startSize;

    // �簢�� ���� ǥ�� ����
    bool _showSelection;
    Vector3 _mouseCur;
    Vector3 _beginCur;
    Vector2 _posMin;
    Vector2 _posMax;

    // ȭ�� �̵� ����
    Vector3 _startMidleCur;
    Vector3 _cameraInitPos;


    List<HelperAI> _list = new List<HelperAI>();
    private void Awake()
    {
        gameObject.SetActive(false);
    }

    private void OnGUI()
    {
        if (!_showSelection) return;

        Rect rect = new Rect();
        rect.min = _posMin;
        rect.max = _posMax;

        GUI.Box(rect, "");
    }

    private void Update()
    {
        Control();
    }

    public void Open()
    {
        gameObject.SetActive(true);
    }

    public void Close()
    {
        _textMeshPro.text = "";
        gameObject.SetActive(false);
    }

    public void Summon()
    {
        Manager.Character.RequestGenerateCharacter(Define.CharacterName.Helper, Vector3.zero);
    }

    void Control()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            _beginCur = Input.mousePosition;
            _beginCur.y = Screen.height - _beginCur.y;

            _showSelection = true;

            _list.Clear();
        }
        if (Input.GetMouseButton(0))
        {
            _mouseCur = Input.mousePosition;
            _mouseCur.y = Screen.height - _mouseCur.y;

            _posMax = Vector2.Max(_mouseCur, _beginCur);
            _posMin = Vector2.Min(_mouseCur, _beginCur);

            Vector3 maxPos = Camera.main.ScreenToWorldPoint(_posMax);
            Vector3 minPos = Camera.main.ScreenToWorldPoint(_posMin);

            Vector3 temp = _beginCur;
            temp.y = Screen.height - temp.y;
            Vector3 boxMinPos = Camera.main.ScreenToWorldPoint(temp);
            Vector3 boxMaxPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            RaycastHit2D[] hits = Physics2D.BoxCastAll((boxMaxPos + boxMinPos) / 2, 
                new Vector2(Mathf.Abs(boxMaxPos.x - boxMinPos.x), Mathf.Abs(boxMaxPos.y - boxMinPos.y))
                , 0, Vector2.zero, 0, 1 << LayerMask.NameToLayer("Character"));

            if(hits.Length > 0)
            {
                foreach(var hit in hits)
                {
                    HelperAI helperAI = hit.collider.gameObject.GetComponent<HelperAI>();
                    if(helperAI != null)
                    {
                        if (!_list.Contains(helperAI))
                            _list.Add(helperAI);
                    }
                }
            }
            _textMeshPro.text = _list.Count.ToString();
        }
        if (Input.GetMouseButtonUp(0))
        {
            _showSelection = false;
        }

        if(Input.GetMouseButtonDown(2))
        {
            _cameraInitPos = Camera.main.transform.position;
            _startMidleCur = Input.mousePosition;

        }
        if (Input.GetMouseButton(2))
        {
            Vector3 distance = Input.mousePosition - _startMidleCur;

            distance = Camera.main.ScreenToWorldPoint(distance);

            distance = distance-Camera.main.ScreenToWorldPoint(Vector3.zero) ;

            Camera.main.transform.position = _cameraInitPos - distance;

        }
        if (Input.GetMouseButtonUp(2))
        {

        }

        if (Input.GetMouseButtonDown(1))
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            foreach(var ai in _list)
            {
                ai.SetMainPos(mousePosition.x);
            }
        }
    }
}
