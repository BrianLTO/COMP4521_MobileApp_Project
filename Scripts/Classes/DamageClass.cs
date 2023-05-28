using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this class is the container for damage
//contains the raw damage as well as the augments that may be applied
public class Damage
{
    public int damage { get; set; }
    public List<ApplyAugment> augments { get; private set; }

    public Damage(int damage, List<ApplyAugment> augments)
    {
        this.damage = damage;
        this.augments = augments;
    }

    public Damage(int damage)
    {
        this.damage = damage;
    }

    public Damage(Damage damage)
    {
        this.damage = damage.damage;
        this.augments = damage.augments;
    }

}
