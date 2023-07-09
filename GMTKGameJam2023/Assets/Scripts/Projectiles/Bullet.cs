using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    public int speed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(UIController.pause) return;

        transform.position += transform.right * speed * Time.deltaTime;
    }


    private void OnCollisionEnter2D(Collision2D collision) {

        if(collision.gameObject.CompareTag("Walls") || collision.gameObject.CompareTag("Ground")) {
            Destroy(gameObject);
        }
    }

}
