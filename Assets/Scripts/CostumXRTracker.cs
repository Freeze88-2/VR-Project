using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class CostumXRTracker : MonoBehaviour
{
    [SerializeField] private XRNode inputSource;
    [SerializeField] private GameObject model;
    public InputDevice inputDevice;

    //[SerializeField]
    //[Tooltip("The input usage that triggers a select, activate or uiInteraction action")]
    //InputHelpers.Button m_SelectUsage = InputHelpers.Button.Grip;
    ///// <summary>Gets or sets the usage to use for detecting selection.</summary>
    //public InputHelpers.Button selectUsage { get { return m_SelectUsage; } set { m_SelectUsage = value; } }

    //[SerializeField]
    //[Tooltip("Gets or sets the usage to use for detecting activation.")]
    //InputHelpers.Button m_ActivateUsage = InputHelpers.Button.Trigger;
    ///// <summary>Gets or sets the usage to use for detecting activation.</summary>
    //public InputHelpers.Button activateUsage { get { return m_ActivateUsage; } set { m_ActivateUsage = value; } }

    //[SerializeField]
    //[Tooltip("Gets or sets the usage to use for detecting a UI press.")]
    //InputHelpers.Button m_UIPressUsage = InputHelpers.Button.Trigger;
    ///// <summary>Gets or sets the usage to use for detecting a UI press.</summary>
    //public InputHelpers.Button uiPressUsage { get { return m_UIPressUsage; } set { m_UIPressUsage = value; } }

    //[SerializeField]
    //[Tooltip("Gets or sets the the amount the axis needs to be pressed to trigger an interaction event.")]
    //float m_AxisToPressThreshold = 0.1f;

    //[SerializeField]
    //[Tooltip("Gets or sets the animation transition to enable when selecting.")]
    //string m_ModelSelectTransition;
    ///// <summary>Gets or sets the animation transition to enable when selecting.</summary>
    //public string modelSelectTransition { get { return m_ModelSelectTransition; } set { m_ModelSelectTransition = value; } }

    //[SerializeField]
    //[Tooltip("Gets or sets the animation transition to enable when de-selecting.")]
    //string m_ModelDeSelectTransition;
    ///// <summary>Gets or sets the animation transition to enable when de-selecting.</summary>
    //public string modelDeSelectTransition { get { return m_ModelDeSelectTransition; } set { m_ModelDeSelectTransition = value; } }


    /// <summary>
    /// InteractionState type to hold current state for a given interaction.
    /// </summary>
    internal struct InteractionState
    {
        /// <summary>This field is true if it is is currently on.</summary>
        public bool active;
        /// <summary>This field is true if the interaction state was activated this frame.</summary>
        public bool activatedThisFrame;
        /// <summary>This field is true if the interaction state was de-activated this frame.</summary>
        public bool deActivatedThisFrame;
    }

    internal enum InteractionTypes { select, activate, uiPress };
    InteractionState m_SelectInteractionState;
    InteractionState m_ActivateInteractionState;
    InteractionState m_UIPressInteractionState;

    // Start is called before the first frame update
    private void Start()
    {
        inputDevice = InputDevices.GetDeviceAtXRNode(inputSource);
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        UpdateTrackingInput();
        //UpdateInput();
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
            if (inputDevice.TryGetFeatureValue(CommonUsages.devicePosition, out devicePosition))
            {
                transform.localPosition = devicePosition;
            }

            if (inputDevice.TryGetFeatureValue(CommonUsages.deviceRotation, out deviceRotation))
            {
                transform.localRotation = deviceRotation;
            }
        }
    }
    //// Override the XRController's controller model's animation (if the prefab contains an animator)
    //internal void UpdateControllerModelAnimation()
    //{
    //    Animator animator = model.GetComponent<Animator>();
    //    if (animator)
    //    {
    //        if (m_SelectInteractionState.activatedThisFrame)
    //            animator.SetTrigger(modelSelectTransition);
    //        else if (m_SelectInteractionState.deActivatedThisFrame)
    //            animator.SetTrigger(modelDeSelectTransition);
    //    }
    //}
    //void UpdateInput()
    //{
    //    // clear state for selection, activation and press state dependent on this frame
    //    m_SelectInteractionState.activatedThisFrame = m_SelectInteractionState.deActivatedThisFrame = false;
    //    m_ActivateInteractionState.activatedThisFrame = m_ActivateInteractionState.deActivatedThisFrame = false;
    //    m_UIPressInteractionState.activatedThisFrame = m_UIPressInteractionState.deActivatedThisFrame = false;

    //    HandleInteractionAction(m_SelectUsage, ref m_SelectInteractionState);
    //    HandleInteractionAction(m_ActivateUsage, ref m_ActivateInteractionState);
    //    HandleInteractionAction(m_UIPressUsage, ref m_UIPressInteractionState);

    //    UpdateControllerModelAnimation();
    //}

    //void HandleInteractionAction(InputHelpers.Button button, ref InteractionState interactionState)
    //{
    //    inputDevice.IsPressed(button, out bool pressed, m_AxisToPressThreshold);

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
}
