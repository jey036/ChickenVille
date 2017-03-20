using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bucket : MonoBehaviour {

    public GameObject grab;
    public int chickenDestroyed;

	// Use this for initialization
	void Start () {
        chickenDestroyed = 0;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Trigger")
        {
            Grab c = grab.GetComponent<Grab>();
            GameObject chicken = c.currchicken;
            Destroy(chicken);
            chickenDestroyed++;
        }
    }
}
