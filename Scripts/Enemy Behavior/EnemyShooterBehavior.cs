using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShooterBehavior : MonoBehaviour
{
    //assigned in inspector
    public LayerMask blockingLayer;
    public GameObject projectilePrefab;

    Rigidbody2D Rb2D;
    EnemyController controller;

    //variables for behavior
    bool goingLeft;
    float attackTimer = 0;
    float attackTimerThreshold { get { return 1 / controller.attackSpeed; } }
    float turnTimer;

    //determines if the enemy is near the player and can see them directly
    bool canSeePlayer { get {return (GameController.instance.playerObject.transform.position - transform.position).magnitude < 15 && !(Physics2D.Linecast(transform.position, GameController.instance.playerObject.transform.position, blockingLayer).collider != null); } }


    // Start is called before the first frame update
    void Start()
    {
        Rb2D = GetComponent<Rigidbody2D>();
        controller = GetComponent<EnemyController>();

        //randomize start direction
        goingLeft = Random.value > 0.5;
    }

    // Update is called once per frame
    void Update()
    {
        //update attack timer
        attackTimer += Time.deltaTime;
    }

    private void FixedUpdate()
    {
        if (canSeePlayer)
        {
            //look at the player
            Rb2D.rotation = Vector2.SignedAngle(Vector2.up, GameController.instance.playerObject.transform.position - transform.position);

            //attack if timer hits threshold and reset timer
            if (attackTimer > attackTimerThreshold)
            {
                FireProjectile();
                attackTimer = 0;
            }

            //circle around the player and switch directions occasionally
            turnTimer -= Time.deltaTime;
            if (turnTimer < 0)
            {
                turnTimer = Random.Range(3f, 6f);
                goingLeft = !goingLeft;
            }
            Vector2 movement = Vector2.Perpendicular(GameController.instance.playerObject.transform.position - transform.position).normalized;
            if (!goingLeft) Rb2D.AddForce(movement * controller.movSpeed, ForceMode2D.Force);
            else Rb2D.AddForce(-movement * controller.movSpeed, ForceMode2D.Force);
        };
    }

    //fire projectile at look direction
    void FireProjectile()
    {
        Vector2 firePosition = transform.position + transform.rotation * Vector2.up * 0.3f;
        GameObject projectileObject = Instantiate(projectilePrefab, firePosition, transform.rotation);
        projectileObject.GetComponent<Rigidbody2D>().velocity = transform.rotation * Vector2.up * controller.projSpeed;
        projectileObject.GetComponent<EnemyProjectileController>().damage = controller.attack;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        //switch directions if collided with terrain
        if (other.gameObject.tag.Equals("Terrain"))
        {
            turnTimer = Random.Range(5f, 8f);
            goingLeft = !goingLeft;
        }
    }
}
