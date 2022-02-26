using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item
{
    public Item(string _name, int _cost, int _attack, int _health, int _spriteCount, float _percent, string _ability)
    {
        name = _name;
        cost = _cost;
        attack = _attack;
        health = _health;
        spriteCount = _spriteCount;
        percent = _percent;
        ability = _ability;
    }

    public string name;
    public int cost;
    public int attack;
    public int health;
    public int spriteCount;
    public float percent;
    public string ability;
}

[System.Serializable]
public class Serialization<T>
{
    public Serialization(List<T> _target) => target = _target;
    public List<T> target;
}

[CreateAssetMenu(fileName ="ItemSo", menuName ="Scriptable Object/ItemSO")]
public class ItemSO : ScriptableObject
{
    public Item[] items;
}
