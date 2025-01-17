using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStat : Stats
{
    [SerializeField]
    int _exp;
    [SerializeField]
    int _gold;

    public int Exp { get { return _exp; } set { _exp = value; } }
    public int Gold { get { return _gold; } set { _gold = value; } }


    private void Start()
    {
        _level = 1;
        _hp = 100;
        _maxHp = 100;
        _attack = 15;
        _defense = 10;
        _movespeed = 5.0f;
        _exp = 0;
        _gold = 0;
    }
}
