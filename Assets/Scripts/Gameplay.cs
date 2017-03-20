using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Gameplay : MonoBehaviour {

    public Dropdown menu;
    public Text status;
    public bool sickness = false;
    private int lastValue;
    private int currValue;

	// Use this for initialization
	void Start () {
        lastValue = menu.value;
        status.text = "Your chickens are flapping in excitement!";
    }

    // Update is called once per frame
    void Update () {
        currValue = menu.value;
        Debug.Log(currValue);

        if (currValue != lastValue) {
            if (currValue == 0) {
                sickness = false;
                status.text = "Your chickens are flapping in excitement!";
            }

            if (currValue == 1)
            {
                sickness = true;
                status.text = "Wait! Watch your step! It seems your chickens have influenza.";
            }

            if (currValue == 2) {
                // Application.LoadLevel(Application.loadedLevel);
                object[] obj = GameObject.FindObjectsOfType(typeof(GameObject));
                foreach (GameObject o in obj)
                {
                    if (o.tag == "Chicken") {
                        HerdSimCore sim = o.GetComponent<HerdSimCore>();
                        sim.Death();
                        o.GetComponent<AudioSource>().Stop();
                    }
                }
                status.text = "Oh no! There's a plague in the farm. Looks like all your chickens are dead.";
            }

            if (currValue == 3) {
                Application.LoadLevel(Application.loadedLevel);
            }

            lastValue = currValue;
        }
    }
}
