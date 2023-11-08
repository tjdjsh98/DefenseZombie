using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class DataManager : MonoBehaviour
{
    Dictionary<CharacterName, Character> _characterDictionary = new Dictionary<CharacterName, Character>();
    Dictionary<BuildingName, Building> _buildingDictionary = new Dictionary<BuildingName, Building>();
    Dictionary<WeaponName, WeaponData> _weaponDataDictionary = new Dictionary<WeaponName, WeaponData>();
    Dictionary<ItemName, ItemData> _itemDataDictionary = new Dictionary<ItemName, ItemData>();
    Dictionary<string, GameObject> _etcDictionary = new Dictionary<string, GameObject>();
    Dictionary<EffectName, Effect> _effectDictionary = new Dictionary<EffectName, Effect>();
    Dictionary<ProjectileName, Projectile> _projectileDictionary = new Dictionary<ProjectileName, Projectile>();
    Dictionary<EquipmentName,Dictionary<CharacterParts, EquipmentData>> _equipmentDictionary = new Dictionary<EquipmentName, Dictionary<CharacterParts, EquipmentData>>();
    public void Init()
    {
        LoadEquipmentData();
        LoadCharacter();
        LoadBuilding();
        LoadEffect();
        LoadProjectile();
        LoadWeaponData();
        LoadItemData();
        LoadEtc();
    }

    void LoadProjectile()
    {
        Projectile[] projectiles = Resources.LoadAll<Projectile>("Prefabs/Projectile");
        foreach(var projectile in projectiles)
        {
            _projectileDictionary.Add(projectile.ProjectileName, projectile);
        }
    }

    public Projectile GetProjectile(ProjectileName name)
    {
        Projectile projectile =null;
        if (_projectileDictionary.TryGetValue(name, out projectile))
        {
            return projectile;
        }

        return projectile;
    }

    void LoadEquipmentData()
    {
        EquipmentData[] equipmentDatas = Resources.LoadAll<EquipmentData>("Datas/EquipmentData");

        foreach(var data in equipmentDatas)
        {
            if (!_equipmentDictionary.ContainsKey(data.EquipmentName))
                _equipmentDictionary.Add(data.EquipmentName, new Dictionary<CharacterParts, EquipmentData>());
            _equipmentDictionary[data.EquipmentName].Add(data.CharacterParts, data);
        }
    }

    public EquipmentData GetEquipmentData(EquipmentName equipmentName, CharacterParts characterPart)
    {
        EquipmentData data = null;
        if(_equipmentDictionary.ContainsKey(equipmentName))
        {
            if (_equipmentDictionary[equipmentName].TryGetValue(characterPart, out data))
            {
                return data;
            }
        }

        return data;
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
    void LoadItemData()
    {
        ItemData[] itemDatas = Resources.LoadAll<ItemData>("Datas/ItemData");

        foreach (ItemData data in itemDatas)
        {
            _itemDataDictionary.Add(data.ItemName, data);

        }
    }
    void LoadEtc()
    {
        GameObject[] etcs = Resources.LoadAll<GameObject>("Prefabs");

        foreach (GameObject etc in etcs)
        {
            _etcDictionary.Add(etc.name, etc);
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
        Effect[] effects = Resources.LoadAll<Effect>("Prefabs/Effect");

        foreach (Effect effect in effects)
        {
            _effectDictionary.Add(effect.EffectName, effect);
        }
    }

    public GameObject GetEffect(EffectName name)
    {
        Effect effect = null;

        if(_effectDictionary.TryGetValue(name, out effect))
        {
            return effect.gameObject;
        }

        return null;
    }

    public WeaponData GetWeaponData(WeaponName weaponName)
    {
        WeaponData weaponData = null;
        _weaponDataDictionary.TryGetValue(weaponName, out weaponData);

        return weaponData;
    }
    
    public ItemData GetItemData(ItemName itemName)
    {
        ItemData itemData = null;
        _itemDataDictionary.TryGetValue(itemName, out itemData);

        return itemData;
    }

    public GameObject GetEtc(string name) 
    { 
        GameObject find = null;
        if (string.IsNullOrEmpty(name)) return null;

        _etcDictionary.TryGetValue(name, out find);

        return find;
    }
}