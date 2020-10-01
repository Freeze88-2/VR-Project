using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToPosition : MonoBehaviour
{
    [SerializeField] private GameObject Place;
    [SerializeField] private float speed = 1f;
    [SerializeField] private Animator anim;
    private Rigidbody rb;
    private Vector3 originalPos;
    public bool lockOnArm;
    // Start is called before the first frame update
    void Start()
    {
        originalPos = transform.position;
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (lockOnArm)
        {
            Vector3 dir = Place.transform.position - transform.position;
            rb.velocity = ((dir * speed) * Time.fixedDeltaTime);

            if(Vector3.Distance(transform.position, Place.transform.position) < 0.1f)
            {
                anim.enabled = true;
            }
        }
        else
        {
            Vector3 dir = originalPos - transform.position;
            rb.velocity = ((dir * speed) * Time.fixedDeltaTime);
        }
    }
}
