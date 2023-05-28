using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


//template abstract class for all augments
[System.Serializable]
public abstract class Augment
{
    //used in description color formatting
    protected string baseColor = "<color=white>";
    protected string amountColor = "<color=yellow>";
    protected string extraColor = "<color=lightblue>";

    public int rank { get; set; }
    public string augmentName { get; protected set; }
    public int rarity { get; protected set; }

    public Augment()
    {
        rank = 1;
    }

    public static PlayerController player { get { return PlayerController.instance; } }

    //virtual functions to be overidden for certain augments
    virtual public int GetHealthFlat(int parameter)
    {
        return parameter;
    }
    virtual public int GetHealthMulti(int parameter)
    {
        return parameter;
    }
    virtual public int GetReductionFlat(int parameter)
    {
        return parameter;
    }
    virtual public float GetReductionMulti(float parameter)
    {
        return parameter;
    }
    virtual public int GetAttackFlat(int parameter)
    {
        return parameter;
    }
    virtual public int GetAttackMulti(int parameter)
    {
        return parameter;
    }
    virtual public int GetFinalAttackMulti(int parameter)
    {
        return parameter;
    }
    virtual public int GetContactDamageFlat(int parameter)
    {
        return parameter;
    }
    virtual public float GetAttackSpeedFlat(float parameter)
    {
        return parameter;
    }
    virtual public float GetAttackSpeedMulti(float parameter)
    {
        return parameter;
    }
    virtual public float GetProjSpeedFlat(float parameter)
    {
        return parameter;
    }
    virtual public float GetProjSpeedMulti(float parameter)
    {
        return parameter;
    }
    virtual public float GetMovSpeedFlat(float parameter)
    {
        return parameter;
    }
    virtual public float GetMovSpeedMulti(float parameter)
    {
        return parameter;
    }
    virtual public float GetDamageMulti(float parameter)
    {
        return parameter;
    }
    virtual public float GetExplosionDamageMulti(float parameter)
    {
        return parameter;
    }
    virtual public float GetExplosionRadiusFlat(float parameter)
    {
        return parameter;
    }
    virtual public void UpdateAugment() { }

    //abstract functions for getting the description about the augment according to its rank
    abstract public string GetDescription();
    abstract public string GetDescription(int newRank);
}

//template abstract class for augments that have a applicable effect (e.g. explosion)
[System.Serializable]
public abstract class ApplyAugment : Augment
{
    //these bool determines whether the augment can apply on damage types
    public bool projectileApply = true;
    public bool explosionApply = true;
    public bool contactApply = true;

    //these overridable functions is called during certain events (e.g. collision with enemy)
    virtual public void onHitWall(Vector2 position)
    {

    }
    virtual public Damage onHitWallDamage(Damage damage)
    {
        return damage;
    }
    virtual public void onHitEnemy(EnemyController enemy)
    {

    }
    virtual public void onHitEnemyContact(EnemyController enemy)
    {

    }
    virtual public void onKill(EnemyController enemy)
    {

    }
}

//below is all the implemented augments
[System.Serializable]
public class Fortitude : Augment
{
    public Fortitude()
    {
        augmentName = "Fortitude";
        rarity = 0;
    }

    int healthFlat { get { return 20 * rank; } }

    override public int GetHealthFlat(int parameter)
    {
        if (player.augments.Any(x => x.GetType() == typeof(Defiance))) return parameter + 2 * healthFlat;
        return parameter + healthFlat;
    }

    public override string GetDescription()
    {
        return GetDescription(rank);
    }

    public override string GetDescription(int rank)
    {
        return baseColor + "Increases your hit points by " + amountColor + healthFlat
            + extraColor + " (+20)</color>";
    }
}

[System.Serializable]
public class Magnitude : Augment
{
    public Magnitude()
    {
        augmentName = "Magnitude";
        rarity = 0;
    }

    int attackFlat { get { return 2 * rank; } }

    override public int GetAttackFlat(int parameter)
    {
        return parameter + attackFlat;
    }

    override public string GetDescription()
    {
        return GetDescription(rank);
    }

    override public string GetDescription(int rank)
    {
        int temp = this.rank;
        this.rank = rank;
        string desc = baseColor + "Increases your attack power by " + amountColor + attackFlat
            + extraColor + " (+2)</color>";
        this.rank = temp;
        return desc;
    }
}

[System.Serializable]
public class RapidFire : Augment
{
    public RapidFire()
    {
        augmentName = "Rapid Fire";
        rarity = 0;
    }

    float attackSpeedFlat { get { return player.baseAttackSpeed * 0.2f * rank; } }

    override public float GetAttackSpeedFlat(float parameter)
    {
        return parameter + attackSpeedFlat;
    }

    override public string GetDescription()
    {
        return GetDescription(rank);
    }

    override public string GetDescription(int rank)
    {
        int temp = this.rank;
        this.rank = rank;
        string desc = baseColor + "Increases your attack speed by " + amountColor + 20 * rank + "%"
            + extraColor + " (+20%)</color>";
        this.rank = temp;
        return desc;
    }
}

[System.Serializable]
public class Velocity : Augment
{
    public Velocity()
    {
        augmentName = "Velocity";
        rarity = 0;
    }

    float projSpeedFlat { get { return player.baseProjSpeed * 0.2f * rank; } }

    override public float GetProjSpeedFlat(float parameter)
    {
        return parameter + projSpeedFlat;
    }

    override public string GetDescription()
    {
        return GetDescription(rank);
    }

    override public string GetDescription(int rank)
    {
        int temp = this.rank;
        this.rank = rank;
        string desc =  baseColor + "Increases your projectile speed by " + amountColor + 20 * rank
            + "%" + extraColor + " (+20%)</color>";
        this.rank = temp;
        return desc;
    }
}

[System.Serializable]
public class Quickness : Augment
{
    public Quickness()
    {
        augmentName = "Quickness";
        rarity = 0;
    }

    float movSpeedFlat { get { return player.baseMovSpeed * 0.2f * rank;  } }

    override public float GetMovSpeedFlat(float parameter)
    {
        return parameter + movSpeedFlat;
    }

    override public string GetDescription()
    {
        return GetDescription(rank);
    }

    override public string GetDescription(int rank)
    {
        int temp = this.rank;
        this.rank = rank;
        string desc = baseColor + "Increases your movement speed by <color=yellow>" + amountColor + 20 * rank
            + "%" + extraColor + " (+20%)</color>";
        this.rank = temp;
        return desc;
    }
}

[System.Serializable]
public class BlastShot : ApplyAugment
{
    public BlastShot()
    {
        augmentName = "Blast Shot";
        rarity = 1;
        explosionApply = false;
        contactApply = false;
    }

    float explosionDamage { get { return 0.2f + 0.2f * rank; } } //ratio of attack power to explosion damage
    float explosionRadius = 1f;

    override public void onHitWall(Vector2 position)
    {
        Damage newDamage = new Damage(Mathf.RoundToInt(player.attack * explosionDamage), player.augments.OfType<ApplyAugment>().ToList());
        ExplosionController.instance.SpawnExplosion(newDamage, position, explosionRadius, 0.5f);
    }

    public override void onHitEnemy(EnemyController enemy)
    {
        //create explosion at enemy
        Damage newDamage = new Damage(Mathf.RoundToInt(player.attack * explosionDamage), player.augments.OfType<ApplyAugment>().ToList());
        ExplosionController.instance.SpawnExplosion(newDamage, enemy.transform.position, explosionRadius, 0.5f);
    }

    override public string GetDescription()
    {
        return GetDescription(rank);
    }

    override public string GetDescription(int rank)
    {
        int temp = this.rank;
        this.rank = rank;
        string desc = baseColor + "Your projectile explodes upon impact\n"
            + "Explosion damage is equal to " + amountColor + (20 + 20 * rank) + "% "
            + extraColor + "(+20%) " + baseColor + "of your attack power\n"
            + "Base explosion radius is " + explosionRadius;
        this.rank = temp;
        return desc;
    }
}

[System.Serializable]
public class MeteoricImpact : ApplyAugment
{
    public MeteoricImpact()
    {
        augmentName = "Meteoric Impact";
        rarity = 1;
        explosionApply = false;
        projectileApply = false;
    }

    float explosionDamage { get { return 0.25f + 0.25f * rank; } } //ratio of attack power to explosion damage
    float explosionRadius = 2f;

    override public void onHitEnemyContact(EnemyController enemy)
    {
        //create explosion at enemy
        Damage newDamage = new Damage(Mathf.RoundToInt(player.contactDamage * explosionDamage), player.augments.OfType<ApplyAugment>().ToList());
        ExplosionController.instance.SpawnExplosion(newDamage, enemy.transform.position, explosionRadius, 0.5f);
    }

    override public string GetDescription()
    {
        return GetDescription(rank);
    }

    override public string GetDescription(int rank)
    {
        int temp = this.rank;
        this.rank = rank;
        string desc = baseColor + "Creates an explosion as you deal contact damage\n"
            + "Explosion damage is equal to " + amountColor + (25 + 25 * rank) + "% "
            + extraColor + "(+20%) " + baseColor + "of your contact damage\n"
            + "Base explosion radius is " + explosionRadius;
        this.rank = temp;
        return desc;
    }
}

[System.Serializable]
public class HighExplosives : Augment
{
    public HighExplosives()
    {
        augmentName = "High Explosives";
        rarity = 1;
    }

    float explosionDamageMulti { get { return 1.25f + 0.25f * rank; } }
    float explosionRadiusFlat { get { return 0.25f * rank; } }

    override public float GetExplosionDamageMulti(float parameter)
    {
        return parameter * explosionDamageMulti;
    }
    override public float GetExplosionRadiusFlat(float parameter)
    {
        return parameter + explosionRadiusFlat;
    }

    override public string GetDescription()
    {
        return GetDescription(rank);
    }

    override public string GetDescription(int rank)
    {
        int temp = this.rank;
        this.rank = rank;
        string desc = baseColor + "Your explosion damage is multiplied by " + amountColor + explosionDamageMulti + "x "
            + extraColor + "(+0.25x)\n"
            + baseColor + "Your base explosion radius is increased by " + amountColor + explosionRadiusFlat + " "
            + extraColor + "(+0.25)\n";
        this.rank = temp;
        return desc;
    }
}

[System.Serializable]
public class Supersonic : Augment
{
    public Supersonic()
    {
        augmentName = "Supersonic";
        rarity = 1;
    }

    float extraProjSpeed { get { return player.projSpeed / player.baseProjSpeed - 1; } }
    float projSpeedToAttack { get { return 00.5f * rank; } } //ratio of extra projectile speed to attack power multiplier
    float damageMulti { get { return extraProjSpeed * projSpeedToAttack + 1; } }

    override public int GetFinalAttackMulti(int parameter)
    {
        return Mathf.RoundToInt(parameter * damageMulti);
    }

    override public string GetDescription()
    {
        return GetDescription(rank);
    }

    override public string GetDescription(int rank)
    {
        int temp = this.rank;
        this.rank = rank;
        string desc = baseColor + "Your attack power is multiplied by " + amountColor + (0.5f * rank) + "x "
            + extraColor + "(+0.5x)" + baseColor + "of your extra projectile speed\n";
        this.rank = temp;
        return desc;
    }
}

[System.Serializable]
public class ReflectiveStrike : ApplyAugment
{
    public ReflectiveStrike()
    {
        augmentName = "Reflective Strike";
        rarity = 1;
        contactApply = false;
    }

    float reflectDamageMulti { get { return 0.8f; } } //ratio of extra projectile speed to attack power multiplier
    public int bounces { get { return rank; } }

    override public Damage onHitWallDamage(Damage damage)
    {
        damage.damage = Mathf.FloorToInt(damage.damage * reflectDamageMulti);
        return damage;
    }

    override public string GetDescription()
    {
        return GetDescription(rank);
    }

    override public string GetDescription(int rank)
    {
        int temp = this.rank;
        this.rank = rank;
        string desc = baseColor + "Your projectile bounces off the wall" + amountColor + bounces + " "
            + extraColor + "(+1) " + baseColor + "times\n"
            + baseColor + "Projectile's hit damage is multiplied by" + amountColor + reflectDamageMulti + "x "
            + extraColor + "" + baseColor + "each time it bounces off a wall\n";
        this.rank = temp;
        return desc;
    }
}

[System.Serializable]
public class PlatedArmor : Augment
{
    public PlatedArmor()
    {
        augmentName = "Plated Armor";
        rarity = 1;
    }

    int reductionFlat { get { return 2 * rank; } }

    override public int GetReductionFlat(int parameter)
    {
        if (player.augments.Exists(x => x.GetType() == typeof(Defiance))) return parameter + 2 * reductionFlat;
        return parameter + reductionFlat;
    }

    override public string GetDescription()
    {
        return GetDescription(rank);
    }

    override public string GetDescription(int rank)
    {
        int temp = this.rank;
        this.rank = rank;
        string desc = baseColor + "Reduce incoming damage by " + amountColor + reductionFlat + " "
            + extraColor + "(+2) " + baseColor + "flat to a minimum of 1\n";
        this.rank = temp;
        return desc;
    }
    
}

[System.Serializable]
public class SpikedHull : Augment
{
    public SpikedHull()
    {
        augmentName = "Spiked Hull";
        rarity = 1;
    }

    int contactDamageFlat { get { return 5 * rank; } }
    int contactDamageRatio { get { return Mathf.RoundToInt(player.health * 0.1f * rank); } } //ratio of hit points to contact damage

    override public int GetContactDamageFlat(int parameter)
    {
        return parameter + contactDamageFlat + contactDamageRatio;
    }

    override public string GetDescription()
    {
        return GetDescription(rank);
    }

    override public string GetDescription(int rank)
    {
        int temp = this.rank;
        this.rank = rank;
        string desc = baseColor + "You gain" + amountColor + 5 * rank + " "
            + extraColor + "(+5) " + baseColor + " contact damage\n"
            + baseColor + "You gain" + amountColor + 10 * rank + "% "
            + extraColor + "(+10%) " + baseColor + "of your maximum hit points as contact damage\n";
        this.rank = temp;
        return desc;
    }
}

[System.Serializable]
public class PreemptiveFire : Augment
{
    public PreemptiveFire()
    {
        augmentName = "Preemptive Fire";
        rarity = 1;
    }

    float attackMulti { get { return 0.75f; } }
    float attackSpeedMulti { get { return 1.5f + 0.5f * rank; } }
    float projSpeedMulti { get { return 0.5f; } }

    override public int GetAttackMulti(int parameter)
    {
        return Mathf.RoundToInt(parameter * attackMulti);
    }

    override public float GetAttackSpeedMulti(float parameter)
    {
        return parameter * attackSpeedMulti;
    }

    override public float GetProjSpeedMulti(float parameter)
    {
        return parameter * projSpeedMulti;
    }

    override public string GetDescription()
    {
        return GetDescription(rank);
    }

    override public string GetDescription(int rank)
    {
        int temp = this.rank;
        this.rank = rank;
        string desc = baseColor + "Your attack speed is multiplied by " + amountColor + attackSpeedMulti + "x "
            + extraColor + "(+0.5x) " + baseColor + "\n"
            + baseColor + "Your attack power is multiplied by " + amountColor + attackMulti + "x "
            + extraColor + "" + baseColor + "\n"
            + baseColor + "Your projectile speed is multiplied by " + amountColor + projSpeedMulti + "x "
            + extraColor + "" + baseColor + "\n";
        this.rank = temp;
        return desc;
    }
}

[System.Serializable]
public class WeaknessExposure : ApplyAugment
{
    public WeaknessExposure()
    {
        augmentName = "Weakness Exposure";
        rarity = 1;
    }

    float hitEffect { get { return 0.25f; } }
    float effectCap { get { return 0.5f + 0.5f * rank; } }

    override public void onHitEnemy(EnemyController enemy)
    {
        enemy.ApplyWeakness(hitEffect, effectCap);
    }

    override public string GetDescription()
    {
        return GetDescription(rank);
    }

    override public string GetDescription(int rank)
    {
        int temp = this.rank;
        this.rank = rank;
        string desc = baseColor + "Enemies hit by you take 25% extra damage for the next 3 seconds" + amountColor + ""
            + extraColor + "" + baseColor + "\n"
            + baseColor + "The effect stacks up to a maximum of " + amountColor + (50 + 50 * rank) + "% "
            + extraColor + "(+50%) " + baseColor + "\n"
            + baseColor + "Additional hits refresh the effect " + amountColor + ""
            + extraColor + "" + baseColor + "\n";
        this.rank = temp;
        return desc;
    }
}

[System.Serializable]
public class Adrenaline : ApplyAugment
{
    public Adrenaline()
    {
        augmentName = "Adrenaline";
        rarity = 1;
    }

    public float damageMulti { get { return 1.25f + 0.25f * rank; } }
    public float movMulti { get { return 1.25f + 0.25f * rank; } }
    public float decayTime { get { return 3f; } }

    override public void onKill(EnemyController enemy)
    {
        PlayerController.instance.TriggerAdrenaline();
    }

    override public string GetDescription()
    {
        return GetDescription(rank);
    }

    override public string GetDescription(int rank)
    {
        int temp = this.rank;
        this.rank = rank;
        string desc = baseColor + "You deal " + amountColor + damageMulti + "x "
            + extraColor + "(+0.25x) " + baseColor + "damage after you kill an enemy\n"
            + baseColor + "You gain " + amountColor + movMulti + "x "
            + extraColor + "(+0.25x) " + baseColor + "movement speed after you kill an enemy\n"
            + baseColor + "This effect decays over 3 seconds " + amountColor + ""
            + extraColor + "" + baseColor + "\n";
        this.rank = temp;
        return desc;
    }
}

[System.Serializable]
public class StormsEye : Augment
{
    public StormsEye()
    {
        augmentName = "Storm's Eye";
        rarity = 2;
    }

    public float attackSpeedMulti { get { return 1 + 1 * rank; } }
    public float rampTime { get { return 3f; } }

    override public string GetDescription()
    {
        return GetDescription(rank);
    }

    override public string GetDescription(int rank)
    {
        int temp = this.rank;
        this.rank = rank;
        string desc = baseColor + "You gain a ramping attack speed multiplier up to " + amountColor + attackSpeedMulti + "x "
            + extraColor + "(+1x) " + baseColor + "over 3 seconds when attacking continously\n"
            + baseColor + "You can no longer move when you attack" + amountColor + " "
            + extraColor + "" + baseColor + "\n";
        this.rank = temp;
        return desc;
    }
}

[System.Serializable]
public class Eruption : ApplyAugment
{
    public Eruption()
    {
        augmentName = "Eruption";
        rarity = 2;
    }

    float explosionDamage { get { return 0.05f * rank; } } //enemy hit point to damage ratio
    float explosionRadius { get { return 3f; } }

    override public void onKill(EnemyController enemy)
    {
        int overkill = -enemy.health;
        int totalDamage = overkill + Mathf.RoundToInt(explosionDamage * enemy.maxhealth);

        Damage newDamage = new Damage(Mathf.RoundToInt(totalDamage), player.augments.OfType<ApplyAugment>().ToList());
        ExplosionController.instance.SpawnExplosion(newDamage, enemy.transform.position, explosionRadius, 0.5f);
    }

    override public string GetDescription()
    {
        return GetDescription(rank);
    }

    override public string GetDescription(int rank)
    {
        int temp = this.rank;
        this.rank = rank;
        string desc = baseColor + "Enemes you kill explode" + amountColor  + " "
            + extraColor + "" + baseColor + "\n"
            + baseColor + "YExplosion damage is equal to " + amountColor + 5 * rank + "% "
            + extraColor + "(+5%) " + baseColor + "of their maximum hit points plus the over kill damage\n"
            + baseColor + "Base explosion radius is 3" + amountColor + " "
            + extraColor + "" + baseColor + "\n";
        this.rank = temp;
        return desc;
    }
}

[System.Serializable]
public class Defiance : Augment
{
    public Defiance()
    {
        augmentName = "Defiance";
        rarity = 2;
    }

    float reductionMulti { get { return 0.25f + 0.25f * rank; } }

    override public float GetReductionMulti(float parameter)
    {
        return parameter + reductionMulti;
    }

    override public string GetDescription()
    {
        return GetDescription(rank);
    }

    override public string GetDescription(int rank)
    {
        int temp = this.rank;
        this.rank = rank;
        string desc = baseColor + "You gain " + amountColor + (25 + 25 * rank) + "% "
            + extraColor + "(+25%) " + baseColor + "damage reduction\n"
            + baseColor + "Doubles the effectiveness of your \"Plated Armor\" and \"Fortitude\" augments" + amountColor + " "
            + extraColor + "" + baseColor + "\n";
        this.rank = temp;
        return desc;
    }
}

[System.Serializable]
public class Corrosion : ApplyAugment
{
    public Corrosion()
    {
        augmentName = "Corrosion";
        rarity = 2;
        explosionApply = false;
        contactApply = false;
    }

    int dot { get { return 10 * rank; } }
    int maxStack { get { return 2 + 3 * rank; } }

    override public void onHitEnemy(EnemyController enemy)
    {
        enemy.ApplyCorrosion(dot, maxStack);
    }

    override public string GetDescription()
    {
        return GetDescription(rank);
    }

    override public string GetDescription(int rank)
    {
        int temp = this.rank;
        this.rank = rank;
        string desc = baseColor + "Enemies hit by your projectiles take 5% of their max hit points and " + amountColor + dot + " "
            + extraColor + "(+10) " + baseColor + "damage over 3 seconds\n"
            + baseColor + "The effect can stack up to " + amountColor + maxStack + " "
            + extraColor + "(+3) " + baseColor + "times\n"
            + baseColor + "Additional hits refresh the effect " + amountColor + " "
            + extraColor + "" + baseColor + "\n"
            + baseColor + "Corrosion damage ignores enemy damage reduction " + amountColor + " "
            + extraColor + "" + baseColor + "\n";
        this.rank = temp;
        return desc;
    }
}

[System.Serializable]
public class Absorption : ApplyAugment
{
    public Absorption()
    {
        augmentName = "Absorption";
        rarity = 2;
    }

    int recovery { get { return rank; } }
    float recoveryContact { get { return 0.01f * rank; } } //percentage of hit point recovered per contact hit

    override public void onHitEnemyContact(EnemyController enemy)
    {
        player.ChangeHealth(Mathf.CeilToInt(recoveryContact * player.maxHealth));
    }

    override public void onHitEnemy(EnemyController enemy)
    {
        player.ChangeHealth(recovery);
    }

    override public string GetDescription()
    {
        return GetDescription(rank);
    }

    override public string GetDescription(int rank)
    {
        int temp = this.rank;
        this.rank = rank;
        string desc = baseColor + "You recover " + amountColor + recovery + " "
            + extraColor + "(+1) " + baseColor + "hit point(s) per enemy hit\n"
            + baseColor + "Contact hits additionally restore " + amountColor + rank + "% "
            + extraColor + "(+1%) " + baseColor + "of your maximum hit points\n";
        this.rank = temp;
        return desc;
    }
} 