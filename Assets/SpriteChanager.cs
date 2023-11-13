using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;
using static Define;

public class SpriteChanager : MonoBehaviour, ICharacterOption
{
    [SerializeField] CharacterSetUpParts _defaultSetUpParts;
    Dictionary<CharacterParts, SpriteLibrary> _spriteLibararyDictionary = new Dictionary<CharacterParts, SpriteLibrary>();

    public bool IsDone { get; set; }


    public void Init()
    {
        Transform[] transformList = GetComponentsInChildren<Transform>();
        foreach (Transform t in transformList)
        {
            foreach (CharacterParts part in Enum.GetValues(typeof(CharacterParts)))
            {
                if (part.ToString().Equals(t.name))
                {
                    _spriteLibararyDictionary.Add(part, t.GetComponent<SpriteLibrary>());
                    break;
                }
            }
        }

        _spriteLibararyDictionary[CharacterParts.Head].spriteLibraryAsset = _defaultSetUpParts._headSpriteLibraryAsset;
        _spriteLibararyDictionary[CharacterParts.Eyes].spriteLibraryAsset = _defaultSetUpParts._eyesSpriteLibraryAsset;
        _spriteLibararyDictionary[CharacterParts.Body].spriteLibraryAsset = _defaultSetUpParts._bodySpriteLibraryAsset;
        _spriteLibararyDictionary[CharacterParts.Legs].spriteLibraryAsset = _defaultSetUpParts._legsSpriteLibraryAsset;
        _spriteLibararyDictionary[CharacterParts.FrontHand].spriteLibraryAsset = _defaultSetUpParts._frontHandSpriteLibraryAsset;
        _spriteLibararyDictionary[CharacterParts.BehindHand].spriteLibraryAsset = _defaultSetUpParts._behindHandSpriteLibraryAsset;
        _spriteLibararyDictionary[CharacterParts.FrontWeapon].spriteLibraryAsset = _defaultSetUpParts._frontWeaponSpriteLibraryAsset;
        _spriteLibararyDictionary[CharacterParts.BehindWeapon].spriteLibraryAsset = _defaultSetUpParts._behindWeaponSpriteLibraryAsset;
        _spriteLibararyDictionary[CharacterParts.Hat].spriteLibraryAsset = _defaultSetUpParts._hatSpriteLibraryAsset;
    }

    public bool ChangeSprite(CharacterParts part, SpriteLibraryAsset asset)
    {
        if (_spriteLibararyDictionary.ContainsKey(part))
        {
            _spriteLibararyDictionary[part].spriteLibraryAsset = asset;
            return true;
        }

        return false;
    }
    public void ChangeDefaultSprite(CharacterParts part)
    {

        if (_spriteLibararyDictionary.ContainsKey(part))
        {
            switch (part)
            {
                case CharacterParts.Hat:
                    _spriteLibararyDictionary[CharacterParts.Hat].spriteLibraryAsset = _defaultSetUpParts._hatSpriteLibraryAsset;
                    break;
                case CharacterParts.Head:
                    _spriteLibararyDictionary[CharacterParts.Head].spriteLibraryAsset = _defaultSetUpParts._headSpriteLibraryAsset;
                    break;
                case CharacterParts.Eyes:
                    _spriteLibararyDictionary[CharacterParts.Eyes].spriteLibraryAsset = _defaultSetUpParts._eyesSpriteLibraryAsset;
                    break;
                case CharacterParts.Body:
                    _spriteLibararyDictionary[CharacterParts.Body].spriteLibraryAsset = _defaultSetUpParts._bodySpriteLibraryAsset;
                    break;
                case CharacterParts.Legs:
                    _spriteLibararyDictionary[CharacterParts.Legs].spriteLibraryAsset = _defaultSetUpParts._legsSpriteLibraryAsset;
                    break;
                case CharacterParts.FrontHand:
                    _spriteLibararyDictionary[CharacterParts.FrontHand].spriteLibraryAsset = _defaultSetUpParts._frontHandSpriteLibraryAsset;
                    break;
                case CharacterParts.BehindHand:
                    _spriteLibararyDictionary[CharacterParts.BehindHand].spriteLibraryAsset = _defaultSetUpParts._behindHandSpriteLibraryAsset;
                    break;
                case CharacterParts.FrontWeapon:
                    _spriteLibararyDictionary[CharacterParts.FrontWeapon].spriteLibraryAsset = _defaultSetUpParts._frontWeaponSpriteLibraryAsset;
                    break;
                case CharacterParts.BehindWeapon:
                    _spriteLibararyDictionary[CharacterParts.BehindWeapon].spriteLibraryAsset = _defaultSetUpParts._behindWeaponSpriteLibraryAsset;
                    break;
                default:
                    break;
            }

        }
    }

    public void DataSerialize()
    {
        
    }

    public void DataDeserialize()
    {
        
    }
}


[System.Serializable]
class CharacterSetUpParts
{
    public SpriteLibraryAsset _eyesSpriteLibraryAsset;
    public SpriteLibraryAsset _headSpriteLibraryAsset;
    public SpriteLibraryAsset _bodySpriteLibraryAsset;
    public SpriteLibraryAsset _legsSpriteLibraryAsset;
    public SpriteLibraryAsset _frontHandSpriteLibraryAsset;
    public SpriteLibraryAsset _behindHandSpriteLibraryAsset;
    public SpriteLibraryAsset _frontWeaponSpriteLibraryAsset;
    public SpriteLibraryAsset _behindWeaponSpriteLibraryAsset;
    public SpriteLibraryAsset _hatSpriteLibraryAsset;


}