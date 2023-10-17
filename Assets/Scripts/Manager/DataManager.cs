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
    Dictionary<WeaponName, WeaponData> _weaponDataDictionary = new Dictionary<WeaponName, WeaponData>();
    public void Init()
    {
        LoadCharacter();
        LoadBuilding();
        LoadEffect();
        LoadWeaponData();
    }

    void LoadBuilding()
    {
        Building[] buildings = Resources.LoadAll<Building>("Prefabs/Building");

        foreach (Building building in buildings)
        {
            _buildingDictionary.Add(building.BuildingName, building);
        }
    }
    void LoadWeaponData()
    {
        WeaponData[] weaponDatas = Resources.LoadAll<WeaponData>("Datas/WeaponData");

        foreach(WeaponData data in weaponDatas)
        {
            _weaponDataDictionary.Add(data.WeaponName, data);
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

    public WeaponData GetWeaponData(WeaponName weaponName)
    {
        WeaponData weaponData = null;
        _weaponDataDictionary.TryGetValue(weaponName, out weaponData);

        return weaponData;
    }
    
}
