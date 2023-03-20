using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public int lvID = 0;
    [SerializeField] private GameObject objectiveBrick;
    [SerializeField] private GameObject tabText;

    private bool _playerPressedFirstTime = false;

    [SerializeField] private FirstPersonMovement firstPersonMovement;
    
    private void Awake()
    {
        objectiveBrick.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Tab) && !firstPersonMovement.playerGrabbing)
        {
            if (!_playerPressedFirstTime)
            {
                _playerPressedFirstTime = true;
                tabText.SetActive(false);
                if (lvID == 0) GameManager.S.ShowTutorial(0);
            }
            objectiveBrick.SetActive(true);
        }
        else
        {
            objectiveBrick.SetActive(false);
        }
    }
}
