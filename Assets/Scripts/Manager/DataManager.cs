using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    Dictionary<string, Character> _characterDictionary = new Dictionary<string, Character>();
    Dictionary<string, Building> _buildingDictionary = new Dictionary<string, Building>();
    public void Init()
    {
        LoadCharacter();
        LoadBuilding();
    }

    void LoadBuilding()
    {
        Building[] buildings = Resources.LoadAll<Building>("Prefabs/Building");

        foreach (Building building in buildings)
        {
            _buildingDictionary.Add(building.name, building);
        }
    }
    public Building GetBuilding(string name)
    {
        Building building = null;
        _buildingDictionary.TryGetValue(name, out building);
        return building;
    }
    void LoadCharacter()
    {
        Character[] characters = Resources.LoadAll<Character>("Prefabs/Character");

        foreach (Character character in characters)
        {
            _characterDictionary.Add(character.name, character);
        }
    }

    public Character GetCharacter(string name)
    {
        Character character = null;
        _characterDictionary.TryGetValue(name, out character);
        return character;
    }
}