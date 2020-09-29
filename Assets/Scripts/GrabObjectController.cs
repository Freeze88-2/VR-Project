using System.Collections;
using UnityEngine;

public class GrabObjectController : MonoBehaviour
{
    [SerializeField] public GameObject grabbedObj;
    [SerializeField] public GrabObjectController otherHand;
    [SerializeField] private Transform anchorpoint;
    private float timer;
    private bool canDrop;

    private void OnTriggerExit(Collider other)
    {
        if (grabbedObj != null && other.gameObject == grabbedObj)
        {
            StopCoroutine(MoveToAnchorPoint());
            grabbedObj.transform.parent = null;
            grabbedObj = null;
        }
        else if (other.CompareTag("Finish"))
        {
            StartCoroutine(SetCanDrop());
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Finish") && grabbedObj == null)
        {
            canDrop = false;
        }

        if (grabbedObj != null && other.gameObject == grabbedObj)
        {
            timer++;

            if (timer == 50)
            {
                StartCoroutine(MoveToAnchorPoint());
            }
        }
        else if (other.gameObject.CompareTag("GameController") && grabbedObj == null)
        {
            if (otherHand.grabbedObj != other.gameObject)
            {
                grabbedObj = other.gameObject;
                timer = 0;
            }
        }
        else if (canDrop && other.gameObject.CompareTag("Finish") && grabbedObj != null)
        {
            StopCoroutine(MoveToAnchorPoint());
            grabbedObj.GetComponent<Rigidbody>().isKinematic = false;
            grabbedObj.transform.parent = null;
            grabbedObj = null;
        }
    }
    private IEnumerator SetCanDrop()
    {
        yield return new WaitForSeconds(0.1f);
        canDrop = true;
    }
    private IEnumerator MoveToAnchorPoint()
    {
        Rigidbody rb = grabbedObj.GetComponent<Rigidbody>();

        grabbedObj.GetComponent<Rigidbody>().isKinematic = true;

        while (grabbedObj != null && Vector3.Distance(grabbedObj.transform.position, anchorpoint.position) > 0.05f)
        {
            rb.MovePosition(anchorpoint.position);
            yield return null;
        }

        grabbedObj.transform.parent = transform.transform;
    }
}
