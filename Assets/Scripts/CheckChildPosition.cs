using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class CheckChildPosition : MonoBehaviour
{
    // put the objective brick collection here
    [SerializeField] private GameObject objective;
    private List<Vector3> _targetPositions;
    private Rigidbody _rb;

    private TransformRetracer _transformRetracer;
    private bool _checkPassed = false;

    [Header("Robot Destruction")] 
    [SerializeField] private GameObject robot;
    [SerializeField] private float timeBeforeDestruction = 5f;
    
    [Header("Skybox Color Change")]
    [SerializeField] private Color skyboxBeginColor = new Color(0f, 0f, 0f);
    [SerializeField] private Color skyboxEndColor = new Color(1f, 0.3568f, 0f);// RGB: 255, 91, 0
    [SerializeField] private float skyboxColorLerpTime = 2f;
    private Material _skyboxMat;
    private Color _prevStartColor;
    private float _prevCompleteness;

    [Header("Music Switch")] 
    [SerializeField] private float musicLerpTime = 3f;
    [SerializeField] private string chaosMusicName = "whitenoise";
    [SerializeField] private string orderMusicName = "EddyCurrents";
    private bool _musicSwitched = false;

    private void Awake()
    {
        _skyboxMat = RenderSettings.skybox;
        _skyboxMat.SetColor("_Top", skyboxBeginColor);
        _prevStartColor = skyboxBeginColor;
        _prevCompleteness = 0f;
        
        // get all target position from the children of objective
        // later use this to check against player bricks
        _targetPositions = new List<Vector3>();
        foreach (Transform child in objective.GetComponentsInChildren<Transform>())
        {
            if (child != objective.transform)
            {
                _targetPositions.Add(child.localPosition);
            }
        }
    }

    private void Start()
    {
        Assert.IsTrue(_targetPositions.Count == transform.parent.childCount-1, $"Level {transform.parent.parent.gameObject.name} have {_targetPositions.Count} target positions but provided {transform.parent.childCount-1} bricks for the player");

        _rb = GetComponent<Rigidbody>();
        _transformRetracer = transform.parent.GetComponent<TransformRetracer>();
        Assert.IsTrue(_transformRetracer, "Need a TransformRetracer component in the brick parent");
        
        // Check brick positions every 0.5 seconds
        InvokeRepeating(nameof(CheckPositions), 2f, 0.5f);
    }

    private void CheckPositions()
    {
        if (_checkPassed) return;
        bool[] positionOccupied = new bool[_targetPositions.Count];
        
        foreach (Transform block in transform.parent.GetComponentsInChildren<Transform>())
        {
            if (block.CompareTag("PositionCheckable"))
            {
                if (block.GetComponent<ObjectGrabbable>().beGrabbed) return;

                Vector3 blockPos = block.localPosition;
                for (int i = 0; i < _targetPositions.Count; i++)
                {
                    if (positionOccupied[i]) continue;
                    if (ManhattanDistance(blockPos, _targetPositions[i]) < 0.07f)
                        positionOccupied[i] = true;
                }
            }
        }
        
        // compute the completeness as a percentage and set skybox color & bgm volume accordingly
        float completeness = 0f;
        foreach (var x in positionOccupied)
        {
            if (x) completeness += 1f;
        }
        completeness /= (float)positionOccupied.Length;
        
        // gradually change the skybox color
        if (completeness != _prevCompleteness)
        {
            StartCoroutine(LerpSkyboxColor(completeness));
            _prevCompleteness = completeness;
        }
        
        // gradually switch between two bgms
        if (completeness > 0 && !_musicSwitched)
        {
            StartCoroutine(LerpMusicVolume());
        }

        if (completeness >= 1f) StartCoroutine(CheckPass());
    }
    
    // The function to call when completeness check have passed, it does four things:
    // 1. make sure the structure is stabilized
    // 2. enable physics on the red static brick
    // 3. Disable unnecessary components on all bricks
    // 4. call TransformRetracer to play robot animation and start restore
    private IEnumerator CheckPass()
    {
        _checkPassed = true;
        // we have to wait for the structure to stabilize before pass
        yield return new WaitForSeconds(1.5f);
        Vector3[] maintainPositions = new Vector3[_targetPositions.Count];
        for (int i = 1; i < transform.parent.childCount; i++)
        {
            Transform child = transform.parent.GetChild(i);
            maintainPositions[i - 1] = child.position;
        }
        yield return new WaitForSeconds(3f);
        for (int i = 1; i < transform.parent.childCount; i++)
        {
            Transform child = transform.parent.GetChild(i);
            if (maintainPositions[i - 1] != child.position)
            {
                _checkPassed = false;
                yield break;
            }
        }
        
        Debug.Log(transform.parent.gameObject.name + " Check Passed");

        _rb.isKinematic = false;
        transform.parent.parent.GetComponent<LevelManager>().enabled = false;
        for (int i = 0; i < _transformRetracer.children.Length; i++)
        {
            Transform child = _transformRetracer.children[i];
            if (child.CompareTag("PositionCheckable"))
            {
                child.tag = "Untagged";
                child.GetComponent<ObjectGrabbable>().enabled = false;
                child.GetComponent<Outline>().enabled = false;
            }
        }
        StartCoroutine(InitiateLoop());
    }

    IEnumerator InitiateLoop()
    {
        yield return new WaitForSeconds(timeBeforeDestruction);
        robot.gameObject.SetActive(true);
        _transformRetracer.RecordOriginalTransform();
        _transformRetracer.CheckPass();
        _skyboxMat.SetColor("_Top", skyboxBeginColor);
        AudioManager.S.Stop(orderMusicName);
        AudioManager.S.SetVolume(chaosMusicName, 1f);
        GameManager.S.LevelComplete(transform.parent.parent.GetComponent<LevelManager>().lvID);
        this.enabled = false;
    }

    private IEnumerator LerpSkyboxColor(float completeness)
    {
        float timeElapsed = 0f;
        Color lerpEndColor = skyboxEndColor * completeness;
        while (timeElapsed <= skyboxColorLerpTime + 0.1f) // add 0.1 here to prevent loop from stopping at timeElapsed = slightly smaller than target
        {
            timeElapsed += Time.deltaTime;
            float t = timeElapsed / skyboxColorLerpTime;
            _skyboxMat.SetColor("_Top", Color.Lerp(_prevStartColor, lerpEndColor, t));
            yield return null;
        }

        _prevStartColor = _skyboxMat.GetColor("_Top");
    }

    private IEnumerator LerpMusicVolume()
    {
        _musicSwitched = true;
        AudioManager.S.Play(orderMusicName);
        
        float timeElapsed = 0f;
        while (timeElapsed <= musicLerpTime + 0.1)
        {
            timeElapsed += Time.deltaTime;
            float t = timeElapsed / musicLerpTime;
            AudioManager.S.SetVolume(orderMusicName, Mathf.Lerp(0f, 1f, t));
            AudioManager.S.SetVolume(chaosMusicName, Mathf.Lerp(0.1f, 1f, 1-t));
            yield return null;
        }
    }
    
    // helper function to get the manhattan distance between two Vector3
    private float ManhattanDistance(Vector3 a, Vector3 b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) + Mathf.Abs(a.z - b.z);
    }
}
