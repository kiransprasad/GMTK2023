using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [SerializeField]
    public Texture2D crosshairs;
    public int level;
    [Header("Parts")]
    public Transform shoulder;
    public Transform arm;
    [Header("Projectiles")]
    public GameObject bullet;

    // Weapon Properties
    // Bullet
    int volley;
    float volleyTime;
    const float volleyCooldown = 0.5f;

    // Cooldowns <?>
    float[] cooldown;
    readonly float[] maxCooldown = { 0, 10, 10, 10, 10 };

    // Start is called before the first frame update
    void Start()
    {
        Cursor.SetCursor(crosshairs, new Vector2(16, 16), CursorMode.Auto);

        volley = 0;
        volleyTime = 0.5f;

        cooldown = new float[5];
    }

    // Update is called once per frame
    void Update() {

        // Graphics
        DrawIdle();
        DrawArm();

        // Inputs for actions
        if(canUse(0) && Input.GetMouseButtonDown(0)) Shoot();
        if(canUse(1) && Input.GetMouseButtonDown(1)) Shield();
        if(canUse(2) && Input.GetKeyDown(KeyCode.Alpha1)) Airlock();
        if(canUse(3) && Input.GetKeyDown(KeyCode.Alpha2)) Shockwave();
        if(canUse(4) && Input.GetKeyDown(KeyCode.Alpha3)) Laser();

        // Volley Logic
        if(volley != 0) {
            volleyTime -= Time.deltaTime;
            if(volleyTime < 0) {
                Shoot();
            }
        }

        // Cooldowns
        reduceCooldowns();
    }

    // <?> Idle animation for boss
    void DrawIdle() {

    }

    // Uses mouse position to calculate the angle of the shoulder and position and angle of the arm
    void DrawArm() {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

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

        cooldown[0] = maxCooldown[0];
    }

    // <?> Shield Arm
    void Shield() {
        Debug.Log("Shield");
        cooldown[1] = maxCooldown[1];
    }

    // <?> Open Airlock
    void Airlock() {
        Debug.Log("Airlock");
        cooldown[2] = maxCooldown[2];
    }

    // <?> Activate Shockwave
    void Shockwave() {
        Debug.Log("Shockwave");
        cooldown[3] = maxCooldown[3];
    }

    // <?> Activate Laser
    void Laser() {
        Debug.Log("Laser");
        cooldown[4] = maxCooldown[4];
    }

    // Reduce Cooldowns
    void reduceCooldowns() {
        for(int i = 0; i < 5; ++i) {
            cooldown[i] = cooldown[i] - Time.deltaTime <= 0 ? 0 : cooldown[i] - Time.deltaTime;
        }
    }
}
