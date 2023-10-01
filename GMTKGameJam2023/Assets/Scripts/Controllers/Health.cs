using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class health : MonoBehaviour
{
    public GameObject bar;

    Vector3 pos;

    float Hp;

    // Update is called once per frame
    void Update()
    {
        Hp = GameObject.FindGameObjectWithTag("Boss").GetComponent<BossController>().bossHP;

        pos = new Vector3(160-(Mathf.Clamp(Hp, 0, 100)/100)*160, 0, 0);

        bar.transform.localPosition = -(pos);

        bar.transform.localScale = new Vector3(Hp/100, 1, 0);
    }
}


/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{

    int Hp;

    int x;

    Vector3 loss;

  

    // Start is called before the first frame update
    void Start()
    {

        Hp = GameObject.FindGameObjectWithTag("Boss").GetComponent<BossController>().bossHP;

        loss = new Vector3(100 - Hp, 0, 0);

        x = Hp - 100;

        
    }

    // Update is called once per frame
    void Update()
    {
        x = Hp - 100;

        loss = new Vector3(x * 3 + 544, 550, 0);

        Hp = GameObject.FindGameObjectWithTag("Boss").GetComponent<BossController>().bossHP;

        transform.position = loss;

        
    }
}
*/