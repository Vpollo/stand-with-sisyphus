using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectGrabbable : MonoBehaviour
{
    private Rigidbody _rb;
    private bool _beGrabbed = false;
    public bool _rotateStopped = true;
    private Transform _grabPointTransform;
    [SerializeField] private float lerpSpeed = 10f;
    [SerializeField] private float rotationLerpTime = 0.2f;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public void Grab(Transform playerGrabPos)
    {
        _grabPointTransform = playerGrabPos;
        _beGrabbed = true;
        _rb.useGravity = false;
        _rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    public void Drop()
    {
        _beGrabbed = false;
        _rb.useGravity = true;
        _rb.constraints = RigidbodyConstraints.None;
    }

    private void FixedUpdate()
    {
        if (_beGrabbed)
        {
            Vector3 newPos = Vector3.Lerp(
                transform.position, 
                _grabPointTransform.position, 
                Time.deltaTime * lerpSpeed);
            _rb.MovePosition(newPos);
        }
    }

    private void Update()
    {
        // Rotate to one of four campus directions using Q and E keys (y axis)
        // same for z axis using mouse wheel
        if (_beGrabbed && _rotateStopped)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                StartCoroutine(RotateSelf(Vector3.up * -90, rotationLerpTime));
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                StartCoroutine(RotateSelf(Vector3.up * 90, rotationLerpTime));
            }
            else if (Input.mouseScrollDelta.y > 0)
            {
                StartCoroutine(RotateSelf(Vector3.right * 90, rotationLerpTime));
            }
            else if (Input.mouseScrollDelta.y < 0)
            {
                StartCoroutine(RotateSelf(Vector3.right * -90, rotationLerpTime));
            }
        }
    }

    IEnumerator RotateSelf(Vector3 byAngles, float inTime)
    {
        _rotateStopped = false;
        Quaternion fromAngle = transform.rotation;
        Quaternion toAngle = Quaternion.Euler(transform.eulerAngles + byAngles);

        float t = 0f;
        while (t <= 1.1f)
        {
            transform.rotation = Quaternion.Slerp(fromAngle, toAngle, t);
            t += Time.deltaTime / inTime;
            yield return null;
        }
        
        _rotateStopped = true;
    }
}
