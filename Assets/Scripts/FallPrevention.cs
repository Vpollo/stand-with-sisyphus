using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallPrevention : MonoBehaviour
{
    private void Update()
    {
        if (transform.position.y < -25f)
        {
            transform.position = new Vector3(transform.position.x, 25f, transform.position.z);
        }
    }
}
