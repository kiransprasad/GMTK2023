using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerController : MonoBehaviour
{

    [SerializeField]
    public Texture2D crosshairs;
    public int level;
    public Light2D globalLight;
    [Header("Parts")]
    public Transform body;
    public Transform shoulder;
    public Transform arm;
    public Transform legs;
    [Header("Sprites")]
    public Sprite[] bodySprites;
    public Sprite[] shoulderSprites;
    public Sprite[] armSprites;
    public Sprite[] legsSprites;
    [Header("Lights")]
    public Light2D[] lights;
    [Header("Projectiles")]
    public GameObject bullet;
    public GameObject shockwave;

    readonly Color[] lightColours = {
        Color.yellow,
        new Color(1, 0.5f, 0),
        Color.magenta,
        new Color(0.9f, 0.25f, 0),
        Color.blue,
        Color.red
    };

    int bossHP;

    // Weapon Properties
    // Bullet
    int volley;
    float volleyTime;
    const float volleyCooldown = 0.3f;

    // Shield
    bool isShielding;
    public bool isShieldBroken;

    // Airlock
    Transform airlock;
    bool airlockOpen;

    // Shockwave
    float shockwaveCharge;
    const float shockwaveMaxCharge = 1;

    // Laser
    float laserCharge;
    const float laserMaxCharge = 2;
    float laserDuration;
    const float laserMaxDuration = 5;

    // Cooldowns <?>
    float[] cooldown;
    readonly float[] maxCooldown = { 3, 3, 12, 15, 20 };
    int currentAction;

    // Animation
    int animState;
    float idleBodyY, idleShoulderY;

    // Progress Booleans
    public bool[] usedWeapon = { false, false, false };

    // Start is called before the first frame update
    void Start() {

        bossHP = 100;

        // Set all sprites
        Cursor.SetCursor(crosshairs, new Vector2(16, 16), CursorMode.Auto);
        body.GetComponent<SpriteRenderer>().sprite = bodySprites[level];
        shoulder.GetComponent<SpriteRenderer>().sprite = shoulderSprites[level];
        arm.GetComponent<SpriteRenderer>().sprite = armSprites[level];
        arm.GetChild(0).GetComponent<SpriteRenderer>().sprite = armSprites[level];
        legs.GetComponent<SpriteRenderer>().sprite = legsSprites[level];

        // Lights
        for(int i = 0; i < lights.Length; ++i) {
            lights[i].color = lightColours[level];
        }
        arm.GetChild(2).GetChild(1).GetComponent<Light2D>().color = lightColours[level];

        // Projectiles
        volley = 0;
        volleyTime = 0.5f;

        // Shield
        isShielding = false;
        isShieldBroken = false;

        // Airlock
        airlock = transform.GetChild(4);
        if(level < 2) airlock.gameObject.SetActive(false);
        airlockOpen = false;

        // Shockwave
        shockwaveCharge = 0;

        // Laser
        laserCharge = 0;
        laserDuration = 0;

        cooldown = new float[5];
        currentAction = 0;

        animState = 0;
        idleBodyY = body.position.y;
        idleShoulderY = shoulder.position.y;
    }

    // Update is called once per frame
    void Update() {

        if(UIController.pause) return;

        if(currentAction == 0) {

            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // Animation
            Animate(animState);

            // Graphics
            DrawArm(mousePos);

            // Inputs for actions
            // Shooting
            if(canUse(0) && Input.GetMouseButtonDown(0) && mousePos.x > -6) Shoot();
            if(volley != 0) {
                volleyTime -= Time.deltaTime;
                if(volleyTime < 0) {
                    Shoot();
                }
            }

            // Shielding
            if(canUse(1) && !isShieldBroken && Input.GetMouseButtonDown(1)) Shield(true);
            if(isShielding && Input.GetMouseButtonUp(1)) Shield(false);

            // Airlock
            if(canUse(2) && Input.GetKeyDown(KeyCode.Alpha1)) Airlock();
            // Airlock effects <?>

            // Shockwave & Laser (Swap update for their own code)
            if(canUse(3) && Input.GetKeyDown(KeyCode.Alpha2)) {
                currentAction = 1;
                usedWeapon[1] = true;
            }
            if(canUse(4) && Input.GetKeyDown(KeyCode.Alpha3)) {
                currentAction = 2;
                usedWeapon[2] = true;
            }
        }
        else if(currentAction == 1) Shockwave();
        else if(currentAction == 2) Laser();

        // Cooldowns
        reduceCooldowns();

        // Restore Light
        if(globalLight.intensity < 1 && currentAction != 2) globalLight.intensity += 0.025f;
        if(lights[2].intensity < 1 && currentAction != 2) {
            for(int i = 2; i < lights.Length; ++i) {
                lights[i].intensity += 0.05f;
            }
        }

    }

    // Idle animation for boss
    void Animate(int state) {
        if(state == 0) {
            if(Time.time % 1.5f < 0.625f) { // 0 - 5/8: High Rest
                body.position = new Vector3(body.position.x, idleBodyY, 0);
                shoulder.position = new Vector3(shoulder.position.x, idleShoulderY, 0);
            }
            else if(Time.time % 1.5f < 0.75f) { // 5/8 - 6/8: Head drop
                body.position = new Vector3(body.position.x, idleBodyY - 0.1f, 0);
                shoulder.position = new Vector3(shoulder.position.x, idleShoulderY, 0);
            }
            else if(Time.time % 1.5f < 1.375f) { // 6/8 - 11/8: Low Rest
                body.position = new Vector3(body.position.x, idleBodyY - 0.1f, 0);
                shoulder.position = new Vector3(shoulder.position.x, idleShoulderY - 0.1f, 0);
            }
            else { // 11/8 - 12/8: Head Raise
                body.position = new Vector3(body.position.x, idleBodyY, 0);
                shoulder.position = new Vector3(shoulder.position.x, idleShoulderY - 0.1f, 0);
            }
        }
    }

    // Uses mouse position to calculate the angle of the shoulder and position and angle of the arm
    void DrawArm(Vector3 mousePos) {

        // Set the shoulder target rotation depending on mouse position (0-60 depending on mouse height)
        float shoulderYFactor = Mathf.Clamp(mousePos.y + 4, 0, 9) * 20 / 3; // Y affects shoulder from 0 to 60
        float shoulderXFactor = ((30 - shoulderYFactor) / 30) * Mathf.Clamp(mousePos.x + 3.25f, 0, 11.5f) * 2; // X brings shoulder back to center by up to 23 degrees
        Quaternion shoulderTargetRot = Quaternion.Euler(0, 0, shoulderYFactor + shoulderXFactor);

        // Set the arm taret rotation to face the mouse
        Quaternion armTargetRot = Quaternion.LookRotation(Vector3.forward, mousePos - arm.position) * Quaternion.Euler(0, 0, 90);
        float rotZ = armTargetRot.eulerAngles.z;
        rotZ = rotZ > 180 ? (rotZ < 300 ? 300 : rotZ) : (rotZ > 60 ? 60 : rotZ); // Clamp angles (awkward because 360->0)
        armTargetRot = Quaternion.Euler(0, 0, rotZ);

        // Move the arm and shoulder to their target rotations over time
        shoulder.rotation = Quaternion.Slerp(shoulder.rotation, shoulderTargetRot, Time.deltaTime * 2);
        arm.rotation = Quaternion.Slerp(arm.rotation, armTargetRot, Time.deltaTime * 3);

        // Move the arm so it stays in line with the shoulder
        float shoulderLength = 1.55f;
        float shoulderAngle = shoulder.rotation.z * 120;
        arm.position = new Vector3(shoulder.position.x + (shoulderLength * Mathf.Sin(shoulderAngle / (180f / Mathf.PI))), shoulder.position.y + (shoulderLength * -Mathf.Cos(shoulderAngle / (180f / Mathf.PI))), 0);
    }

    // Returns whether or not the weapon can be used
    bool canUse(int weapon) {
        return level >= weapon && cooldown[weapon] == 0;
    }

    // <?> Cannon Arm
    void Shoot() {

        // Get the position of the barrel on the arm
        float armLength = 1.55f;
        float armAngle = arm.rotation.z * 120 + 90;
        Vector3 barrelPos = new Vector3(arm.position.x + (armLength * Mathf.Sin(armAngle / (180f / Mathf.PI))), arm.position.y + (armLength * -Mathf.Cos(armAngle / (180f / Mathf.PI))), 0);

        // Initialize the bullet
        GameObject b = Instantiate(bullet, barrelPos, arm.rotation);
        b.GetComponent<Bullet>().speed = (level / 3) * 2 + 4;
        //b.GetComponent<SpriteRenderer>().color = lightColours[level];
        b.transform.GetChild(1).GetComponent<Light2D>().color = lightColours[level];

        // Start a volley
        if(volley == 0) volley = 1;

        // See if the volley continues
        if(volley < level) {
            volley += 2;
            volleyTime = volleyCooldown;
        }
        else volley = 0;

        startCooldown(0);
    }

    // Shield Arm
    void Shield(bool isActive) {

        isShielding = isActive;
        arm.GetChild(2).gameObject.SetActive(isActive);

        if(!isActive) startCooldown(1);
    }

    // Open Airlock
    void Airlock() {
        usedWeapon[0] = true;

        StartCoroutine(OpenAirlock());

        startCooldown(2);
    }

    IEnumerator OpenAirlock() {

        // Animate Opening

        // Keep open for 2 Seconds <?>
        airlockOpen = true;
        yield return new WaitForSeconds(2);
        airlockOpen = false;

        // Animate Closing

    }

    // Activate Shockwave
    void Shockwave() {
        
        // Charge the shockwave first
        shockwaveCharge += Time.deltaTime;
        lights[0].pointLightInnerRadius = shockwaveCharge + 0.75f;
        lights[0].pointLightOuterRadius = shockwaveCharge * 3 + 2;
        lights[0].intensity = 1 + shockwaveCharge;
        globalLight.intensity = 1 - shockwaveCharge * 0.25f;

        if(shockwaveCharge > shockwaveMaxCharge) {

            // Create the shockwave
            GameObject s = Instantiate(shockwave, transform.position, Quaternion.identity);
            s.GetComponent<SpriteRenderer>().color = lightColours[level];

            // Reset
            shockwaveCharge = 0;
            currentAction = 0;
            lights[0].pointLightInnerRadius = 0.75f;
            lights[0].pointLightOuterRadius = 2;
            lights[0].intensity = 1;
            startCooldown(3);
        }
    }

    // Activate Laser
    void Laser() {

        if(laserCharge > laserMaxCharge) {

            // Use the laser charge value as a flag, activate laser on first frame only
            if(laserCharge < laserMaxCharge * 2) {
                laserCharge = laserMaxCharge * 2 + 1;
                arm.GetChild(3).gameObject.SetActive(true);
            }

            // Shoot for the duration of the attack
            laserDuration += Time.deltaTime;
            if(laserDuration > laserMaxDuration) {

                // Reset
                laserCharge = 0;
                laserDuration = 0;
                currentAction = 0;
                lights[1].color = lightColours[level];
                lights[1].pointLightInnerRadius = 0.25f;
                lights[1].pointLightOuterRadius = 1;
                lights[1].intensity = 1;

                arm.GetChild(3).gameObject.SetActive(false);

                startCooldown(4);
            }

            // Firing the laser for the duration
            DrawArm(new Vector3(-2, laserDuration * 0.8f - 4, 0));

        }
        else {

            // Charge the laser first
            laserCharge += Time.deltaTime;
            lights[1].color = new Color(1, 0, 0, 1);
            lights[1].pointLightInnerRadius = laserCharge;
            lights[1].pointLightOuterRadius = laserCharge * 3;
            lights[1].intensity = laserCharge;
            globalLight.intensity -= Time.deltaTime * 0.625f;

            for(int i = 2; i < lights.Length; ++i) {
                lights[i].intensity = 1 - laserCharge/4;
            }

            // Move the arm to the bottom until ready to fire
            DrawArm(new Vector3(-2, -4, 0));
        }
    }

    void startCooldown(int weapon) {
        cooldown[weapon] = maxCooldown[weapon];
    }

    void reduceCooldowns() {
        for(int i = 0; i < 5; ++i) {
            cooldown[i] = cooldown[i] - Time.deltaTime <= 0 ? 0 : cooldown[i] - Time.deltaTime;
        }
    }

    // Misc

    public void resetShield() {
        isShieldBroken = false;
        arm.GetChild(2).GetChild(1).GetComponent<Shield>().life = 3;
    }

    public void loseHP(int hp) {
        bossHP -= hp;
        if(bossHP <= 0) {
            GameObject.FindGameObjectWithTag("MainCamera").GetComponent<SceneController>().incState();
        }
    }
}
