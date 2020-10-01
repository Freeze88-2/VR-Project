using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowObject : MonoBehaviour
{
    [SerializeField] private GameObject follow;
    [SerializeField] private float yOffset;
    [SerializeField] private bool followY = true;

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = follow.transform.position;
        if (!followY)
        {
            pos.y = transform.position.y;
        }
        else
        {
            pos.y += yOffset;
        }
        transform.position = pos;
        Quaternion rot = follow.transform.rotation;
        rot.x = 0;
        rot.z = 0;
        transform.rotation = rot;
    }
}
