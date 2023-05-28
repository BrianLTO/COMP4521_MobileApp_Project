using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRammerBehavior : MonoBehaviour
{
    public LayerMask blockingLayer;

    Rigidbody2D Rb2D;

    //behavior variables
    public float ramThreshold = 720;
    public float spinMultiplier = 2;
    public float dashSpeed = 5;
    float movSpeed { get { return GetComponent<EnemyController>().movSpeed; } }
    float ramCooldownTime { get { return 2 / movSpeed; } }
    float ramCooldown = 0.1f;
    bool ramMode = false;

    //determines if the enemy is near the player and can see them directly
    bool canSeePlayer { get { return (GameController.instance.playerObject.transform.position - transform.position).magnitude < 10 && !(Physics2D.Linecast(transform.position, GameController.instance.playerObject.transform.position, blockingLayer).collider != null); } }

    void Awake()
    {
        Rb2D = GetComponent <Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        //if spinning fast enough, ram at the player
        if (Rb2D.angularVelocity > ramThreshold) ramMode = true;

        //update ram cooldown
        ramCooldown -= Time.deltaTime;
        //lock cooldown if the player cannot be seen
        if (!canSeePlayer) ramCooldown = 0.1f;
    }

    private void FixedUpdate()
    {
        if (ramMode)
        {
            //add force with vector towards the player
            Vector2 dashDirection = (GameController.instance.playerObject.transform.position - transform.position).normalized;
            Rb2D.AddForce(dashDirection * movSpeed * dashSpeed, ForceMode2D.Impulse);

            //angular drag for slowing rotation
            Rb2D.angularDrag = 3;

            //reset ram cooldown and mode
            ramCooldown = ramCooldownTime;
            ramMode = false;
        }
        else
        {
            if (ramCooldown <= 0)
                //no angular drag during charge up
                Rb2D.angularDrag = 0;
                Rb2D.AddTorque(movSpeed * spinMultiplier, ForceMode2D.Force);
        }
    }
}
