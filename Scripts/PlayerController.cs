using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    static public PlayerController instance = null;

    //assigned in inspector
    public int baseHealth = 100;
    public int baseAttack = 10;
    public float baseAttackSpeed = 1;
    public float baseProjSpeed = 5;
    public float baseMovSpeed = 1;
    public int baseReductionFlat = 0;
    public float baseReductionMulti = 0;
    public float baseDamagemulti = 1;
    public GameObject projectilePrefab;
    public int baseContactDamage { get; } = 10;
    public GameObject damagedParticle;
    public GameObject destroyedParticle;
    public AudioClip damagedSound;
    public AudioClip destroyedSound;

    //player attributes
    [HideInInspector] public int health { get; private set; }
    [HideInInspector] public int maxHealth { get; private set; }
    [HideInInspector] public int attack { get; private set; }
    [HideInInspector] public int contactDamage { get; private set; }
    [HideInInspector] public float attackSpeed { get; private set; }
    [HideInInspector] public float projSpeed { get; private set; }
    [HideInInspector] public float movSpeed { get; private set; }
    [HideInInspector] public int reductionFlat { get; private set; }
    [HideInInspector] public float reductionMulti { get; private set; }
    [HideInInspector] public float damageMulti { get { return baseDamagemulti * adrenalineDamageMulti; }  }
    [HideInInspector] public float explosionMulti { get; private set; }
    [HideInInspector] public float explosionRange { get; private set; }
    [HideInInspector] public int upgradePoint { get; set; } = 0;
    [HideInInspector] public int rerollPoint { get; set; } = 0;

    //lists for holding all owned augments
    [HideInInspector] public List<Augment> augments { get; private set; } = new List<Augment>();

    //player current conditions
    [HideInInspector] public bool isAttacking { get; set; } = false;
    [HideInInspector] public bool canMove { get; private set; } = true;
    [HideInInspector] public bool isDamaged { get { return health < maxHealth; } }

    //variables used for save data
    [HideInInspector] public int upgradePointOnEnter { get; set; } = 0;
    [HideInInspector] public int rerollPointOnEnter { get; set; } = 0;
    [HideInInspector] public int healthOnLevelEnter { get; set; } = 0;
    [HideInInspector] public int damageDealt { get; set; } = 0;
    [HideInInspector] public int damageTaken { get; set; } = 0;
    [HideInInspector] public int damageHealed { get; set; } = 0;
    [HideInInspector] public int damageDealtOnEnter { get; set; } = 0;
    [HideInInspector] public int damageTakenOnEnter { get; set; } = 0;
    [HideInInspector] public int damageHealedOnEnter { get; set; } = 0;

    //variables for firing projectiles
    float attackTimer;
    float attackTimerThreshold { get { return 1 / attackSpeed; } }

    //storms eye augment variables
    float stormsEyeAttackSpeedMax;
    float stormsEyeAttackSpeed = 1;
    float stormsEyeRamp;

    //adrenaline augment variables
    float adrenalineDecayTime = 0;
    float adrenalineDamageMulti = 1;
    float adrenalineDamageMultiMax = 1;
    public float adrenalineMovMulti { get; private set; } = 1;
    float adrenalineMovMultiMax = 1;

    //variables for player destroyed
    bool isDestroyed { get { return health <= 0 && GameController.instance.isPlaying; } }
    bool playingDestroyedAnim;

    //Trigger the adrenaline effect
    public void TriggerAdrenaline()
    {
        adrenalineDamageMulti = adrenalineDamageMultiMax;
        adrenalineMovMulti = adrenalineMovMultiMax;
    }

    //return the actual damage taken after reduction (used for knockback calculations)
    public int GetAugmentedDamage(int damage)
    {
        damage *= Mathf.RoundToInt(1 - (reductionMulti / (reductionMulti + 1)));
        damage -= reductionFlat;
        damage = Mathf.Max(1, damage);
        return damage;
    }

    //initialize player stats from save data
    public void InitializeFromSave(SaveData save)
    {
        //initlaize stats from save
        upgradePoint = save.upgradePoint;
        rerollPoint = save.rerollPoint;
        health = save.currentHealth;
        maxHealth = save.maxHealth;
        damageDealt = save.damageDealt;
        damageTaken = save.damageTaken;
        damageHealed = save.damageHealed;
        augments = save.augments;
        Initialize();
    }

    //reset player stats for new game
    public void InitializeNewGame()
    {
        //reset some player stat
        upgradePoint = 0;
        rerollPoint = 0;
        augments.Clear();
        health = maxHealth;

        //reset end screen stat
        damageDealt = 0;
        damageTaken = 0;
        damageHealed = 0;

    }

    //initialize player base stats from global upgrades
    public void Initialize()
    {
        baseHealth = Mathf.RoundToInt(baseHealth * (1 + GameController.instance.healthLevel * 0.1f));
        baseAttack = Mathf.RoundToInt(baseAttack * (1 + GameController.instance.attackLevel * 0.1f));
        baseAttackSpeed *= 1 + GameController.instance.attackLevel * 0.1f;
        baseProjSpeed *= 1 + GameController.instance.projSpeedLevel * 0.1f;
        baseMovSpeed *= 1 + GameController.instance.movSpeedLevel * 0.1f;
        baseReductionFlat = GameController.instance.reductionFlatLevel;
        playingDestroyedAnim = false;
        UpdateStats();
    }

    void Awake()
    {
        //singleton script
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isDestroyed && !playingDestroyedAnim) OnDestroyed(); //play death animation once if destroyed
        UpdateAttack();
        if (adrenalineDecayTime > 0) UpdateAdrenaline(); //update adrenaline timers
    }
    
    //change the players health (used for both healing and damaging)
    public void ChangeHealth(int amount)
    {
        int modifiedAmount = amount;
        if (amount < 0) //if it is damaging
        {
            //calculate reductions
            modifiedAmount *= Mathf.RoundToInt(1 - (reductionMulti / (reductionMulti + 1)));
            modifiedAmount += reductionFlat;
            modifiedAmount = Mathf.Min(-1, modifiedAmount);
            Instantiate(damagedParticle, GameController.instance.playerObject.transform.position, Quaternion.identity);
            GameController.instance.audioSource.PlayOneShot(damagedSound);
        }

        if (modifiedAmount > 0)
        {
            damageHealed += modifiedAmount; //store damage healed for end screen
        }
        else
        {
            damageTaken += modifiedAmount; //store damage taken for end screen
        }

        if (modifiedAmount < -1) //if player takes damage more than minimum amount
        {
            GraphicsController.instance.makeWallGlow(new Color(1f, 0f, 0f), 0.3f + Mathf.Log(-modifiedAmount, 100), 1f, 2); //player damaged glow
        }

        health += modifiedAmount;
        if (health >= maxHealth) health = maxHealth;
        UIHealthBar.instance.SetValue(health / (float)maxHealth); //set health bar UI
    }

    //update adrenaline effect and timer
    void UpdateAdrenaline()
    {
        if (adrenalineDamageMulti > 1)
        {
            adrenalineDamageMulti -= (adrenalineDamageMultiMax - 1) * Time.deltaTime;
            if (adrenalineDamageMulti < 1) adrenalineDamageMulti = 1;
        }
        if (adrenalineMovMulti > 1)
        {
            adrenalineMovMulti -= (adrenalineMovMultiMax - 1) * Time.deltaTime;
            if (adrenalineMovMulti < 1) adrenalineMovMulti = 1;
        }
    }

    //update attack timers and storms eye effect
    void UpdateAttack()
    {
        if (isAttacking)
        {
            if (stormsEyeAttackSpeedMax > 1)
            {
                canMove = false;
                if (stormsEyeAttackSpeed < stormsEyeAttackSpeedMax)
                {
                    stormsEyeAttackSpeed += stormsEyeAttackSpeedMax * Time.deltaTime / stormsEyeRamp;
                    if (stormsEyeAttackSpeed > stormsEyeAttackSpeedMax) stormsEyeAttackSpeed = stormsEyeAttackSpeedMax;
                }
                if (attackTimer > attackTimerThreshold / stormsEyeAttackSpeed)
                {
                    attackTimer = attackTimerThreshold / stormsEyeAttackSpeed - (attackTimerThreshold - attackTimer);
                }
            }
            if (attackTimer <= 0)
            {
                FireProjectile();
                attackTimer = attackTimerThreshold / stormsEyeAttackSpeed;
            }

        }
        else
        {
            stormsEyeAttackSpeed = 1;
            canMove = true;
        }
        attackTimer -= Time.deltaTime;
    }

    //update stats according to held augments
    public void UpdateStats()
    {
        int healthMissing = maxHealth - health;
        maxHealth = baseHealth;
        attack = baseAttack;
        contactDamage = baseContactDamage;
        attackSpeed = baseAttackSpeed;
        projSpeed = baseProjSpeed;
        movSpeed = baseMovSpeed;
        reductionFlat = baseReductionFlat;
        reductionMulti = baseReductionMulti;
        explosionMulti = 1;
        explosionRange = 0;

        //get flat modifications for stats
        foreach (var v in augments)
        {
            maxHealth = v.GetHealthFlat(maxHealth);
            attack = v.GetAttackFlat(attack);
            attackSpeed = v.GetAttackSpeedFlat(attackSpeed);
            projSpeed = v.GetProjSpeedFlat(projSpeed);
            movSpeed = v.GetMovSpeedFlat(movSpeed);
            reductionFlat = v.GetReductionFlat(reductionFlat);
            explosionMulti = v.GetExplosionDamageMulti(explosionMulti);
            explosionRange = v.GetExplosionRadiusFlat(explosionRange);
        }

        //get multiplied modifications for stats
        foreach (var v in augments)
        {
            maxHealth = v.GetHealthMulti(maxHealth);
            attack = v.GetAttackMulti(attack);
            attackSpeed = v.GetAttackSpeedMulti(attackSpeed);
            projSpeed = v.GetProjSpeedMulti(projSpeed);
            movSpeed = v.GetMovSpeedMulti(movSpeed);
            reductionMulti = v.GetReductionMulti(reductionMulti);
        }

        //get final modifications for stats
        foreach (var v in augments)
        {
            attack = v.GetFinalAttackMulti(attack);
            contactDamage = v.GetContactDamageFlat(contactDamage);
        }

        //get storms eye stats
        if (augments.Any(x => x.GetType() == typeof(StormsEye)))
        {
            StormsEye stormsEye = augments.Find(x => x.GetType() == typeof(StormsEye)) as StormsEye;
            stormsEyeAttackSpeedMax = stormsEye.attackSpeedMulti;
            stormsEyeRamp = stormsEye.rampTime;
        }
        else
        {
            stormsEyeAttackSpeedMax = 1;
        }

        //get adrenaline stats
        if (augments.Any(x => x.GetType() == typeof(Adrenaline)))
        {
            Adrenaline adrenaline = augments.Find(x => x.GetType() == typeof(Adrenaline)) as Adrenaline;
            adrenalineDecayTime = adrenaline.decayTime;
            adrenalineDamageMultiMax = adrenaline.damageMulti;
            adrenalineMovMultiMax = adrenaline.movMulti;
        }

        health = maxHealth - healthMissing; //deduct health missing 

        //used for save data
        healthOnLevelEnter = health;
        damageDealtOnEnter = damageDealt;
        damageTakenOnEnter = damageTaken;
        damageHealedOnEnter = damageHealed;
        upgradePointOnEnter = upgradePoint;
        rerollPointOnEnter = rerollPoint;
    }

    //fire a projectile
    void FireProjectile()
    {
        GameObject playerObject = GameController.instance.playerObject;
        if (playerObject == null) return;
        Vector2 firePosition = playerObject.transform.position + playerObject.transform.rotation * Vector2.up * 0.3f;

        GameObject projectileObject = Instantiate(projectilePrefab, firePosition, playerObject.transform.rotation);
        projectileObject.GetComponent<Rigidbody2D>().velocity = playerObject.transform.rotation * Vector2.up * projSpeed;
        projectileObject.GetComponent<PlayerProjectileController>().damage = new Damage(attack, augments.OfType<ApplyAugment>().ToList()); //put augments in the damage instance
        Destroy(projectileObject, 10f); //destroy projectile after 10 seconds
    }

    //called ONCE when player is destroyed
    void OnDestroyed()
    {
        playingDestroyedAnim = true;
        StartCoroutine(PlayerDestroyedAnimation()); //play death animation
        UIIngame.instance.gameObject.SetActive(false); //disable in-game UI
    }

    //death animation
    IEnumerator PlayerDestroyedAnimation()
    {
        GameController.instance.playerObject.GetComponent<PlayerMovement>().enabled = false; //no moving allowed during death animation
        var destroyedEffect = Instantiate(destroyedParticle, GameController.instance.playerObject.transform.position, Quaternion.identity); //spawn death particles
        destroyedEffect.transform.parent = GameController.instance.playerObject.transform;
        yield return new WaitForSeconds(1f);
        GameController.instance.playerObject.GetComponent<SpriteRenderer>().enabled = false; //remove sprite of player
        GameController.instance.playerObject.GetComponent<Rigidbody2D>().simulated = false; //remove player collision
        GameController.instance.audioSource.PlayOneShot(destroyedSound);
        yield return new WaitForSeconds(1f);
        StartCoroutine(UIGameover.instance.ShowEndScreen()); //show end screen animation
    }
}
