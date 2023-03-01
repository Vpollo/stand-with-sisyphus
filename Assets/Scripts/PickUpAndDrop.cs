using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PickUpAndDrop : MonoBehaviour
{
    [SerializeField] private Transform playerCameraTransform;
    [SerializeField] private float rayCastDistance = 2f;
    [SerializeField] private LayerMask pickUpLayerMask;

    private FirstPersonMovement _firstPersonMovement;
    [SerializeField] private Transform playerGrabPositionTransform;
    [SerializeField] private float grabPositionMoveSpeed = 2.5f;
    private Vector3 _originalGrabPosition;

    private ObjectGrabbable _objectToGrab;

    private void Awake()
    {
        _firstPersonMovement = gameObject.GetComponent<FirstPersonMovement>();
        _originalGrabPosition = playerGrabPositionTransform.localPosition;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            PickUpOrDrop();
        }

        if (_firstPersonMovement.playerGrabbing && Input.GetMouseButton(1))
        {
            MoveGrabPoint();
        }
    }

    private void MoveGrabPoint()
    {
        Vector2 targetVelocity = new Vector2( Input.GetAxis("Horizontal") * grabPositionMoveSpeed, Input.GetAxis("Vertical") * grabPositionMoveSpeed);
        playerGrabPositionTransform.localPosition += new Vector3(targetVelocity.x * Time.deltaTime, 0f, targetVelocity.y * Time.deltaTime);
    }

    private void PickUpOrDrop()
    {
        if (!_objectToGrab)
        {
            Ray ray = new Ray(playerCameraTransform.position, playerCameraTransform.forward);
            RaycastHit hit;
            Debug.DrawRay(playerCameraTransform.position, playerCameraTransform.forward*rayCastDistance, Color.red, 3f);
            if (Physics.Raycast(ray, out hit, rayCastDistance, pickUpLayerMask))
            {
                playerGrabPositionTransform.position = hit.transform.position;
                _objectToGrab =  hit.transform.GetComponent<ObjectGrabbable>();
                _objectToGrab.Grab(playerGrabPositionTransform);
                    
                _firstPersonMovement.playerGrabbing = true;
            }
        }
        else
        {
            _objectToGrab.Drop();
            _objectToGrab = null;
            _firstPersonMovement.playerGrabbing = false;
            playerGrabPositionTransform.localPosition = _originalGrabPosition;
        }
    }
}
