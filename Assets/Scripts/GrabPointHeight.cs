using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabPointHeight : MonoBehaviour
{
    [SerializeField] private Transform grabHeight;
    void Update()
    {
        transform.position = new Vector3(transform.position.x, grabHeight.position.y, transform.position.z);
    }
}
