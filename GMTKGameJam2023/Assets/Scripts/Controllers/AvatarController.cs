using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    Vector3 player;

    Vector3 car;

    float dis;

    public GameObject ply;

    public GameObject carr;

    public Run run;

    float y;

    Vector3 dirn;

    Vector3 front;

    float go;

    Vector3 speed;

    public float CarVelocity;

    float x;

    float angle;

    Vector3 vec;

    Vector3 left;

    Vector3 right;

    Vector3 ri;

    bool InCar = false;

    Animator m_Animator;

    float ang;

    void Start()
    {
        run = GameObject.Find("Player").GetComponent<Run>();

        CarVelocity = 10;

        m_Animator = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //Car animation

        Debug.Log(Transform.rotation.z);

        ang = transform.rotation.z;

        m_Animator.SetFloat("Angle", ang);

        //Entering car
        player = GameObject.FindWithTag("Player").transform.position;

        car = transform.position;

        dis = Mathf.Abs(car.magnitude - player.magnitude);

        if (Input.GetKeyDown("e") && dis < 2 && InCar == false)
        {
            ply.transform.parent = carr.transform;

            ply.transform.localPosition = new Vector3(0, 0, 0);

            run.enabled = false;

            InCar = true;

            ply.transform.localScale = new Vector3(0.25f, 0.45f, 1f);

            ply.transform.localRotation = Quaternion.identity;

        }

        if (Input.GetKeyDown("q") && InCar == true)
        {
            ply.transform.localPosition = new Vector3(0, 1, 0);

            ply.transform.parent = null;

            run.enabled = true;

            ply.transform.rotation = Quaternion.Euler(0, 0, 0);

            InCar = false;

            ply.transform.localScale = new Vector3(1, 1, 1);
        }

        //Movement

        if (InCar == true)
        {
            y = Input.GetAxisRaw("Vertical");

            front = GameObject.FindWithTag("front").transform.position;

            dirn = front - transform.position;

            speed = y * dirn * Time.deltaTime * CarVelocity;

            transform.position = transform.position + speed;

            x = Input.GetAxisRaw("Horizontal");

            //turning

            if (y != 0)
            {
                if (x > 0)
                {
                    //vector that is between the center of the car object and the right front of the car
                    right = GameObject.Find("right").transform.position - transform.position;

                    //Use triganometry to get the angle using tan and the direction vectors components then convert to degrees
                    float angle = Mathf.Atan2(right.y, right.x) * Mathf.Rad2Deg;

                    //set the angle of the object in the z
                    transform.rotation = Quaternion.Euler(0, 0, angle);
                }

                if (x < 0)
                {
                    //vector that is between the center of the car object and the right front of the car
                    left = GameObject.Find("left").transform.position - transform.position;

                    //Use triganometry to get the angle using tan and the direction vectors components then convert to degrees
                    float angle1 = Mathf.Atan2(left.y, left.x) * Mathf.Rad2Deg;


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

    // Returns a boolean as to whether or not the player reached their destination
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
        if(player.airlockOpen) Dash(true);

        if(grounded && !player.airlockOpen) {
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
                if(transform.position.x >= 5) {
                    transform.position += new Vector3(-1, 1, 0);
                }
                else if(transform.position.x >= 4) {
                    transform.position += new Vector3(0, 1.5f, 0);
                }
                else if(transform.position.x >= 3) {
                    transform.position += new Vector3(1, 1, 0);
                }
                else if(transform.position.x >= 2) {
                    transform.position += new Vector3(1.5f, 0, 0);
                }
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
        if(player.level == 0) {
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
        else if(player.level == 1) {

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

        

    }


    void updateTests() {
        suction = player.usedWeapon[0];
        testPassed[3] = player.usedWeapon[1];
        testPassed[4] = player.usedWeapon[2];
    }

    // SPEEDRUNER-SPECIFIC

    public bool enterRoom() {

        return Run(3.9f);
    }

    public void fightBoss() {

        int x = 5;
        float sTime = 0;

        // Attacking
        if(transform.position.x < 7f) {
            Run(x);
        }
        if(player.level >= 0) {
            if(Random.Range(0f, 1f) < 0.001f) {
                Jump();
                x = 1;
            }
            else if(Random.Range(0f, 1f) > 0.999f) {
                Jump();
                x = 3;
            }
            else if (Run(x)) {
                x = 5;
            }

            useAbility(0);
        }
        if(player.level >= 1) {
            if(player.isShielding) {
                sTime += Time.deltaTime;
            }
            if(sTime > 5 && player.isShielding) {
                useAbility(1);
            }
            else {
                sTime = 0;
                useAbility(0);
            }
        }
        if(player.level >= 2) {// <?>
            if(player.isShielding) {
                sTime += Time.deltaTime;
            }
            if(sTime > 5 && player.isShielding) {
                useAbility(1);
            }
            else {
                sTime = 0;
                useAbility(0);
            }
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

