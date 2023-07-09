using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    public static bool pause = false;

    public GameObject SettingsPanel;

    // Start is called before the first frame update
    void Start() {
        // bewber
    }

    public void Play() {
        SceneManager.LoadScene("Arena1");
    }

    public void Settings() {
        SettingsPanel.SetActive(true);
        pause = true;
    }

    public void CloseSettings() {
        GameObject.FindGameObjectWithTag("SettingsPanel").SetActive(false);
        pause = false;
    }

}
