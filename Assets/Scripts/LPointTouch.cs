using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Oculus.Avatar;
using System;

public class LPointTouch : MonoBehaviour
{
    public GameObject controller;
    public Vector3 teleportPosition;
    public GameObject line; // line helps select objects
    // public GameObject menu;
    public GameObject centerEye;
    public float save;
    float pressTimer;
    bool buttonInit;
    int menuIndex;
    bool teleporting;
    // to handle highlighting of menu items
    class ChangedObject
    {
        public Renderer renderer;
        public Color originalColor;
        public GameObject obj;

        public ChangedObject(GameObject gameObject, Renderer renderer, Color color)
        {
            this.renderer = renderer;
            originalColor = renderer.material.color;
            renderer.material.color = color;
            obj = gameObject;
        }

        public void resetColor()
        {
            renderer.material.color = originalColor;
        }

    }

    ChangedObject changedObject;
    GameObject currentButton;

    // Use this for initialization
    void Start()
    {
        teleporting = false;
        teleportPosition = GameObject.Find("Player").transform.position;
        //menu = GameObject.Find("Canvas").GetComponent<Image>();
        line.SetActive(false);
        // menu.SetActive(false);
        pressTimer = 0.0f;
        buttonInit = false;
        menuIndex = 0;
        save = 1.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.LTouch) >= 0.8f)
        {
            teleporting = true;
            handleTeleportation();
        }
        else
        {
            line.SetActive(false);
            teleporting = false;
        }
    }

    private void handleTeleportation()
    {
        // check if the teleport button is pressed 

        Ray myRay = new Ray(controller.transform.position, controller.transform.forward);
        RaycastHit rayHit;

        if (Physics.Raycast(myRay, out rayHit, Mathf.Infinity))
        {
            if (rayHit.collider.gameObject.name == "Terrain" && OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.LTouch) >= 0.8f)
            {
                pressTimer += Time.deltaTime;
                enablePointingLine();
                // Debug.LogFormat("pointing");
            }
            else
            {
                line.SetActive(false);
                pressTimer = 0.0f;
            }

            if (pressTimer > 2.0f)
            {
                // Debug.LogFormat("moving to " + rayHit.point.x + " " + rayHit.point.z);
                teleportPosition = new Vector3(rayHit.point.x, 13.5f, rayHit.point.z);
                pressTimer = 0.0f;
                line.SetActive(false);
                line.SetActive(true);
            }
        }
    }

    private void enablePointingLine()
    {
        line.SetActive(true);
        /* Cast the selection line from the controller */
        line.transform.position = controller.transform.position;
        line.transform.forward = controller.transform.forward;
    }
}