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
        transform.localScale += new Vector3(0.1f, 0.1f, 0.1f);
        if(transform.localScale.x > 23.5f) Destroy(gameObject);
    }
}