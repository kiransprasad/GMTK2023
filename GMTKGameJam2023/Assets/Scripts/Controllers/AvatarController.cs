using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarController : MonoBehaviour
{

    public Animator animator;

    float horizontalMove = 0f;

    [SerializeField]
    public PlayerController player;
    public LayerMask platformLayerMask;

    BoxCollider2D collider;

    // X-movement
    float moveSpeed;
    bool isRunning;

    // Y-movement
    bool grounded;
    Vector3 groundPos;
    int jumpAnimState;
    [SerializeField] float jumpForce;

    // Projectiles
    float[] cooldown;
    readonly float[] maxCooldown = { 1, 1, 10, 1, 3 };
    [SerializeField]
    public GameObject pellet;
    public GameObject flame;
    public GameObject chargeShot;

    // Mentor Variables
    bool[] testPassed = { false, false, false, false, false, true };
    bool mentorRight;
    bool suction;
    float suckTime;
    float jumpTimer;

    // Start is called before the first frame update
    void Start() {

        collider = GetComponent<BoxCollider2D>();

        // X-movement
        moveSpeed = 4;
        isRunning = false;

        // Y-movement
        grounded = false;
        Vector3 groundPos = Vector3.zero;
        jumpAnimState = 0;
        jumpForce = 10;

        cooldown = new float[maxCooldown.Length];

        mentorRight = true;
        suction = false;
        suckTime = 0;
        jumpTimer = 0;

    }

    void Update() {

        if (grounded == true)
        {
            animator.SetFloat("Down", 1);
            animator.SetFloat("TakeOff", 0);
            animator.SetFloat("Up", 0);
        }

        // Jump Animation
        if (jumpAnimState != 0) {

            if(jumpAnimState == 1) {

                // Takeoff
                animator.SetFloat("TakeOff", 1);

                animator.SetFloat("Down", 0);

                jumpAnimState = 2;
            }
            else if(jumpAnimState == 2) {


                if(GetComponent<Rigidbody2D>().velocity.y > 0) {

                    // Up
                    animator.SetFloat("Up", 1);

                    animator.SetFloat("TakeOff", 0);
                }
                
            }
            else if(jumpAnimState == 3) {

                // Land

                jumpAnimState = 0;
            }
        }
        else if(isRunning) animator.SetFloat("Speed", 1);
        else animator.SetFloat("Speed", 0);

        if(grounded) {
            transform.position = new Vector3(transform.position.x, groundPos.y + collider.bounds.extents.y, 0);
            if(GetComponent<Rigidbody2D>().velocity.y < 0) {
                GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x, 0);
            }
        }

        updateTests();

        reduceCooldowns();
    }

    // FixedUpdate updates with the Physics engine
    // Ground check is in here
    private void FixedUpdate() {

        // Raycast Down
        RaycastHit2D ray = Physics2D.Raycast(collider.bounds.center, Vector2.down, collider.bounds.extents.y + 0.1f, platformLayerMask);
        Debug.DrawRay(collider.bounds.center, Vector2.down * (collider.bounds.extents.y + 0.1f));

        if(ray.collider && GetComponent<Rigidbody2D>().velocity.y < 0.1f) {
            groundPos = ray.point;
            grounded = true;
            if(jumpAnimState == 2) {
                GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x, 0);
                jumpAnimState = 3;
            }
        }
    }

    // Returns a boolean as to whether or not the player reached their destination
    public bool Run(float target, int stateInc = 0) {

        // Moving Left
        if(transform.position.x > target + Time.deltaTime) {
            transform.localScale = new Vector3(1, 1, 1);
            transform.position += Vector3.left * moveSpeed * Time.deltaTime;
            isRunning = true;
            return false;
        }

        // Moving Right
        else if(transform.position.x < target - Time.deltaTime) {
            transform.localScale = new Vector3(-1, 1, 1);
            transform.position += Vector3.right * moveSpeed * Time.deltaTime;
            isRunning = true;
            return false;
        }

        // Arrived at destination (should then face left by default)
        else {
            transform.localScale = new Vector3(1, 1, 1);
            transform.position = new Vector3(target, transform.position.y, 0);
            isRunning = false;
            return true;
        }

    }

    public void Jump() {

        if(grounded) {
            grounded = false;
            GetComponent<Rigidbody2D>().AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            jumpAnimState = 1;
        }


    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.gameObject.CompareTag("BossProjectileTest")) {
            testPassed[0] = true;
        }
    }

    // Use all abilities
    public void useAbility(int weapNum) {

        if(cooldown[weapNum] == 0 && transform.localScale.x > 0) {

            if(weapNum == 0) {
                GameObject proj = Instantiate(pellet);
                proj.transform.position = transform.position;
                proj.GetComponent<BasicProjectile>().player = player;
            }
            else if(weapNum == 1) {
                GameObject proj = Instantiate(flame);
                proj.transform.position = transform.position;
                proj.GetComponent<BasicProjectile>().player = player;
                startCooldown(0, 2);
            }
            else if(weapNum == 2) {
                GameObject proj = Instantiate(chargeShot);
                proj.transform.position = transform.position;
                proj.GetComponent<BasicProjectile>().player = player;
                startCooldown(0, 3);
                startCooldown(1, 5);
            }
            else if(weapNum == 3) {
                // <?> Slash
            }
            else {
                // <?> Dash
            }

            startCooldown(weapNum);
        }
    }

    void startCooldown(int weapon, int multiplier = 1) {
        cooldown[weapon] = maxCooldown[weapon] * multiplier;
    }

    void reduceCooldowns() {
        for(int i = 0; i < 5; ++i) {
            cooldown[i] = cooldown[i] - Time.deltaTime <= 0 ? 0 : cooldown[i] - Time.deltaTime;
        }
    }


    // MENTOR-SPECIFIC

    public bool testMechanic() {

        // Level 0: Hit Bewber with a projectile (OnCollision)
        if(player.level == 0) {
            if(mentorRight) {
                if(Run(8)) {
                    mentorRight = false;
                }
            }
            else {
                if(Run(-1)) {
                    mentorRight = true;
                }
            }
        }

        // Level 1: Block 3 Bullets
        else if(player.level == 1) {

            if(mentorRight) {
                if(Run(8)) {
                    mentorRight = false;
                }
            }
            else {
                if(Run(-1)) {
                    mentorRight = true;
                }
            }

            useAbility(0);

            if(player.isShieldBroken) {
                player.resetShield();
                testPassed[1] = true;
            }

        }

        // Level 2: Use the Airlock
        if(player.level == 2) {

            if(suction) {
                suckTime += Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(-6.5f, 2.2f, 0), suckTime);
                transform.rotation = Quaternion.Euler(0, 0, suckTime);
                if(suckTime > 3) {
                    suction = false;
                    transform.rotation = Quaternion.Euler(0, 0, suckTime);
                    testPassed[2] = true;
                }
            }
            else {
                if(mentorRight) {
                    if(Run(8)) {
                        mentorRight = false;
                    }
                }
                else {
                    if(Run(-1)) {
                        mentorRight = true;
                    }
                }

                useAbility(1);
            }
        }

        // Level 3: Use the Shockwave
        if(player.level == 3) {

            if(mentorRight) {
                if(Run(8)) {
                    mentorRight = false;
                }
            }
            else {
                if(Run(-3)) {
                    mentorRight = true;
                }
            }

            useAbility(3);
            useAbility(0);
        }

        // Level 4: Use the Laser

        // Level 5: True

        if(transform.position.x % 10 - Random.Range(-10.0f, 10.0f) < 0.05f && jumpTimer > 2) {
            Jump();
            jumpTimer = 0;
        }

        jumpTimer += Time.deltaTime;

        return testPassed[player.level];
    }

    void updateTests() {
        suction = player.usedWeapon[0];
        testPassed[3] = player.usedWeapon[1];
        testPassed[4] = player.usedWeapon[2];
    }

    // SPEEDRUNER-SPECIFIC

    public bool enterRoom() {

        if(transform.position.x > 4 - Time.deltaTime && transform.position.x < 4 + Time.deltaTime) {

        }

        return Run(3.9f);
    }

    public void fightBoss() {



    }


    public bool movePast() {

        return true;

    }
}
