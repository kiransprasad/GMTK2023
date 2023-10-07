using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedrunnerController : MonoBehaviour
{

    public Animator animator;

    [SerializeField]
    public BossController boss;
    public LayerMask platformLayerMask;

    new BoxCollider2D collider;

    bool colliding;

    // X-movement
    float speed;
    readonly int maxSpeed = 4;

    // Y-movement
    bool grounded;
    Vector3 groundPos;
    int jumpAnimState;
    float jumpForce;

    // Start is called before the first frame update
    void Start() {

        collider = GetComponent<BoxCollider2D>();

        // X-movement
        speed = 0;

        // Y-movement
        grounded = false;
        Vector3 groundPos = Vector3.zero;
        jumpAnimState = 0;
        jumpForce = 25;
    }

    void Update() {

        #region Animation Data
        animator.SetFloat("grounded", grounded ? 1 : (animator.GetFloat("grounded") == 1 ? -1 : 0));
        animator.SetFloat("xSpeed", Mathf.Abs(speed)/4);
        animator.SetFloat("ySpeed", GetComponent<Rigidbody2D>().velocity.y);
        #endregion

        if(grounded) {
            transform.position = new Vector3(transform.position.x, groundPos.y + collider.bounds.extents.y, 0);
            if(GetComponent<Rigidbody2D>().velocity.y < 0) {
                GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x, 0);
            }
        }

        #region Movement
        Run(Input.GetAxisRaw("Horizontal"));
        if(Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) Jump();
        #endregion

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
    public void Run(float direction) {

        // Accel and Decel
        speed += direction * Time.deltaTime * 25;
        if(direction == 0) speed -= Mathf.Sign(speed) * Time.deltaTime * 15;
        if(Mathf.Abs(speed) < 0.1f && direction == 0) speed = 0;

        // Cap speed
        if(direction == 1 && speed > maxSpeed) {
            speed = maxSpeed;
        }
        else if(direction == -1 && speed < -maxSpeed) {
            speed = -maxSpeed;
        }

        // Move
        transform.localScale = new Vector3(speed == 0 ? transform.localScale.x : -Mathf.Sign(speed), 1, 1);
        transform.position += Vector3.right * speed * Time.deltaTime;
    }

    public void Jump() {

        if(grounded) {
            grounded = false;
            GetComponent<Rigidbody2D>().AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            jumpAnimState = 1;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.gameObject.CompareTag("BossProjectile")) {
            // <?>
            Debug.Log("Hit By: " + collision.gameObject.name);
        }
        if(collision.gameObject.CompareTag("BossLaser")) {
            // <?>
            Debug.Log("Hit By: " + collision.gameObject.name);
        }
    }
}