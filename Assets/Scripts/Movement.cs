using UnityEngine;
using UnityEngine.XR;
public class Movement : MonoBehaviour
{
    [SerializeField] private float speed = 1f;
    [SerializeField] private XRNode inputSource;
    private Vector2 inputAxis;
    private CharacterController character;

    // Start is called before the first frame update
    private void Start()
    {
        character = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    private void Update()
    {
        InputDevice device = InputDevices.GetDeviceAtXRNode(inputSource);
        device.TryGetFeatureValue(CommonUsages.primary2DAxis, out inputAxis);
    }
    private void FixedUpdate()
    {
        Vector3 dir = new Vector3(inputAxis.x, -9, inputAxis.y);

        character.Move(dir * speed * Time.fixedDeltaTime);
    }
}
