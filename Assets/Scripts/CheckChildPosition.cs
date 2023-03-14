using System;
using System.Collections;
using System.Linq;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;
using UnityEngine.Assertions;

public class CheckChildPosition : MonoBehaviour
{
    [SerializeField] private Vector3[] targetPositions;
    private Rigidbody _rb;

    private TransformRetracer _transformRetracer;
    private bool _checkPassed = false;

    [SerializeField] private float timeBeforeDestruction = 5f;
    
    [Header("Skybox Color Change")]
    [SerializeField] private Color skyboxBeginColor = new Color(0f, 0f, 0f);
    [SerializeField] private Color skyboxEndColor = new Color(1f, 0.3568f, 0f);// RGB: 255, 91, 0
    [SerializeField] private float skyboxColorLerpTime = 2f;
    private Material _skyboxMat;
    private Color _prevStartColor;
    private float _prevCompleteness;

    private void Awake()
    {
        _skyboxMat = RenderSettings.skybox;
        _skyboxMat.SetColor("_Top", skyboxBeginColor);
        _prevStartColor = skyboxBeginColor;
        _prevCompleteness = 0f;
    }

    private void Start()
    {
        Assert.IsTrue(targetPositions.Length == transform.parent.childCount-1);

        _rb = GetComponent<Rigidbody>();
        _transformRetracer = transform.parent.GetComponent<TransformRetracer>();
        Assert.IsTrue(_transformRetracer);
        
        InvokeRepeating(nameof(CheckPositions), 2f, 0.5f);
    }

    private void CheckPositions()
    {
        if (_checkPassed) return;
        bool[] positionOccupied = new bool[targetPositions.Length];
        
        foreach (Transform block in transform.parent.GetComponentsInChildren<Transform>())
        {
            if (block.CompareTag("PositionCheckable"))
            {
                if (block.GetComponent<ObjectGrabbable>().beGrabbed) return;

                Vector3 blockPos = block.localPosition;
                for (int i = 0; i < targetPositions.Length; i++)
                {
                    if (positionOccupied[i]) continue;
                    if (ManhattanDistance(blockPos, targetPositions[i]) < 0.07f)
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
        if (completeness != _prevCompleteness)
        {
            StartCoroutine(LerpSkyboxColor(completeness));
            _prevCompleteness = completeness;
        }

        if (completeness >= 1f) CheckPass();
    }

    private void CheckPass()
    {
        Debug.Log(transform.parent.gameObject.name + " Check Passed");

        _rb.isKinematic = false;
        _checkPassed = true;
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
        _transformRetracer.CheckPass();
        _skyboxMat.SetColor("_Top", skyboxBeginColor);
        this.enabled = false;
    }

    private void CheckFail()
    {
        Debug.Log("Check Fail");
    }

    IEnumerator LerpSkyboxColor(float completeness)
    {
        Debug.Log(completeness);
        float timeElapsed = 0f;
        while (timeElapsed <= skyboxColorLerpTime)
        {
            timeElapsed += Time.deltaTime;
            float t = (timeElapsed / skyboxColorLerpTime) * completeness;
            _skyboxMat.SetColor("_Top", Color.Lerp(_prevStartColor, skyboxEndColor, t));
            yield return null;
        }

        _prevStartColor = _skyboxMat.GetColor("_Top");
    }

    private float ManhattanDistance(Vector3 a, Vector3 b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) + Mathf.Abs(a.z - b.z);
    }
}
