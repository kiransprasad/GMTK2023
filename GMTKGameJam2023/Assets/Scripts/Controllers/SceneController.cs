using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneController : MonoBehaviour
{

    int state;
    float startDelay;

    // Elevator
    GameObject elevator;
    bool elevatorMoving;

    // Dialogue
    public DialogueManager textBox;
    public Dialogue[] levelDialogue;
    bool beginDialogue;
    int dialogueCount;

    // Avatar Gameobjects
    AvatarController mentor;
    AvatarController speedrunner;

    // Mentor Variables
    bool mentorRight;

    // Start is called before the first frame update
    void Start()
    {
        state = -1;
        startDelay = 2;

        beginDialogue = true;
        dialogueCount = 0;

        mentorRight = true;

        elevator = GameObject.FindGameObjectWithTag("Elevator");
        mentor = GameObject.FindGameObjectWithTag("Mentor").GetComponent<AvatarController>();
        speedrunner = GameObject.FindGameObjectWithTag("Speedrunner").GetComponent<AvatarController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(UIController.pause) return;

        if(state == -1) {
            startDelay -= Time.deltaTime;
            if(startDelay < 0) {
                ++state;
            }
        }

        if(state == 0) OpenElevator();

        else if(state == 1) {
            mentor.GetComponent<SpriteRenderer>().sortingOrder = 12;
            state += mentor.Run(3.9f) ? 1 : 0;
        }

        else if(state == 2) CloseElevator();

        else if(state == 3) Speak();

        // <?>
        else if(state == 4) state += MentorTest() ? 1 : 0;

        /*
        else if(state == 5) AvatarRun(mentor, 3.9f, 1);

        else if(state == 6) Speak();

        else if(state == 7) OpenElevator();

        else if(state == 8) AvatarRun(mentor, 0, 1);

        else if(state == 9) {
            mentor.GetComponent<SpriteRenderer>().sortingOrder = -1;
            CloseElevator();
        }
        */

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

    void Speak() {

        if(beginDialogue) {
            textBox.Write(levelDialogue[dialogueCount]);
            beginDialogue = false;
        }

        if(Input.GetKeyDown(KeyCode.Space)) {
            if(!textBox.DisplayNext()) {
                beginDialogue = true;
                ++dialogueCount;
                ++state;
            }
        }

    }

    bool MentorTest() {

        if(mentorRight) {
            if(mentor.Run(6)) {
                mentorRight = false;
            }
        }
        else {
            if(mentor.Run(2)) {
                mentorRight = true;
            }
            else if(mentor.transform.position.x > 4 + Time.deltaTime && mentor.transform.position.x < 4 - Time.deltaTime) {
                Debug.Log("Jump");
                mentor.Jump();
            }
        }

        return mentor.testMechanic();

    }
}
