using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//base class for enemies
public abstract class Enemy
{
    public int level { get { return GameController.instance.level; } }
    abstract public int health { get; }
    abstract public int attack { get; }
    abstract public int contactDamage { get; }
    abstract public int reduction { get; }
    abstract public float attackSpeed { get; }
    abstract public float projSpeed { get; }
    abstract public float movSpeed { get; }

    int baseHealth;
    int baseAttack;
    int baseContactDamage;
    int baseReduction;
    float baseAttackSpeed;
    float baseProjSpeed;
    float baseMovSpeed;
}

//class for enemy type shooter
public class EnemyShooter : Enemy
{
    //return attributes according to level
    override public int health { get { return baseHealth + Mathf.RoundToInt(Mathf.Pow(level,1.2f) * 3); } }
    override public int attack { get { return baseAttack + Mathf.RoundToInt(Mathf.Pow(level, 1.2f) * 2); } }
    override public int contactDamage { get { return baseContactDamage + level; } }
    override public float attackSpeed { get { return baseAttackSpeed + 0.1f * Mathf.Log(level, 2); } }
    override public float projSpeed { get { return baseProjSpeed + 0.1f * Mathf.Log(level, 2); } }
    override public float movSpeed { get { return baseMovSpeed + 0.1f * Mathf.Log(level, 1.5f); } }
    override public int reduction { get { return baseReduction + level; } }

    //base attributes
    int baseHealth = 10;
    int baseAttack = 5;
    int baseContactDamage = 1;
    float baseAttackSpeed = 0.5f;
    float baseProjSpeed = 3;
    float baseMovSpeed = 1;
    int baseReduction = 0;
}

//class for enemy type rammer
public class EnemyRammer : Enemy
{
    //return attributes according to level
    override public int health { get { return baseHealth + Mathf.RoundToInt(Mathf.Pow(level, 1.2f) * 5); } }
    override public int attack { get { return baseAttack; } }
    override public int contactDamage { get { return baseContactDamage + Mathf.RoundToInt(Mathf.Pow(level, 1.2f) * 3); } }
    override public float attackSpeed { get { return baseAttackSpeed; } }
    override public float projSpeed { get { return baseProjSpeed; } }
    override public float movSpeed { get { return baseMovSpeed + 0.1f * Mathf.Log(level, 1.4f); } }
    override public int reduction { get { return baseReduction + Mathf.RoundToInt(Mathf.Pow(level, 1.2f) * 1.5f); } }

    //base attributes
    int baseHealth = 20;
    int baseAttack = 0;
    int baseContactDamage = 10;
    float baseAttackSpeed = -1;
    float baseProjSpeed = 3;
    float baseMovSpeed = 2;
    int baseReduction = 0;
}

