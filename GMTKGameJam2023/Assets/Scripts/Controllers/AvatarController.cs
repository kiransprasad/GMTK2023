using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    Vector3 player;

    Vector3 car;

    float dis;

    public GameObject ply;

    public GameObject carr;

    public Run run;

    float y;

    Vector3 dirn;

    Vector3 front;

    float go;

    Vector3 speed;

    public float CarVelocity;

    float x;

    float angle;

    Vector3 vec;

    Vector3 left;

    Vector3 right;

    Vector3 ri;

    bool InCar = false;

    Animator m_Animator;

    float ang;

    void Start()
    {
        run = GameObject.Find("Player").GetComponent<Run>();

        CarVelocity = 10;

        m_Animator = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //Car animation

        Debug.Log(Transform.rotation.z);

        ang = transform.rotation.z;

        m_Animator.SetFloat("Angle", ang);

        //Entering car
        player = GameObject.FindWithTag("Player").transform.position;

        car = transform.position;

        dis = Mathf.Abs(car.magnitude - player.magnitude);

        if (Input.GetKeyDown("e") && dis < 2 && InCar == false)
        {
            ply.transform.parent = carr.transform;

            ply.transform.localPosition = new Vector3(0, 0, 0);

            run.enabled = false;

            InCar = true;

            ply.transform.localScale = new Vector3(0.25f, 0.45f, 1f);

            ply.transform.localRotation = Quaternion.identity;

        }

        if (Input.GetKeyDown("q") && InCar == true)
        {
            ply.transform.localPosition = new Vector3(0, 1, 0);

            ply.transform.parent = null;

            run.enabled = true;

            ply.transform.rotation = Quaternion.Euler(0, 0, 0);

            InCar = false;

            ply.transform.localScale = new Vector3(1, 1, 1);
        }

        //Movement

        if (InCar == true)
        {
            y = Input.GetAxisRaw("Vertical");

            front = GameObject.FindWithTag("front").transform.position;

            dirn = front - transform.position;

            speed = y * dirn * Time.deltaTime * CarVelocity;

            transform.position = transform.position + speed;

            x = Input.GetAxisRaw("Horizontal");

            //turning

            if (y != 0)
            {
                if (x > 0)
                {
                    //vector that is between the center of the car object and the right front of the car
                    right = GameObject.Find("right").transform.position - transform.position;

                    //Use triganometry to get the angle using tan and the direction vectors components then convert to degrees
                    float angle = Mathf.Atan2(right.y, right.x) * Mathf.Rad2Deg;

                    //set the angle of the object in the z
                    transform.rotation = Quaternion.Euler(0, 0, angle);
                }

                if (x < 0)
                {
                    //vector that is between the center of the car object and the right front of the car
                    left = GameObject.Find("left").transform.position - transform.position;

                    //Use triganometry to get the angle using tan and the direction vectors components then convert to degrees
                    float angle1 = Mathf.Atan2(left.y, left.x) * Mathf.Rad2Deg;

                    //set the angle of the object in the z
                    transform.rotation = Quaternion.Euler(0, 0, angle1);
                }
            }

        }

        

    }
}
