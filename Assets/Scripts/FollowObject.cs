using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowObject : MonoBehaviour
{
    [SerializeField] private GameObject follow;
    [SerializeField] private float yOffset;

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = follow.transform.position;
        pos.y += yOffset;
        transform.position = pos;
    }
}
