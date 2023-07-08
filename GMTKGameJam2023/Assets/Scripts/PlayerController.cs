using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerController : MonoBehaviour
{

    [SerializeField]
    public Texture2D crosshairs;
    public int level;
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

    readonly Color[] lightColours = {
        Color.yellow,
        new Color(1, 0.5f, 0),
        Color.magenta,
        new Color(0.9f, 0.25f, 0),
        Color.blue,
        Color.red
    };

    // Weapon Properties
    // Bullet
    int volley;
    float volleyTime;
    const float volleyCooldown = 0.3f;

    // Shield
    bool isShielding;

    // Cooldowns <?>
    float[] cooldown;
    readonly float[] maxCooldown = { 0, 10, 10, 10, 10 };

    // Animation
    int animState;
    float idleBodyY, idleShoulderY;

    // Start is called before the first frame update
    void Start()
    {
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
        transform.GetChild(2).GetChild(2).GetChild(1).GetComponent<Light2D>().color = lightColours[level];

        volley = 0;
        volleyTime = 0.5f;

        isShielding = false;

        cooldown = new float[5];

        animState = 0;
        idleBodyY = body.position.y;
        idleShoulderY = shoulder.position.y;
    }

    // Update is called once per frame
    void Update() {

        if(UIController.pause) return;

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
        if(canUse(1) && Input.GetMouseButtonDown(1)) Shield(true);
        if(isShielding && Input.GetMouseButtonUp(1)) Shield(false);

        if(canUse(2) && Input.GetKeyDown(KeyCode.Alpha1)) Airlock();
        if(canUse(3) && Input.GetKeyDown(KeyCode.Alpha2)) Shockwave();
        if(canUse(4) && Input.GetKeyDown(KeyCode.Alpha3)) Laser();

        // Cooldowns
        reduceCooldowns();
    }

    // <?> Idle animation for boss
    void Animate(int state) {
        if(state == 0) {
            if(Time.time % 1.5f < 0.375f) {
                body.position = new Vector3(body.position.x, idleBodyY, 0);
                shoulder.position = new Vector3(shoulder.position.x, idleShoulderY, 0);
            }
            else if(Time.time % 1.5f < 0.75f) {
                body.position = new Vector3(body.position.x, idleBodyY - 0.1f, 0);
                shoulder.position = new Vector3(shoulder.position.x, idleShoulderY, 0);
            }
            else if(Time.time % 1.5f < 1.125f) {
                body.position = new Vector3(body.position.x, idleBodyY - 0.1f, 0);
                shoulder.position = new Vector3(shoulder.position.x, idleShoulderY - 0.1f, 0);
            }
            else {
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

    // <?> Shield Arm
    void Shield(bool isActive) {

        isShielding = isActive;
        transform.GetChild(2).GetChild(2).gameObject.SetActive(isActive);

        if(!isActive) startCooldown(1);
    }

    // <?> Open Airlock
    void Airlock() {
        Debug.Log("Airlock");
        startCooldown(2);
    }

    // <?> Activate Shockwave
    void Shockwave() {
        Debug.Log("Shockwave");
        startCooldown(3);
    }

    // <?> Activate Laser
    void Laser() {
        Debug.Log("Laser");
        startCooldown(4);
    }

    void startCooldown(int weapon) {
        cooldown[weapon] = maxCooldown[weapon];
    }

    void reduceCooldowns() {
        for(int i = 0; i < 5; ++i) {
            cooldown[i] = cooldown[i] - Time.deltaTime <= 0 ? 0 : cooldown[i] - Time.deltaTime;
        }
    }
}
