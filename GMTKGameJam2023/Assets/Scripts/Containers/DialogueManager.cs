using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public GameObject textBox;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;

    Queue<string> sentences;

    Coroutine coType;

    // Start is called before the first frame update
    void Start()
    {
        sentences = new Queue<string>();
    }

    public void Write(Dialogue d) {

        // Init Textbox
        textBox.SetActive(true);
        nameText.text = d.name;

        // Empty the queue
        sentences.Clear();

        // Add each sentence to the queue
        foreach(string sentence in d.sentences) {
            sentences.Enqueue(sentence);
        }

        DisplayNext();
    }

    // Tries to display the next sentence, and returns true if successful.
    public bool DisplayNext() {
        
        if(sentences.Count == 0) {
            EndDialogue();
            return false;
        }

        string sentence = sentences.Dequeue();
        StopCoroutine(coType);
        coType = StartCoroutine(TypeSentence(sentence));
        return true;
    }

    IEnumerator TypeSentence(string sentence) {

        dialogueText.text = "";

        foreach(char letter in sentence.ToCharArray()) {
            dialogueText.text += letter;
            yield return null;
        }
    }

    public void EndDialogue() {

        textBox.SetActive(false);

    }
}
