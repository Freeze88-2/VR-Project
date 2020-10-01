using UnityEngine;
using UnityEngine.XR;
public class Movement : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private float speed = 100f;
    [SerializeField] private GameObject cam;

    [Header("Controller")]
    [SerializeField] private XRNode inputSource;

    [Header("ArmSwing")]
    [SerializeField] private GameObject leftArm;
    [SerializeField] private GameObject rightArm;
    [SerializeField] private float treshHold = 0.02f;

    private RaycastHit prevHitLeft, prevHitRight;
    private Vector2 inputAxis;
    private InputDevice device;
    private Rigidbody character;
    private bool moving;
    private float stopTimer;

    // Start is called before the first frame update
    void Start()
    {
        character = GetComponent<Rigidbody>();
        device = InputDevices.GetDeviceAtXRNode(inputSource);
    }

    // Update is called once per frame
    void Update()
    {
        device.TryGetFeatureValue(CommonUsages.primary2DAxis, out inputAxis);

        Physics.Raycast(leftArm.transform.position, -Vector3.up, 
            out RaycastHit hit, 100f, LayerMask.GetMask("Ground"));

        Physics.Raycast(rightArm.transform.position, -Vector3.up, 
            out RaycastHit hit2, 100f, LayerMask.GetMask("Ground"));

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
        Vector3 dir = Vector3.zero;

        if (!moving && inputAxis.magnitude > 0)
        {
            dir = cam.transform.forward * inputAxis.y + cam.transform.right * inputAxis.x;
            dir = dir.normalized + character.velocity;
        }
        else if (moving)
        {
            dir = cam.transform.forward.normalized + character.velocity;
        }

        dir.y = 0;
        character.velocity = (dir * speed * Time.fixedDeltaTime);
    }
}
