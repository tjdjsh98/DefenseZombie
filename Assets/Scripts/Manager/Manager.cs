using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    static Manager _manager;

    static string _managerName = "Manager";


    static DataManager _data;
    public static DataManager Data { get { Init(); return _data ; } }

    static CharacterManager _character;
    public static CharacterManager Character { get { Init(); return _character; } }

    static BuildingManager _building;
    public static BuildingManager Building { get { Init(); return _building; } }

    static ItemManager _item;
    public static ItemManager Item { get { Init(); return _item; } }

    static UIManager _ui;
    public static UIManager UI { get { Init(); return _ui; } }

    static GameManager _game;
    public static GameManager Game { get { Init(); return _game; } }

    private void Awake()
    {
        Init();
    }

    static void Init()
    {
        if (_manager != null) return;

        _manager = GameObject.Find(_managerName).GetOrAddComponent<Manager>();
        _data = GameObject.Find(_managerName).GetOrAddComponent<DataManager>();
        _character = GameObject.Find(_managerName).GetOrAddComponent<CharacterManager>();
        _building = GameObject.Find(_managerName).GetOrAddComponent<BuildingManager>();
        _item = GameObject.Find(_managerName).GetOrAddComponent<ItemManager>();
        _ui = GameObject.Find(_managerName).GetOrAddComponent<UIManager>();
        _game = GameObject.Find(_managerName).GetOrAddComponent<GameManager>();

        _data.Init();
        _character.Init();
        _building.Init();
        _item.Init();
        _ui.Init();
        _game.Init();

        Application.runInBackground = true;
    }
}
