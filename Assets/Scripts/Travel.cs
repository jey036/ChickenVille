using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Travel : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        LPointTouch Lcontrol = GameObject.Find("controller_left").GetComponent<LPointTouch>();
        transform.position = Lcontrol.teleportPosition;
    }
}
