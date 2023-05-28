using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    static public PlayerMovement instance;

    //assigned in inspector
    public float baseSpeed = 2f;
    public float maxAngle = 20f;
    public float deadzoneAngle = 5f;
    public int smoothing = 10;
    public bool calibrateBool;
    public float knockbackCoeff = 1;
    public float knockbackFadeP = 1;
    public float knockbackFade = 1;
    public float dashMultiplier;
    public float dashCooldown = 3;
    public float dashTimeThreshold = 0.5f;
    public int dashDistanceThreshold = 100;
    public ParticleSystem particles;


    float speedMultiplier { get { return PlayerController.instance.movSpeed * PlayerController.instance.adrenalineMovMulti; } }
    bool canMove { get { return PlayerController.instance.canMove; } }

    Rigidbody2D rb2D;
    float sqrDeadzoneAngle;
    float movementNormalize;

    float xAngleNormalize;
    float yAngleNormalize;
    bool calibrated = false;
    Vector2 touchOrigin;
    float dashCooldownTimer;
    float touchDurationTimer;

    Vector2 movementVector;
    Vector2 knockbackVector;
    List<Vector2> vectorList = new List<Vector2>();


    //apply knockback vector to player
    public void ApplyKnockback(Vector2 knockback)
    {
        knockbackVector = knockback;
    }

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        movementNormalize = 1f / maxAngle;
        sqrDeadzoneAngle = Mathf.Pow(deadzoneAngle, 2);
    }

    // Update is called once per frame
    void Update()
    {
        if (calibrated) 
        {
            if (canMove) movementVector = GetMovementVector();
            else movementVector = new Vector2(0, 0);
        }

        if (calibrateBool) //for debugging only
        {
            Calibrate();
            calibrateBool = false;
        }

        if (!GameController.instance.isPaused) GetShootAndDashDirection();
    }

    void FixedUpdate()
    {
        if (movementVector != null)
        {
            //get the smoothed movement vector and apply to rigid body
            Vector2 movement = getSmoothedMovement();
            rb2D.velocity = movement;

            //emit particles if player is moving
            if (movement.magnitude < 0.1f) particles.Stop();
            else
            {
                if (!particles.isPlaying) particles.Play();
            }
        }
        if (knockbackVector != null)
        {
            //additionally apply the knockback vector if it exists
            rb2D.velocity += knockbackVector;

            //dampen the knockback
            knockbackVector *= 1 - (1 - knockbackFadeP) * Time.deltaTime;
            knockbackVector = Vector2.ClampMagnitude(knockbackVector, knockbackVector.magnitude - knockbackFade * Time.deltaTime);
        }

    }
    
    //get the shooting direction and dash direction by screen touches
    void GetShootAndDashDirection()
    {
        dashCooldownTimer -= Time.deltaTime; //update dash cooldown

        if (Input.touchCount > 0) //if a touch exists
        {
            PlayerController.instance.isAttacking = true;

            Touch touch = Input.touches[0];
            touchDurationTimer += Time.deltaTime;

            if (touch.phase == TouchPhase.Began)
            {
                touchOrigin = touch.position;
                UIJoystick.instance.setOrigin(new Vector2(touchOrigin.x - Screen.width/2, touchOrigin.y - Screen.height/2)); //set origin of joystick to touch origin
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                Vector2 currentTouch = touch.position;
                UIJoystick.instance.setStick(new Vector2(currentTouch.x - Screen.width/2, currentTouch.y - Screen.height/2)); //set joystick stick to current touch position
                float x = currentTouch.x - touchOrigin.x;
                float y = currentTouch.y - touchOrigin.y;

                //rotate the game object according to touch (joystick)
                float angle = Mathf.Atan2(y, x) * Mathf.Rad2Deg - 90;
                if (angle < 0) angle += 360;
                gameObject.transform.eulerAngles = new Vector3(
                    gameObject.transform.eulerAngles.x,
                    gameObject.transform.eulerAngles.y,
                    angle
                );
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                //if the touch duration is short enough, the swipe distance is long enough
                //and dash cooldown is complete
                if (Vector2.Distance(touch.position, touchOrigin) > dashDistanceThreshold && touchDurationTimer < dashTimeThreshold && dashCooldownTimer < 0)
                {
                    //dash the player towards swipe direction
                    ApplyKnockback((touch.position - touchOrigin).normalized * speedMultiplier * dashMultiplier);
                    dashCooldownTimer = dashCooldown; //reset swipe cooldown
                }
            }
        }
        else
        {
            touchDurationTimer = 0;
            UIJoystick.instance.resetPosition(); //reset joy stick if no touch
            PlayerController.instance.isAttacking = false;
        }
    }

    //get movement vector according to accelerometer reading
    Vector2 GetMovementVector()
    {
        float xAngle = Mathf.Acos(-Mathf.Clamp(Input.acceleration.x,-1f,1f))*Mathf.Rad2Deg;
        float yAngle = Mathf.Acos(-Mathf.Clamp(Input.acceleration.y,-1f,1f))*Mathf.Rad2Deg;
        
        if (Input.acceleration.z > 0)
        {
            xAngle = 360 - xAngle;
            yAngle = 360 - yAngle;
        }

        xAngle -= xAngleNormalize;
        yAngle -= yAngleNormalize;

        if (xAngle > 180) xAngle = xAngle - 360;
        if (yAngle > 180) yAngle = yAngle - 360;
        if (xAngle < -180) xAngle = xAngle + 360;
        if (yAngle < -180) yAngle = yAngle + 360;

        Vector2 movement = new Vector2(xAngle, yAngle);
        if (movement.SqrMagnitude() < sqrDeadzoneAngle) return new Vector2(0f, 0f);
        movement = Vector2.ClampMagnitude(movement, maxAngle) * movementNormalize;
        return movement;
    }

    //get smoothed movment vector
    Vector2 getSmoothedMovement()
    {
        vectorList.Insert(0, movementVector);
        if (vectorList.Count > smoothing) vectorList.RemoveAt(vectorList.Count - 1);
        Vector2 smoothedVector = new Vector2(0f, 0f);
        foreach (var v in vectorList)
        {
            smoothedVector += v;
        }
        smoothedVector /= vectorList.Count;
        smoothedVector *= baseSpeed;
        smoothedVector *= speedMultiplier;
        return smoothedVector;
    }

    //function for debugging and prototyping
    IEnumerator StartupNormalizeAngle()
    {
        yield return new WaitForSeconds(1f);
        Calibrate();
        Debug.Log("calibrating");
        yield return null;
    }

    //calibrate the movement system, making current accelerometer readings as default
    public void Calibrate()
    {
        xAngleNormalize = Mathf.Acos(-Mathf.Clamp(Input.acceleration.x, -1f, 1f)) * Mathf.Rad2Deg;
        yAngleNormalize = Mathf.Acos(-Mathf.Clamp(Input.acceleration.y, -1f, 1f)) * Mathf.Rad2Deg;
        calibrated = true;
    }
}
