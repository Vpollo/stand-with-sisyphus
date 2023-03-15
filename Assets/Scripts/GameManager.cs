using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager S;

    [SerializeField] private GameObject[] tutorials;

    private void Awake()
    {
        if (!S) S = this;
        else Destroy(this.gameObject);
    }

    public void LevelComplete(int lvID)
    {
        switch(lvID)
        {
            case 0:
                break;
        }
    }

    public void ShowTutorial(int tutorialID)
    {
        if (tutorialID >= tutorials.Length) return;
        tutorials[tutorialID].SetActive(true);
    }
}
