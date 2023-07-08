using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneController : MonoBehaviour
{

    int state;
    float wait;

    // Elevator
    GameObject elevator;
    bool elevatorMoving;

    // Avatar Gameobjects
    GameObject mentor;

    // Avatar Variables
    int aMoveSpeed;

    // Start is called before the first frame update
    void Start()
    {
        state = -1;
        wait = 2;

        aMoveSpeed = 3;

        elevator = GameObject.FindGameObjectWithTag("Elevator");
        mentor = GameObject.FindGameObjectWithTag("Mentor");
    }

    // Update is called once per frame
    void Update()
    {
        if(UIController.pause) return;

        if(state == -1) {
            wait -= Time.deltaTime;
            if(wait < 0) {
                ++state;
            }
        }
        if(state == 0) {
            OpenElevator();
        }
        else if(state == 1) {
            mentor.GetComponent<SpriteRenderer>().sortingOrder = 12;
            AvatarRun(mentor, 3.9f, 1);
        }
        else if(state == 2) {
            CloseElevator();
        }
        else if(state == 3) {
            //Dialogue();
        }
    }

    void OpenElevator() {

        if(elevator.transform.GetChild(0).position.x > -1) {
            elevator.transform.GetChild(0).position += Vector3.left * Time.deltaTime;
            elevator.transform.GetChild(1).position += Vector3.right * Time.deltaTime;
        }
        else {
            elevator.transform.GetChild(0).position = new Vector3(-1, elevator.transform.GetChild(0).position.y, elevator.transform.GetChild(0).position.z);
            elevator.transform.GetChild(1).position = new Vector3(1.3f, elevator.transform.GetChild(1).position.y, elevator.transform.GetChild(1).position.z);
            ++state;
        }
    }

    void CloseElevator() {

        if(elevator.transform.GetChild(0).position.x < -0.175f) {
            elevator.transform.GetChild(0).position += Vector3.right * Time.deltaTime;
            elevator.transform.GetChild(1).position += Vector3.left * Time.deltaTime;
        }
        else {
            elevator.transform.GetChild(0).position = new Vector3(-0.175f, elevator.transform.GetChild(0).position.y, elevator.transform.GetChild(0).position.z);
            elevator.transform.GetChild(1).position = new Vector3(0.45f, elevator.transform.GetChild(1).position.y, elevator.transform.GetChild(1).position.z);
            ++state;
        }
    }

    void AvatarRun(GameObject avatar, float target, int stateInc = 0) {

        if(avatar.transform.position.x > target + Time.deltaTime) {
            avatar.transform.localScale = new Vector3(1, 1, 1);
            avatar.transform.position += Vector3.left * aMoveSpeed * Time.deltaTime;
        }
        else if(avatar.transform.position.x < target - Time.deltaTime) {
            avatar.transform.localScale = new Vector3(-1, 1, 1);
            avatar.transform.position += Vector3.right * aMoveSpeed * Time.deltaTime;
        }
        else {
            avatar.transform.localScale = new Vector3(1, 1, 1);
            avatar.transform.position = new Vector3(target, avatar.transform.position.y, avatar.transform.position.z);
            state += stateInc;
        }

    }
}
