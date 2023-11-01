using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_Commander : UIBase
{
    CommanderCenter _commanderCenter;

    [SerializeField] TextMeshProUGUI _textMeshPro;

    Rect startSize;

    // 사각형 영역 표시 변수
    bool _showSelection;
    Vector3 _mouseCur;
    Vector3 _beginCur;
    Vector2 _posMin;
    Vector2 _posMax;

    // 화면 이동 변수
    Vector3 _startMidleCur;
    Vector3 _cameraInitPos;


    List<HelperAI> _list = new List<HelperAI>();
    public override void Init()
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

    public void Open(CommanderCenter commanderCenter)
    {
        _commanderCenter = commanderCenter;
        gameObject.SetActive(true);
    }

    public void Close()
    {
        _textMeshPro.text = "";
        gameObject.SetActive(false);
    }

    public void Summon()
    {
        Character character = null;
        Manager.Character.GenerateCharacter(Define.CharacterName.Helper, _commanderCenter.transform.position,ref character);
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
                , 0, Vector2.zero, 0, Define.PlayerLayerMask);

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
                Debug.Log(ai);
                ai.SetMainPos(mousePosition.x);
            }
        }
    }
}
