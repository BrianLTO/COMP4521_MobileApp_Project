using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EnemyController : MonoBehaviour
{
    //assigned in inspector
    public GameObject damagedParticle;
    public GameObject destroyedParticle;
    public GameObject corrosionParticle;

    //attributes for the enemy
    [HideInInspector] public bool isAttacking { get; private set; }
    [HideInInspector] public int health { get; private set; }
    [HideInInspector] public int maxhealth { get; private set; }
    [HideInInspector] public int attack { get; private set; }
    [HideInInspector] public int contactDamage { get; private set; }
    [HideInInspector] public float attackSpeed { get; private set; }
    [HideInInspector] public float projSpeed { get; private set; }
    [HideInInspector] public float movSpeed { get; private set; }
    [HideInInspector] public int reduction { get; private set; }

    bool initialized = false;
    bool isDestroyed { get { return health <= 0; } }

    float weaknessDuration = 3;
    float weaknessTimeRemaining;
    float weaknessMultiplier = 1;

    float corrosionDuration = 3;
    float corrosionTimeRemaining;
    float corrosionDOT = 0;
    float corrosionDamageBuffer = 0;
    int corrosionStacks = 0;

    // Update is called once per frame
    void Update()
    {
        if (initialized)
        {
            //call OnDestroyed when health is less or equal to 0
            if (isDestroyed) OnDestroyed();
        }
    }

    private void FixedUpdate()
    {
        //update weakness effect and timers
        if (weaknessMultiplier > 1)
        {
            weaknessTimeRemaining -= Time.deltaTime;
            if (weaknessTimeRemaining < 0)
            {
                weaknessMultiplier = 1;
                weaknessTimeRemaining = weaknessDuration;
            }
        }

        //update corrosion effect and timers
        if (corrosionStacks > 0)
        {
            corrosionTimeRemaining -= Time.deltaTime;
            if (corrosionTimeRemaining < 0)
            {
                corrosionStacks = 0;
                corrosionTimeRemaining = corrosionDuration;
            }
            DealCorrosionDamage();
        }
    }

    //apply the weakness effect
    public void ApplyWeakness(float effect, float cap)
    {
        if (weaknessMultiplier-1 < cap)
        {
            weaknessMultiplier += effect;
            if (weaknessMultiplier - 1 > cap) weaknessMultiplier = cap+1;
        }
        weaknessTimeRemaining = weaknessDuration;
    }

    //apply the corrosion effect according the input parameters
    public void ApplyCorrosion(int dot, int cap)
    {
        corrosionDOT = dot + maxhealth * 0.05f;
        if (corrosionStacks < cap) corrosionStacks++;
        corrosionTimeRemaining = corrosionDuration;
    }

    //calculate and deal corrosion damage
    public void DealCorrosionDamage()
    {
        corrosionDamageBuffer += Time.deltaTime * corrosionDOT * corrosionStacks * weaknessMultiplier * PlayerController.instance.damageMulti / corrosionDuration;
        int damage = Mathf.FloorToInt(corrosionDamageBuffer);
        corrosionDamageBuffer -= damage;
        PlayerController.instance.damageDealt += damage;
        health -= damage;
        if (damage > 0) Instantiate(corrosionParticle, transform.position, Quaternion.identity); //spawn particles if damaged this frame
        UIEnemyHealthBar.instance.TrackThisEnemy(health / (float)maxhealth, gameObject); //update UI health bar

        //trigger adrenaline on kill
        if (isDestroyed)
        {
            if (PlayerController.instance.augments.Any(x => x.GetType() == typeof(Adrenaline)))
            {
                (PlayerController.instance.augments.Find(x => x.GetType() == typeof(Adrenaline)) as Adrenaline).onKill(this);
            }
        }
    }

    //deal damage to the enemy
    public void DealDamage(Damage damageInstance)
    {
        int modifiedDamage = Mathf.RoundToInt(damageInstance.damage * PlayerController.instance.damageMulti * weaknessMultiplier);
        modifiedDamage -= reduction;
        modifiedDamage = Mathf.Max(1, modifiedDamage);
        health -= modifiedDamage;
        PlayerController.instance.damageDealt += modifiedDamage; //store damage dealt for end screen stats
        Instantiate(damagedParticle, transform.position, Quaternion.identity); //spawn damaged particles
        UIEnemyHealthBar.instance.TrackThisEnemy(health / (float)maxhealth, gameObject); //update UI health bar

        if (modifiedDamage > 1)
        {
            GraphicsController.instance.makeWallGlow(new Color(0.5f, 1f, 1f), 0.2f, 0.5f, 0); //enemy damaged glow
        }

        //apply on hit effects of augments
        foreach (ApplyAugment augment in damageInstance.augments)
        {
            augment.onHitEnemy(this);
        }
        if (isDestroyed)
        {
            //apply on kill effects of augments
            GameController.instance.destroyCount++; //store kill count for end screen stats
            foreach (ApplyAugment augment in damageInstance.augments)
            {
                augment.onKill(this);
            }
            OnDestroyed();
        }
    }

    //initialize stats according to the enemy type
    public void InitializeStats(Enemy enemyType)
    {
        health = enemyType.health;
        maxhealth = enemyType.health;
        attack = enemyType.attack;
        contactDamage = enemyType.contactDamage;
        attackSpeed = enemyType.attackSpeed;
        projSpeed = enemyType.projSpeed;
        movSpeed = enemyType.movSpeed;
        reduction = enemyType.reduction;
        initialized = true;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag.Equals("Player")) //oncollision with player
        {
            //Damage the player
            PlayerController PC = PlayerController.instance;
            PC.ChangeHealth(-contactDamage);

            //Apply knockback to player
            int augmentedDamage = PlayerController.instance.GetAugmentedDamage(contactDamage);
            if (augmentedDamage > 1)
            {
                Vector2 knockback = (other.transform.position - transform.position).normalized * PlayerMovement.instance.knockbackCoeff * Mathf.Log(augmentedDamage / (PlayerController.instance.maxHealth * 0.05f) + 2, 2);
                PlayerMovement.instance.ApplyKnockback(knockback);
            }

            //Apply player contact augments
            Damage contact = new Damage(Mathf.RoundToInt(PC.contactDamage * PC.damageMulti), PC.augments.OfType<ApplyAugment>().ToList().FindAll(x => x.contactApply));
            foreach (ApplyAugment augment in contact.augments)
            {
                augment.onHitEnemyContact(this);
            }

            //Damage the enemy
            DealDamage(contact);
        }
    }


    void OnDestroyed()
    {
        Instantiate(destroyedParticle, transform.position, Quaternion.identity); //spawn destroyed particles
        gameObject.SetActive(false); //no need to destroy as level ends quickly
    }
}
