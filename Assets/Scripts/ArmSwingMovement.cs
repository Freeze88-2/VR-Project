using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class ArmSwingMovement : MonoBehaviour
{

    [SerializeField] private GameObject leftArm;
    [SerializeField] private GameObject rightArm;
    [SerializeField] private float treshHold = 0.02f;
    [SerializeField] private GameObject cam;

    [SerializeField] private float speed = 5f;
    private CharacterController character;
    private RaycastHit prevHitLeft, prevHitRight;
    private bool moving;
    private float stopTimer;

    // Start is called before the first frame update
    void Start()
    {
        character = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        RaycastHit hit2;

        Physics.Raycast(leftArm.transform.position, -Vector3.up, out hit, 100f, LayerMask.GetMask("Ground"));
        Physics.Raycast(rightArm.transform.position, -Vector3.up, out hit2, 100f, LayerMask.GetMask("Ground"));

        if (hit.distance > prevHitLeft.distance + treshHold && hit2.distance < prevHitRight.distance + treshHold || 
            hit.distance < prevHitLeft.distance + treshHold && hit2.distance > prevHitRight.distance + treshHold)
        {
            moving = true;
            stopTimer = 0;
        }
        else
        {
            stopTimer++;
            if (stopTimer > 10)
            {
                moving = false;
            }
        }

        prevHitLeft = hit;
        prevHitRight = hit2;
    }

    private void FixedUpdate()
    {
        if (moving)
        {
            character.Move(cam.transform.forward.normalized * speed * Time.fixedDeltaTime);
        }
    }
}
