using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarController : MonoBehaviour
{

    [SerializeField]
    public PlayerController player;

    float moveSpeed;

    // Start is called before the first frame update
    void Start()
    {

        moveSpeed = 4;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Returns a boolean as to whether or not the player reached their destination
    public bool Run(float target, int stateInc = 0) {

        // Moving Left
        if(transform.position.x > target - Time.deltaTime) {
            transform.localScale = new Vector3(1, 1, 1);
            transform.position += Vector3.left * moveSpeed * Time.deltaTime;
            return false;
        }

        // Moving Right
        else if(transform.position.x < target + Time.deltaTime) {
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



    }

    // MENTOR-SPECIFIC

    public bool testMechanic() {

        // Level 0: Hit Bewber with a projectile
        if(player.level == 0) return 


    }

    public bool testMechanic() {

        // Level 0: Hit Bewber with a projectile
        if(player.level == 0) return


    }

}
