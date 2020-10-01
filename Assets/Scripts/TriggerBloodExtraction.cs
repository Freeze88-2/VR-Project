using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerBloodExtraction : MonoBehaviour
{
    [SerializeField] private MoveToPosition bloodDraw;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            bloodDraw.lockOnArm = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            bloodDraw.lockOnArm = false;
        }
    }
}
