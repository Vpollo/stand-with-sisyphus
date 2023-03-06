using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public class CheckChildPosition : MonoBehaviour
{
    [SerializeField] private Vector3[] targetPositions;

    private void Start()
    {
        Assert.IsTrue(targetPositions.Length == transform.childCount);
        
        InvokeRepeating(nameof(CheckPositions), 0f, 2f);
    }

    private void CheckPositions()
    {
        bool[] positionOccupied = new bool[targetPositions.Length];
        
        foreach (Transform block in GetComponentsInChildren<Transform>())
        {
            if (block.CompareTag("PositionCheckable"))
            {
                if (block.GetComponent<ObjectGrabbable>().beGrabbed) return;

                Vector3 blockPos = block.localPosition;
                for (int i = 0; i < targetPositions.Length; i++)
                {
                    if (positionOccupied[i]) continue;
                    if (ManhattanDistance(blockPos, targetPositions[i]) < 0.45f)
                        positionOccupied[i] = true;
                }
            }
        }

        if (positionOccupied.All(x => x)) CheckPass();
        else CheckFail();
    }

    private void CheckPass()
    {
        Debug.Log("Check Passed");
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
