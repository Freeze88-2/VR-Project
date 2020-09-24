using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class CostumXRcontroller : MonoBehaviour
{
    [SerializeField] private XRNode inputSource;
    private InputDevice device;

    // Start is called before the first frame update
    void Start()
    {
        device = InputDevices.GetDeviceAtXRNode(inputSource);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTrackingInput();
    }

    protected void UpdateTrackingInput()
    {
        Vector3 devicePosition = new Vector3();
        Quaternion deviceRotation = new Quaternion();

#if LIH_PRESENT_V1API
            if (m_PoseProvider != null)
            {
                Pose poseProviderPose = new Pose();
                if(m_PoseProvider.TryGetPoseFromProvider(out poseProviderPose))
                {
                    transform.localPosition = poseProviderPose.position;
                    transform.localRotation = poseProviderPose.rotation;
                }
            }
            else
#elif LIH_PRESENT_V2API
            if (m_PoseProvider != null)
            {
                Pose poseProviderPose = new Pose();
                var retFlags = m_PoseProvider.GetPoseFromProvider(out poseProviderPose);
                if ((retFlags & SpatialTracking.PoseDataFlags.Position) > 0)
                {
                    transform.localPosition = poseProviderPose.position;
                }
                if ((retFlags & SpatialTracking.PoseDataFlags.Rotation) > 0)
                { 
                    transform.localRotation = poseProviderPose.rotation;
                }
            }            
            else
#endif
        {
            if (device.TryGetFeatureValue(CommonUsages.devicePosition, out devicePosition))
                transform.localPosition = devicePosition;

            if (device.TryGetFeatureValue(CommonUsages.deviceRotation, out deviceRotation))
                transform.localRotation = deviceRotation;
        }
    }

    //void HandleInteractionAction(XRNode node, InputHelpers.Button button, ref InteractionState interactionState)
    //{
    //    bool pressed = false;
    //    device.IsPressed(button, out pressed, m_AxisToPressThreshold);

    //    if (pressed)
    //    {
    //        if (!interactionState.active)
    //        {
    //            interactionState.activatedThisFrame = true;
    //            interactionState.active = true;
    //        }
    //    }
    //    else
    //    {
    //        if (interactionState.active)
    //        {
    //            interactionState.deActivatedThisFrame = true;
    //            interactionState.active = false;
    //        }
    //    }
    //}
    // Override the XRController's current position and rotation (used for interaction state playback)
    internal void UpdateControllerPose(Vector3 position, Quaternion rotation)
    {
        transform.localPosition = position;
        transform.localRotation = rotation;
    }
}
