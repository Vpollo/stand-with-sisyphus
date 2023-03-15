using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager S;

    [SerializeField] private GameObject[] tutorials;
    [SerializeField] private GameObject[] levels;
    [SerializeField] private GameObject lv0ExtraFloor;

    private void Awake()
    {
        if (!S) S = this;
        else Destroy(this.gameObject);

        // for (int i = 1; i < levels.Length; i++)
        // {
        //     levels[i].SetActive(false);
        // }

        foreach (GameObject tutorial in tutorials)
        {
            tutorial.SetActive(false);
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
                break;
            case 2:
                levels[3].SetActive(true);
                break;
        }
    }

    public void ShowTutorial(int tutorialID)
    {
        if (tutorialID >= tutorials.Length) return;
        tutorials[tutorialID].SetActive(true);
    }
}
