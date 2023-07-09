using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{

    int Hp;

    // Start is called before the first frame update
    void Start()
    {
        Hp = GameObject.FindGameObjectWithTag("Boss").GetComponent<PlayerController>().bossHP;

        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(Hp);
    }
}
