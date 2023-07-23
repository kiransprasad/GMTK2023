using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{

    BossController boss;
    public int life;

    // Start is called before the first frame update
    void Start()
    {
        boss = transform.parent.GetComponentInParent<BossController>();

        life = boss.level * 2 + 1;
    }

    private void OnTriggerEnter2D(Collider2D collision) {

        if(collision.gameObject.CompareTag("Pellet")) {
            Destroy(collision.gameObject);
            life -= 1;

            if(life <= 0) {
                boss.isShieldBroken = true;
            }
        }

        if(collision.gameObject.CompareTag("ChargeShot")) {
            Destroy(collision.gameObject);
            life -= 2;

            if(life <= 0) {
                boss.isShieldBroken = true;
            }
        }

    }
}
