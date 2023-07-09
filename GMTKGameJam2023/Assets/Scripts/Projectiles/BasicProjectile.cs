using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicProjectile : MonoBehaviour
{

    public float speed;
    public int damage;
    public PlayerController player;

    // Update is called once per frame
    void Update()
    {
        transform.position += Vector3.left * Time.deltaTime * speed;
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if(collision.gameObject.CompareTag("Boss")) {
            player.loseHP(damage);
            Destroy(gameObject);
        }

        if(collision.gameObject.CompareTag("Walls") || collision.gameObject.CompareTag("Ground")) {
            Destroy(gameObject);
        }
    }
}
