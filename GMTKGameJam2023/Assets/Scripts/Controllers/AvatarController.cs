using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarController : MonoBehaviour
{

    [SerializeField]
    public PlayerController player;

    BoxCollider2D collider;
    const float height = 0.1f;

    // X-movement
    float moveSpeed;

    // Y-movement
    bool grounded;
    float yVelocity;


    // Start is called before the first frame update
    void Start()
    {

        collider = GetComponent<BoxCollider2D>();

        // X-movement
        moveSpeed = 4;

        // Y-movement
        grounded = true;
        yVelocity = 0;

    }

    // FixedUpdate updates with the Physics engine
    // Ground check is in here
    private void FixedUpdate() {

        if(Input.GetKeyDown(KeyCode.J)) {
            Jump();
        }

        // Raycast Down
        RaycastHit2D ray = Physics2D.Raycast(collider.bounds.center + new Vector3(0, -1, 0), Vector2.down, collider.bounds.extents.y + 0.1f);

        if(ray.collider && ray.collider.gameObject.tag == "Ground") {
            grounded = true;
            yVelocity = 0;
            //transform.position = new Vector3(transform.position.x, ray.point.y, 0);
        }
        else {
            yVelocity -= 0.1f;
            transform.position += new Vector3(0, yVelocity, 0);
        }
    
    }

    // Returns a boolean as to whether or not the player reached their destination
    public bool Run(float target, int stateInc = 0) {

        // Moving Left
        if(transform.position.x > target + Time.deltaTime) {
            transform.localScale = new Vector3(1, 1, 1);
            transform.position += Vector3.left * moveSpeed * Time.deltaTime;
            return false;
        }

        // Moving Right
        else if(transform.position.x < target - Time.deltaTime) {
            transform.localScale = new Vector3(-1, 1, 1);
            transform.position += Vector3.right * moveSpeed * Time.deltaTime;
            return false;
        }

        // Arrived at destination (should then face left by default)
        else {
            transform.localScale = new Vector3(1, 1, 1);
            transform.position = new Vector3(target, transform.position.y, transform.position.z);
            return true;
        }

    }

    public void Jump() {

        if(grounded) {
            grounded = false;
            yVelocity = 1;
        }
    }

    // MENTOR-SPECIFIC

    public bool testMechanic() {

        // Level 0: Hit Bewber with a projectile
        if(player.level == 0) return true;

        else return false;


    }

    public void Training() {
        return;

    }

}