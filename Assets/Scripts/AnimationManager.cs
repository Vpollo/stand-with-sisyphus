using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    [SerializeField] private Animator animator;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) animator.SetTrigger("Sweep");
    }
}
