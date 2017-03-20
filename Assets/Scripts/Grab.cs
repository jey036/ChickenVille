using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grab : MonoBehaviour {
    private GameObject grabbedObject;
    private Quaternion lastRotation, currentRotation;
    public bool grabbing;
    public GameObject currchicken;
    public GameObject menu;
    public OVRInput.Controller controller;    
    public float grabRadius;
    public LayerMask grabMask;

    void GrabObject()
    {
        grabbing = true;
        RaycastHit[] hits;
        
        hits = Physics.SphereCastAll(transform.position, grabRadius, transform.forward, 0f, grabMask);
        if (hits.Length > 0)
        {
            menu.SetActive(true);
            int closestHit = 0;
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].distance < hits[closestHit].distance)
                    closestHit = i;
            }
            Debug.LogFormat("You are trying to grab something.");
            CreateLine grabbed = gameObject.GetComponent<CreateLine>();
            grabbed.grabbed = true;
            grabbedObject = hits[closestHit].transform.gameObject.transform.root.gameObject;
            currchicken = grabbedObject;
            GameObject.Find(grabbedObject.name).GetComponent<HerdSimCore>().enabled = false;
            grabbedObject.GetComponent<Rigidbody>().isKinematic = true;
            grabbedObject.transform.position = transform.position + transform.forward * 3;
            grabbedObject.transform.parent = transform;
        }
        else
        {
            Debug.LogFormat("nothing was close enough.");
        }
    }

    void DropObject()
    {
        grabbing = false;
        menu.SetActive(false);
        CreateLine grabbed = gameObject.GetComponent<CreateLine>();
        grabbed.grabbed = false;
        if (grabbedObject != null)
        {
            grabbedObject.transform.parent = null;
            HerdSimCore sim = GameObject.Find(grabbedObject.name).GetComponent<HerdSimCore>();
            sim._startPosition = transform.position + transform.forward * 3;
            Vector3 pos = transform.position + transform.forward * 3;
            sim._ground = new Vector3(pos.x,10.0f,pos.z);
            GameObject.Find(grabbedObject.name).GetComponent<HerdSimCore>().enabled = true;
            grabbedObject.GetComponent<Rigidbody>().isKinematic = false;
            grabbedObject = null;
        }
    }
	// Use this for initialization
	void Start () {
        menu.SetActive(false);
	}

	// Update is called once per frame
	void Update ()
    {

        if (!grabbing && OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, OVRInput.Controller.RTouch) == 1.0f)
        {
            GrabObject();
        }
        else if(grabbing && OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, OVRInput.Controller.RTouch) < 1.0f) DropObject();

    }
}
