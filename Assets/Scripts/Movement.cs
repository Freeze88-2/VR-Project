using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.XR;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
public class Movement : MonoBehaviour
{
    [SerializeField] private float speed = 1f;
    [SerializeField] private XRNode inputSource;
    private Vector2 inputAxis;
    private CharacterController character;

    // Start is called before the first frame update
    void Start()
    {
        character = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
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
