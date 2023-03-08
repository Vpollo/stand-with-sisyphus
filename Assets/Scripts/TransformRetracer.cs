using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformRetracer : MonoBehaviour
{
    public Transform[] children;
    private Vector3[] _originalPositions;
    private Quaternion[] _originalRotations;
    
    [SerializeField] private Animator animator;
    [SerializeField] private float timeBeforeRetrace = 5f;
    [SerializeField] private float retraceTime = 2f;
    [SerializeField] private float infiniteLoopGap = 2f;
    
    private void Awake()
    {
        children = GetComponentsInChildren<Transform>();

        _originalPositions = new Vector3[children.Length];
        _originalRotations = new Quaternion[children.Length];
    }

    public void CheckPass()
    {
        RecordOriginalTransform();
        
        animator.SetTrigger("Sweep");

        StartCoroutine(RestoreStructure());
    }

    private void RecordOriginalTransform()
    {
        for (int i = 0; i < children.Length; i++)
        {
            Transform child = children[i];
            _originalPositions[i] = child.position;
            _originalRotations[i] = child.rotation;
        }
    }

    IEnumerator RestoreStructure()
    {
        yield return new WaitForSeconds(timeBeforeRetrace);
        Debug.Log("start restore");
        for (int i = 1; i < children.Length; i++)
        {
            StartCoroutine(RestoreTransform(i, retraceTime));
        }
        Invoke("CheckPass", retraceTime + infiniteLoopGap + 0.2f);
    }

    IEnumerator RestoreTransform(int childIdx, float lerpTime)
    {
        Transform child = children[childIdx];
        child.GetComponent<Rigidbody>().isKinematic = true;
        
        Vector3 startingPos = child.position;
        Vector3 finalPos = _originalPositions[childIdx];
        Quaternion startAngle = child.rotation;
        Quaternion finalAngle = _originalRotations[childIdx];
        
        float elapsedTime = 0;
        while (elapsedTime < lerpTime + 0.2f)
        {
            child.position = Vector3.Lerp(startingPos, finalPos, (elapsedTime / lerpTime));
            child.rotation = Quaternion.Slerp(startAngle, finalAngle, (elapsedTime / lerpTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        child.GetComponent<Rigidbody>().isKinematic = false;
    }
}
