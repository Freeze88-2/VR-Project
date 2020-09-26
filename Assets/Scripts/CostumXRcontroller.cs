using System.Collections;
using UnityEngine;
using UnityEngine.XR;

public class CostumXRcontroller : MonoBehaviour
{
    [SerializeField] private readonly XRNode inputSource;
    [SerializeField] private GameObject grabbedObj;
    [SerializeField] private readonly Transform anchorpoint;
    private InputDevice device;
    private float timer;

    // Start is called before the first frame update
    private void Start()
    {
        device = InputDevices.GetDeviceAtXRNode(inputSource);
    }

    private void Update()
    {

    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        UpdateTrackingInput();
    }

    protected void UpdateTrackingInput()
    {
        Vector3 devicePosition = new Vector3();
        Quaternion deviceRotation = new Quaternion();

        if (device.TryGetFeatureValue(CommonUsages.devicePosition, out devicePosition))
        {
            transform.localPosition = devicePosition;
        }

        if (device.TryGetFeatureValue(CommonUsages.deviceRotation, out deviceRotation))
        {
            transform.localRotation = deviceRotation;
        }

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
#endif

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("GameController"))
        {
            if (grabbedObj != null && other.gameObject == grabbedObj)
            {
                grabbedObj = null;
            }
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (grabbedObj != null && other.gameObject == grabbedObj)
        {
            timer++;

            if (timer == 200)
            {
                StartCoroutine(MoveToAnchorPoint());
            }
        }
        else if (other.gameObject.CompareTag("GameController"))
        {
            grabbedObj = other.gameObject;
            timer = 0;
        }
        else if (other.gameObject.CompareTag("Finish") && grabbedObj != null)
        {
            grabbedObj.transform.parent = null;
            grabbedObj.GetComponent<Rigidbody>().isKinematic = false;
            grabbedObj = null;
        }
    }

    private IEnumerator MoveToAnchorPoint()
    {
        Rigidbody rb = grabbedObj.GetComponent<Rigidbody>();

        grabbedObj.transform.parent = transform.transform;
        grabbedObj.GetComponent<Rigidbody>().isKinematic = true;

        while (grabbedObj.transform.position != anchorpoint.position)
        {
            rb.MovePosition(anchorpoint.position);
            yield return null;
        }
    }
}
