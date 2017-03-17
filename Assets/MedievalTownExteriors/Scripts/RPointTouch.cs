using System.Collections.Generic;
using UnityEngine;

public class RPointTouch : MonoBehaviour
{
    enum selectionType { single, group, none }; // controlt he current state of selection
    selectionType currentSelectionType = selectionType.none; // set the default selection type
    public bool selectState; // Controls the selection type (i.e. ray, or go go, or other method'
    public bool initialAdd;
    public GameObject controller; // handle for controller object
    public GameObject line; // line helps select objects
    public GameObject avatarPos;
    public float speed;

    class ChangedObject
    {
        public Renderer renderer;
        public Color originalColor;
        public GameObject obj;

        public ChangedObject(GameObject gameObject, Renderer renderer, Color color)
        {
            // this.renderer = renderer;            
            //originalColor = renderer.material.color;
            //renderer.material.color = color;
            //Debug.Log(gameObject.name);

            foreach (Transform c in gameObject.transform)
            {
                Renderer rend = c.GetComponent<Renderer>();
                foreach (Material m in rend.materials)
                {
                    m.color = color;
                }
            }
            obj = gameObject;
        }

        public void resetColor()
        {
            GameObject temp = null;
            Vector3 pos = new Vector3(0.0f, -10.0f, 0.0f);

            if (obj.name.Contains("cabinet"))
            {
                temp = Instantiate(Resources.Load("cabinet") as GameObject, pos, Quaternion.identity) as GameObject;
            }

            if (obj.name.Contains("chair"))
            {
                temp = Instantiate(Resources.Load("chair") as GameObject, pos, Quaternion.identity) as GameObject;
            }

            if (obj.name.Contains("desk"))
            {
                temp = Instantiate(Resources.Load("desk") as GameObject, pos, Quaternion.identity) as GameObject;
            }

            if (obj.name.Contains("locker"))
            {
                temp = Instantiate(Resources.Load("locker") as GameObject, pos, Quaternion.identity) as GameObject;
            }

            if (obj.name.Contains("3DTV"))
            {
                temp = Instantiate(Resources.Load("3DTV") as GameObject, pos, Quaternion.identity) as GameObject;
            }

            if (obj.name.Contains("whiteboards"))
            {
                temp = Instantiate(Resources.Load("whiteboards") as GameObject, pos, Quaternion.identity) as GameObject;
            }

            for (int i = 0; i < obj.transform.childCount; i++)
            {
                Material[] c = obj.transform.GetChild(i).GetComponent<Renderer>().materials;
                Material[] t = temp.transform.GetChild(i).GetComponent<Renderer>().materials;

                for (int j = 0; j < c.Length; j++)
                {
                    c[j].color = t[j].color;
                }
            }

            Destroy(temp);

        }

    }

    ChangedObject changedObject;
    RaycastHit rayHit;
    List<ChangedObject> selectedObjects;
    // Use this for initialization
    void Start()
    {
        selectState = false;
        initialAdd = true;
        selectedObjects = new List<ChangedObject>();
        speed = 5.0f;
    }

    // Update is called once per frame
    void Update()
    {
        detectSelectionEnable(); // detects if the player enables selection mode.
        handleObjectManipulation(); // code for highlighting and picking up objects goes here

    }

    /* Helper Methods */
    private void handleObjectManipulation()
    {
        if (selectState)
        {
            enablePointingLine(); // turns on the "flashlight" to help with selecting objects
            Ray myRay = new Ray(controller.transform.position, controller.transform.forward);
            if (Physics.Raycast(myRay, out rayHit))
            {
                GameObject selected;
                Renderer hitRenderer;
                bool lockedOn = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.RTouch) >= 0.5f;

                /*************************/
                /* MOVE MULTIPLE OBJECTS */
                /*************************/
                if (currentSelectionType == selectionType.group)
                {
                    selected = null;
                    if (!lockedOn) /* Change the color of the object */
                    {
                        // highlighted the group of objects
                        selected = rayHit.collider.transform.root.gameObject;
                        hitRenderer = null;
                        if (selected.name == "room" || selected.name == "Floor") return;

                        //Transform[] all = selected.GetComponentsInChildren<Transform>();

                        hitRenderer = selected.GetComponent<Renderer>();
                        ChangedObject newObject = new ChangedObject(selected, hitRenderer, Color.green);
                        //if (selectedObjects[selectedObjects.Count - 1].renderer == hitRenderer) return;
                        if (selectedObjects.Exists(i => (i.obj == selected))) return;
                        else selectedObjects.Add(newObject);

                    }
                    else/* else move the object */
                    {
                        foreach (ChangedObject i in selectedObjects)
                        {
                            if (i.obj.name != "Floor" && i.obj.name != "room")
                            {

                                //Debug.LogFormat("You're moving a group of objects!");\
                                i.obj.transform.root.position = Vector3.MoveTowards(i.obj.transform.root.position, avatarPos.transform.position, speed * Time.deltaTime);
                                //i.obj.transform.root.position = new Vector3(
                                //    rayHit.point.x,
                                //    i.obj.transform.root.position.y,
                                //    rayHit.point.z
                                //);
                                //i.obj.transform.rotation = controller.transform.rotation;
                            }
                        }

                    }
                    /***************************/
                    /* MOVE INDIVIDUAL OBJECTS */
                    /***************************/
                }
                else if (currentSelectionType == selectionType.single)
                {
                    if (!lockedOn) /* Change the color of the object */
                    {
                        // find out what we're looking at
                        selected = rayHit.collider.transform.root.gameObject;
                        hitRenderer = selected.GetComponent<Renderer>();
                        if (selected.name != "Floor" && selected.name != "room")
                        {
                            //Debug.Log(selected.name);
                            if (changedObject != null) // if we haven't hit anything before
                                if (changedObject.obj == selected) return; // if it's the same thing we hit last, don't do anything
                                else changedObject.resetColor();//changedObject.renderer.material.color = changedObject.originalColor; // otherwise, change the color
                            changedObject = new ChangedObject(selected, hitRenderer, Color.blue); // else if null, create a new object
                        }
                    }
                    else/* else move the object */
                    {
                        selected = rayHit.collider.transform.root.gameObject;
                        hitRenderer = selected.GetComponent<Renderer>();
                        if (selected.name != "Floor" && selected.name != "room")
                        {
                            //Debug.LogFormat("You're moving an object!");
                            selected.transform.position = Vector3.MoveTowards(selected.transform.position, avatarPos.transform.position, speed * Time.deltaTime);
                            //selected.transform.position = new Vector3(
                            //    rayHit.point.x,
                            //    selected.transform.root.position.y,
                            //    selected.transform.root.position.z
                            //);
                            //selected.transform.root.rotation = avatarPos.transform.rotation;
                        }
                    }
                }
            }
            //else if (changedObject != null)
            //{
            //    changedObject.renderer.material.color = changedObject.originalColor;
            //    changedObject = null;
            //}
        }
        else
        {
            line.SetActive(false);
        }
    }

    private void detectSelectionEnable()
    {
        // Single selection
        if (OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.RTouch))
        {
            //Debug.LogFormat("Single Selection");
            selectState = !selectState;
            // this means we're turning off selection
            if (currentSelectionType == selectionType.single)
            {
                currentSelectionType = selectionType.none;
                selectState = false;
                changedObject.resetColor();
            }
            else
            {
                currentSelectionType = selectionType.single;
                selectState = true;
            }

        }
        // Group selection
        if (OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.RTouch))
        {
            //Debug.LogFormat("Group Selection");
            // Check if we were already in group select, if so, reset all selected objects.
            if (currentSelectionType == selectionType.group)
            {

                print(selectedObjects.Count);
                foreach (ChangedObject i in selectedObjects)
                {
                    i.resetColor();
                }
                selectedObjects.Clear();
                currentSelectionType = selectionType.none;
                selectState = false;
            }
            else
            {
                selectState = true;
                currentSelectionType = selectionType.group;
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