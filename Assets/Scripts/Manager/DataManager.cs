using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class DataManager : MonoBehaviour
{
    Dictionary<CharacterName, Character> _characterDictionary = new Dictionary<CharacterName, Character>();
    Dictionary<BuildingName, Building> _buildingDictionary = new Dictionary<BuildingName, Building>();
    Dictionary<string, GameObject> _effectDictionary = new Dictionary<string, GameObject>();
    Dictionary<string, ParabolaProjectile> _projectileDictionary = new Dictionary<string, ParabolaProjectile>();
    public void Init()
    {
        LoadCharacter();
        LoadBuilding();
        LoadEffect();
    }

    void LoadBuilding()
    {
        Building[] buildings = Resources.LoadAll<Building>("Prefabs/Building");

        foreach (Building building in buildings)
        {
            _buildingDictionary.Add(building.BuildingName, building);
        }
    }
    public Building GetBuilding(BuildingName name)
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
            _characterDictionary.Add(character.CharacterName, character);
        }
    }

    public Character GetCharacter(CharacterName name)
    {
        Character character = null;
        _characterDictionary.TryGetValue(name, out character);
        return character;
    }
    void LoadEffect()
    {
        GameObject[] effects = Resources.LoadAll<GameObject>("Prefabs/Effect");

        foreach (GameObject effect in effects)
        {
            _effectDictionary.Add(effect.name, effect);
        }
    }

    public GameObject GetEffect(string name)
    {
        GameObject effect = null;

        if(_effectDictionary.TryGetValue(name, out effect))
        {
            return effect;
        }

        return null;
    }
    void LoadProjectile()
    {

    }

    public ParabolaProjectile GetProjectile(string name)
    {
        ParabolaProjectile projectile = null;

        if(_projectileDictionary.TryGetValue(name, out projectile))
        {
            return projectile;
        }

        return null ;
    }
}
