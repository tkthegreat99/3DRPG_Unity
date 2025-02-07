using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats : MonoBehaviour
{
    [SerializeField]
    protected int _level;
    [SerializeField]
    protected int _hp;
    [SerializeField]
    protected int _maxHp;
    [SerializeField]
    protected int _attack;
    [SerializeField]
    protected int _defense;
    [SerializeField]
    protected float _movespeed;

    public int Level { get { return _level; } set { _level = value; } }
    public int HP { get { return _hp; } set { _hp = value; } }
    public int MaxHp { get { return _maxHp; } set { _maxHp = value; } }
    public int Attack { get { return _attack; } set { _attack = value; } }
    public int Defense { get { return _defense; } set { _defense = value; } }
    public float MoveSpeed { get { return _movespeed; } set { _movespeed = value; } }

    private void Start()
    {
        _level = 1;
        _hp = 100;
        _maxHp = 100;
        _attack = 10;
        _defense = 10;
        _movespeed = 5.0f;
    }

    public virtual void OnAttacked(Stats attacker)
    {
        int damage = Mathf.Max(0, attacker.Attack - Defense);
        HP -= damage;
        if(HP <= 0)
        {
            HP = 0;
            OnDead();
        }
    }

    protected virtual void OnDead()
    {
        Managers.Game.Despawn(gameObject);
    }

}
