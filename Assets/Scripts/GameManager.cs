using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager S;

    [SerializeField] private GameObject[] tutorials;
    [SerializeField] private GameObject[] levels;
    [SerializeField] private GameObject lv0ExtraFloor;
    [SerializeField] private GameObject titleScreen;
    [SerializeField] private GameObject credit;

    private void Awake()
    {
        if (!S) S = this;
        else Destroy(this.gameObject);

        for (int i = 1; i < levels.Length; i++)
        {
            levels[i].SetActive(false);
        }

        foreach (GameObject tutorial in tutorials)
        {
            tutorial.SetActive(false);
        }
        
        credit.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O) && Input.GetKeyDown(KeyCode.P))
        {
            Scene scene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(scene.name);
        }
    }

    public void LevelComplete(int lvID)
    {
        switch(lvID)
        {
            case 0:
                lv0ExtraFloor.SetActive(false);
                levels[1].SetActive(true);
                ShowTutorial(1);
                break;
            case 1:
                levels[2].SetActive(true);
                titleScreen.SetActive(false);
                ShowTutorial(0, false);
                break;
            case 2:
                levels[3].SetActive(true);
                ShowTutorial(1, false);
                break;
            case 3:
                credit.SetActive(true);
                break;
        }
    }

    public void ShowTutorial(int tutorialID, bool turnOn=true)
    {
        if (tutorialID >= tutorials.Length) return;
        tutorials[tutorialID].SetActive(turnOn);
    }
}
