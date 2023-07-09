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
    float yVelocity;
    int jumpAnimState;
    float jumpHeight;


    // Start is called before the first frame update
    void Start()
    {

        collider = GetComponent<BoxCollider2D>();

        // X-movement
        moveSpeed = 4;
        isRunning = false;

        // Y-movement
        grounded = false;
        Vector3 groundPos = Vector3.zero;
        yVelocity = 0;
        jumpAnimState = 0;
        jumpHeight = 0.13f;

    }

    void Update()
    {

        // Jump Animation
        if(jumpAnimState != 0) {

            if(jumpAnimState == 1) {

                // Takeoff

                jumpAnimState = 2;
            }
            else if(jumpAnimState == 2) {
                if(yVelocity > 0) {

                    // Up

                }
                else {

                    // Down

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
        }
        else {
            yVelocity -= 0.5f * Time.deltaTime;
            transform.position += new Vector3(0, yVelocity, 0);
        }
    }

    // FixedUpdate updates with the Physics engine
    // Ground check is in here
    private void FixedUpdate() {

        // Raycast Down
        RaycastHit2D ray = Physics2D.Raycast(collider.bounds.center, Vector2.down, collider.bounds.extents.y + 0.1f, platformLayerMask);
        Debug.DrawRay(collider.bounds.center, Vector2.down * (collider.bounds.extents.y + 0.1f));

        if(ray.collider && yVelocity < 0.1f) {
            groundPos = ray.point;
            grounded = true;
            if(jumpAnimState == 2) {
                yVelocity = 0;
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
            yVelocity = jumpHeight;
            jumpAnimState = 1;
        }
    }

    // MENTOR-SPECIFIC

    public bool testMechanic() {

        // Level 0: Hit Bewber with a projectile
        if(player.level == 0) return false;

        else return false;


    }

    public void Training() {
        return;

    }

}
