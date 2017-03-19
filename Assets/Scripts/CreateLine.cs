using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateLine : MonoBehaviour {

    public bool grabbed;
    public GameObject bucket;

    LineRenderer line;
    GameObject[] chickens;

    // Use this for initialization
    void Start()
    {
        grabbed = false;

        line = gameObject.AddComponent<LineRenderer>();
        line.widthMultiplier = 0.01f;
        line.numPositions = 2;
    }

    // Update is called once per frame
    void Update()
    {
        if (!grabbed)
        {
            chickens = GameObject.FindGameObjectsWithTag("Chicken");
            GameObject closest = null;
            float distance = Mathf.Infinity;
            Vector3 position = transform.position;
            foreach (GameObject c in chickens)
            {
                Vector3 diff = c.transform.position - position;
                float dist = diff.sqrMagnitude;
                if (dist < distance)
                {
                    closest = c;
                    distance = dist;
                }
            }

            line.SetPosition(0, transform.position);
            line.SetPosition(1, closest.transform.position);
        }
        else {
            line.SetPosition(0, transform.position);
            line.SetPosition(1, bucket.transform.position);
        }
    }
}
