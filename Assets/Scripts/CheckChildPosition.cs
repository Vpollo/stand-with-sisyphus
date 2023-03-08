using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public class CheckChildPosition : MonoBehaviour
{
    [SerializeField] private Vector3[] targetPositions;
    private Rigidbody _rb;

    private TransformRetracer _transformRetracer;
    private bool _checkPassed = false;

    private void Start()
    {
        Assert.IsTrue(targetPositions.Length == transform.parent.childCount-1);

        _rb = GetComponent<Rigidbody>();
        _transformRetracer = transform.parent.GetComponent<TransformRetracer>();
        Assert.IsTrue(_transformRetracer);
        
        InvokeRepeating(nameof(CheckPositions), 2f, 2f);
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

        if (positionOccupied.All(x => x)) CheckPass();
        else CheckFail();
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
        yield return new WaitForSeconds(2f);
        _transformRetracer.CheckPass();
    }

    private void CheckFail()
    {
        Debug.Log("Check Fail");
    }

    private float ManhattanDistance(Vector3 a, Vector3 b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) + Mathf.Abs(a.z - b.z);
    }
}
