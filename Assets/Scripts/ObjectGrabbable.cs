using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class ObjectGrabbable : MonoBehaviour
{
    private Rigidbody _rb;
    public bool beGrabbed = false;
    public bool _rotateStopped = true;
    private Transform _grabPointTransform;
    private LayerMask _everything = ~0;
    [SerializeField] private float lerpSpeed = 10f;
    [SerializeField] private float rotationLerpTime = 0.2f;
    private LineRenderer _lr;
    private Outline _outline;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _lr = GameObject.Find("ProjectionLine").GetComponent<LineRenderer>();
        _outline = GetComponent<Outline>();
    }

    private void Start()
    {
        _outline.enabled = false;
    }

    public void Grab(Transform playerGrabPos)
    {
        AudioManager.S.Play("pickup");
        _grabPointTransform = playerGrabPos;
        beGrabbed = true;
        _rb.useGravity = false;
        _rb.constraints = RigidbodyConstraints.FreezeRotation;

        _lr.enabled = true;
        SetProjectionLine();
        SnapToCampusDirection();

        _outline.enabled = true;
    }

    public void Drop()
    {
        AudioManager.S.Play("drop");
        beGrabbed = false;
        _rb.useGravity = true;
        _rb.constraints = RigidbodyConstraints.None;

        _lr.enabled = false;
        
        _outline.enabled = false;
    }

    private void FixedUpdate()
    {
        if (beGrabbed)
        {
            // Move to the grab point on player
            Vector3 newPos = Vector3.Lerp(
                transform.position, 
                _grabPointTransform.position, 
                Time.deltaTime * lerpSpeed);
            _rb.MovePosition(newPos);
            
            // update line renderer to draw projected landing point
            SetProjectionLine();
        }
    }

    private void SetProjectionLine()
    {
        _lr.SetPosition(0, transform.position);
        Ray ray = new Ray(transform.position, new Vector3(0, -1, 0));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 50f, _everything))
        {
            _lr.SetPosition(1, hit.point);
        }
    }

    private void Update()
    {
        // Rotate to one of four campus directions using Q and E keys (y axis)
        // same for z axis using mouse wheel
        if (beGrabbed && _rotateStopped)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                StartCoroutine(RotateSelf(Vector3.up * 90, rotationLerpTime));
            }
            if (Input.GetKeyDown(KeyCode.F))
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

    private void SnapToCampusDirection()
    {
        Vector3 rotDiff = Vector3.zero;
        
        float rotY = transform.rotation.eulerAngles.y % 90;
        rotDiff.y = (rotY > 45) ? (90 - rotY) : -rotY;
        
        float rotX = transform.rotation.eulerAngles.x % 90;
        rotDiff.x = (rotX > 45) ? (90 - rotX) : -rotX;
        
        float rotZ = transform.rotation.eulerAngles.z % 90;
        rotDiff.z = (rotZ > 45) ? (90 - rotZ) : -rotZ;
        
        StartCoroutine(RotateSelf(rotDiff, rotationLerpTime));
    }
}
