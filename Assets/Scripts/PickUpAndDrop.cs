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
    
    // for outline
    private RaycastHit hit;
    private Transform _highlight;
    private Outline _outline;

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

        if (_firstPersonMovement.playerGrabbing)
        {
            MoveGrabPoint();
        }
        else
        {
            AddOutLineToPointedBlock();
        }
    }

    private void MoveGrabPoint()
    {
        if (_firstPersonMovement.movingWhileGrab)
        {
            playerGrabPositionTransform.position = _objectToGrab.transform.position;
        }
        else
        {
            Vector2 targetVelocity = new Vector2( Input.GetAxis("Horizontal") * grabPositionMoveSpeed, Input.GetAxis("Vertical") * grabPositionMoveSpeed);
            playerGrabPositionTransform.localPosition += new Vector3(targetVelocity.x * Time.deltaTime, Input.mouseScrollDelta.y * Time.deltaTime, targetVelocity.y * Time.deltaTime);
        }
    }

    private void PickUpOrDrop()
    {
        if (!_objectToGrab)
        {
            Ray ray = new Ray(playerCameraTransform.position, playerCameraTransform.forward);
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
            _firstPersonMovement.movingWhileGrab = false;
            playerGrabPositionTransform.localPosition = _originalGrabPosition;
        }
    }

    private void AddOutLineToPointedBlock()
    {
        Ray ray = new Ray(playerCameraTransform.position, playerCameraTransform.forward);
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.CompareTag("PositionCheckable"))
            {
                if (hit.transform == _highlight) return;
                if (_highlight) _outline.enabled = false;
                
                _highlight = hit.transform;
                _outline = _highlight.GetComponent<Outline>();
                _outline.enabled = true;
            }
            else
            {
                if (_highlight)
                {
                    _highlight = null;
                    _outline.enabled = false;
                    _outline = null;
                }
            }
        }
    }
}
