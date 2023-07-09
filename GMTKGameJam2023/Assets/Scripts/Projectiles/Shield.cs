using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{

    PlayerController player;
    public int life;

    // Start is called before the first frame update
    void Start()
    {
        player = transform.parent.GetComponentInParent<PlayerController>();

        life = player.level * 2 + 1;
    }

    private void OnCollisionEnter2D(Collision2D collision) {

        if(life <= 0) {

            if(collision.gameObject.CompareTag("Pellet")) {
                Destroy(collision.gameObject);
                life -= 1;

                if(life <= 0) {
                    player.isShieldBroken = true;
                }
            }

            if(collision.gameObject.CompareTag("ChargeShot")) {
                Destroy(collision.gameObject);
                life -= 2;

                if(life <= 0) {
                    player.isShieldBroken = true;
                }
            }

        }
    }
}
