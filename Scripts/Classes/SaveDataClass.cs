using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this class contains the save data
[System.Serializable]
public class SaveData
{
    //global save data
    public int health, attack, attackSpeed, projSpeed, movSpeed, reductionFlat, coins;

    //save data for a play session
    public bool inGame;
    public int level, seed, currentHealth, maxHealth, upgradePoint, rerollPoint, destroyCount;
    public int damageDealt, damageTaken, damageHealed;
    public List<Augment> augments;
}
