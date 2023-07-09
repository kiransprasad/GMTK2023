using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shockwave : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale += new Vector3(0.05f, 0.05f, 0.05f);
        if(transform.localScale.x > 23.5f) Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision) {

        if(collision.gameObject.CompareTag("Pellet") || collision.gameObject.CompareTag("Fire") || collision.gameObject.CompareTag("ChargeShot")) {
            Destroy(gameObject);
        }

    }

}
