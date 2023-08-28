using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    Dictionary<string, Character> _characterDictionary = new Dictionary<string, Character>();
    public void Init()
    {
        LoadCharacter();
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