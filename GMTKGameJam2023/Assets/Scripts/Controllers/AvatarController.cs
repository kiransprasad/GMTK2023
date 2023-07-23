using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarController : MonoBehaviour
{

    public Animator animator;

    float horizontalMove = 0f;

    [SerializeField]
    public BossController boss;
    public LayerMask platformLayerMask;

    BoxCollider2D collider;

    bool colliding;

    [SerializeField] public GameObject afterimage;

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
    readonly float[] maxCooldown = { 1f, 0.5f, 12, 1, 2f };
    [SerializeField]
    public GameObject pellet;
    public GameObject flame;
    public GameObject chargeShot;

    // Mentor Variables
    bool[] testPassed = { false, false, false, false, false, true };
    bool mentorRight;
    bool suction;
    float suckTime;
    bool runBack;
    float jumpTimer;

    bool wallUp;
    float destroyTimer;

    Vector3 dashTarget;

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
        jumpForce = 25;

        cooldown = new float[maxCooldown.Length];

        mentorRight = true;
        suction = false;
        suckTime = 0;
        runBack = false;
        jumpTimer = 0;

        wallUp = true;
        destroyTimer = 0;

        dashTarget = transform.position;

    }

    void Update() {

        if(grounded == true) {
            animator.SetFloat("Down", 1);
            animator.SetFloat("TakeOff", 0);
            animator.SetFloat("Up", 0);
        }

        // Jump Animation
        if(jumpAnimState != 0) {

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

        if(transform.position.y > -1) {
            dashTarget.y = -2;
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
        else {
            grounded = false;
        }
    }

    // Returns a boolean as to whether or not the boss reached their destination
    public bool Run(float target, int stateInc = 0) {

        dashTarget.x = target;

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

        dashTarget.y = transform.position.y + 1;
        if(boss.airlockOpen) Dash(true);

        if(grounded && !boss.airlockOpen) {
            grounded = false;
            GetComponent<Rigidbody2D>().AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            jumpAnimState = 1;
        }


    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.gameObject.CompareTag("BossProjectile")) {
            if(tag == "Mentor") testPassed[0] = true;
            else colliding = true;
        }
        if(collision.gameObject.CompareTag("BossLaser")) {
            colliding = true;
        }
    }

    // Use all abilities
    public void useAbility(int weapNum) {

        if(cooldown[weapNum] == 0 && transform.localScale.x > 0) {

            if(weapNum == 0) {
                GameObject proj = Instantiate(pellet);
                proj.transform.position = transform.position;
                proj.GetComponent<BasicProjectile>().boss = boss;
            }
            else if(weapNum == 1) {
                GameObject proj = Instantiate(flame);
                proj.transform.position = transform.position;
                proj.GetComponent<BasicProjectile>().boss = boss;
                startCooldown(0, 2);
            }
            else if(weapNum == 2) {
                GameObject proj = Instantiate(chargeShot);
                proj.transform.position = transform.position;
                proj.GetComponent<BasicProjectile>().boss = boss;
                startCooldown(0, 3);
                startCooldown(1, 5);
            }

            startCooldown(weapNum);
        }
    }


    void Dash(bool force) {
        if(cooldown[4] == 0 || force) {

            GameObject a = Instantiate(afterimage);
            a.transform.position = transform.position;

            float dist = Mathf.Sqrt(Mathf.Pow(dashTarget.y - transform.position.y, 2) + Mathf.Pow(dashTarget.x - transform.position.x, 2));

            float clamp = 2;

            if(!force) {
                transform.position = Vector3.Lerp(transform.position, dashTarget, dist/clamp);
            }
            else {
                transform.position += new Vector3(1, 1, 0);
            }

            startCooldown(4);
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
        if(boss.level == 0) {
            if(mentorRight) {
                if(Run(7)) {
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
        else if(boss.level == 1) {

            if(mentorRight) {
                if(Run(7)) {
                    mentorRight = false;
                }
            }
            else {
                if(Run(-1)) {
                    mentorRight = true;
                }
            }

            useAbility(0);

            if(boss.isShieldBroken) {
                boss.resetShield();
                testPassed[1] = true;
            }

        }

        // Level 2: Use the Airlock
        if(boss.level == 2) {

            if(suction) {
                suckTime += Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(-6.5f, 2.2f, 0), suckTime / 600);
                if(suckTime > 3) {
                    suction = false;
                    runBack = true;
                }
            }
            else if(runBack) {
                if(Run(3.9f)) {
                    testPassed[boss.level] = true;
                }
            }
            else {
                if(mentorRight) {
                    if(Run(7)) {
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
        if(boss.level == 3) {

            if(mentorRight) {
                if(Run(7)) {
                    mentorRight = false;
                }
            }
            else {
                if(Run(-1)) {
                    mentorRight = true;
                }
            }

            useAbility(3);
            useAbility(0);
        }

        // Level 4: Use the Laser

        // Level 5: True

        if(Random.Range(0, 10.0f) < 0.1f && jumpTimer > 3) {
            Jump();
            jumpTimer = 0;
        }

        jumpTimer += Time.deltaTime;

        return testPassed[boss.level];
    }

    void updateTests() {
        suction = boss.usedWeapon[0];
        testPassed[3] = boss.usedWeapon[1];
        testPassed[4] = boss.usedWeapon[2];
    }

    // SPEEDRUNER-SPECIFIC

    public bool enterRoom() {

        return Run(3.9f);
    }

    public void fightBoss() {


        // ATTACKING
        useAbility(0);
        if(boss.level > 0 && (boss.isShielding || !boss.burned) && !boss.airlockOpen) {
            useAbility(1);
        }
        if(boss.level > 1) {
            useAbility(2);
        }

        // Defending
        if(colliding) {
            Dash(true);
        }
        else {
            Dash(false);
        }
    }


    public bool movePast() {

        destroyTimer += Time.deltaTime;

        if((int) destroyTimer % 2 == 0) {
            GameObject.FindGameObjectWithTag("Boss").transform.position += Vector3.right;
        }
        else {
            GameObject.FindGameObjectWithTag("Boss").transform.position += Vector3.left;
        }

        if(destroyTimer < 4) {
            return false;
        }

        return Run(-8);
    }

}