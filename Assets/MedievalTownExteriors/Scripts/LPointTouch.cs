using UnityEngine;
using UnityEngine.UI;

public class LPointTouch : MonoBehaviour
{
    public GameObject controller;
    public Vector3 teleportPosition;
    public GameObject line; // line helps select objects
    public GameObject menu;
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
        menu.SetActive(false);
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


        //if (OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, OVRInput.Controller.LTouch) >= 0.8f)
        if (!teleporting)
        {
            handleSpawningFurniture();
        }

    }

    private void handleSpawningFurniture()
    {
        RaycastHit rayHit;
        if (OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, OVRInput.Controller.LTouch) >= 0.5f)
        {
            pressTimer += Time.deltaTime;
            //enablePointingLine();
        }
        else
        {
            //line.SetActive(false);
            pressTimer = 0.0f;
        }

        if (pressTimer > 2.0f)
        {
            if (menu.activeSelf) menu.SetActive(false);
            else menu.SetActive(true);
            pressTimer = 0.0f;

        }

        // on each subsequent press, navigate to the next one.
        if (OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.LTouch))
        {
            if (buttonInit)
            {
                currentButton.GetComponent<Button>().Select();
                if (menuIndex == 6) menuIndex = 0;
                else menuIndex++; // check if we have initialized the menu before inc menuIndex
            }
            else buttonInit = true;
            currentButton = menu.transform.GetChild(menuIndex).gameObject;
            currentButton.GetComponent<Button>().Select();

            //Debug.LogFormat(currentButton.name);
        }
        // remember the option highlighted so that pressing 'y' will spawn that furniture piece
        if (OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.LTouch) && menu.active)
        {

            // place it in front of the user if not a whiteboard
            if (currentButton.GetComponentInChildren<Text>().text != "whiteboard" && currentButton.GetComponentInChildren<Text>().text != "save")
            {
                GameObject newFurniture = Instantiate(
                        Resources.Load(currentButton.GetComponentInChildren<Text>().text) as GameObject,
                        centerEye.transform.position + centerEye.transform.forward * 3,
                        centerEye.transform.rotation
                    ) as GameObject;
            }
            else if (currentButton.GetComponentInChildren<Text>().text == "save")
            {
                Debug.Log("Saving...");
                save = 0.0f;
            }
            else
            {
                // check if we're are looking at "room" RaycastHit hit;
                Ray myRay = new Ray(centerEye.transform.position, centerEye.transform.forward);
                if (Physics.Raycast(myRay, out rayHit))
                {
                    GameObject selected = rayHit.collider.gameObject;
                    Debug.LogFormat(selected.name);
                    Debug.LogFormat(currentButton.name);

                    if (selected != null && selected.name == "room") // if we hit something
                    {
                        GameObject newFurniture = Instantiate(
                            Resources.Load(currentButton.GetComponentInChildren<Text>().text) as GameObject,
                            centerEye.transform.position + centerEye.transform.forward * 3,
                            centerEye.transform.rotation
                        ) as GameObject;

                    }
                }
                // get the position of the 
            }

        }
    }

    private void handleTeleportation()
    {
        // check if the teleport button is pressed 

        Ray myRay = new Ray(controller.transform.position, controller.transform.forward);
        RaycastHit rayHit;

        if (Physics.Raycast(myRay, out rayHit, Mathf.Infinity))
        {
            if (rayHit.collider.gameObject.name == "Floor" && OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.LTouch) >= 0.8f)
            {
                pressTimer += Time.deltaTime;
                enablePointingLine();
            }
            else
            {
                line.SetActive(false);
                pressTimer = 0.0f;
            }

            if (pressTimer > 2.0f)
            {
                Debug.LogFormat("moving to " + rayHit.point.x + " " + rayHit.point.z);
                teleportPosition = new Vector3(rayHit.point.x, 2.0f, rayHit.point.z);
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