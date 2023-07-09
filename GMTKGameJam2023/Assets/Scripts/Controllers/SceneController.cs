using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{

    int state;
    int level;
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

    // Start is called before the first frame update
    void Start()
    {
        state = -1;
        startDelay = 2;

        beginDialogue = true;
        dialogueCount = 0;

        elevator = GameObject.FindGameObjectWithTag("Elevator");
        mentor = GameObject.FindGameObjectWithTag("Mentor").GetComponent<AvatarController>();
        speedrunner = GameObject.FindGameObjectWithTag("Speedrunner").GetComponent<AvatarController>();
        level = GameObject.FindGameObjectWithTag("Boss").GetComponent<PlayerController>().level;
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

        else if(state == 4) state += mentor.testMechanic() ? 1 : 0;

        else if(state == 5) Speak();

        //<?>
        else if(state == 6) state += mentor.Run(0) ? 1 : 0;

        else if(state == 7) {
            mentor.GetComponent<SpriteRenderer>().sortingOrder = -1;
            CloseElevator();
            GameObject.FindGameObjectWithTag("Boss").GetComponent<Rigidbody2D>().isKinematic = false;
        }

        else if(state == 8) state += speedrunner.enterRoom() ? 1 : 0;

        else if(state == 9) speedrunner.fightBoss(); // <?> EVENT

        else if(state == 10) state += speedrunner.movePast() ? 1 : 0;

        else if(state == 11) nextScene(); // <?> event

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

    public void incState() {
        ++state;
    }

    void nextScene() {
        if(level == 5) {
            SceneManager.LoadScene("End");
        }
        else {
            SceneManager.LoadScene("Arena" + (level + 2));
        }
    }
}
