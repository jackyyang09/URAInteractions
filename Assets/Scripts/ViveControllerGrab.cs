using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViveControllerGrab : MonoBehaviour {

    // A reference to the object being tracked. In this case, a controller.
    private SteamVR_TrackedObject trackedObj;

    // Stores the GameObject that the trigger is currently colliding with, 
    // so you have the ability to grab the object.
    private GameObject collidingObject;
    // Serves as a reference to the GameObject that the player is currently grabbing.
    private GameObject objectInHand;

    // A Device property to provide easy access to the controller.
    //It uses the tracked object’s index to return the controller’s input.
    private SteamVR_Controller.Device Controller
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    HeadFollow headScript;

    Animator anim;

    void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }

    private void Start()
    {
        headScript = GameObject.Find("Character").GetComponent<HeadFollow>();
        anim = transform.GetChild(0).GetComponent<Animator>();
    }

    private void SetCollidingObject(Collider col)
    {
        if (collidingObject || !col.GetComponent<Rigidbody>())
        {
            return;
        }
        // Assigns the object as a potential grab target.
        collidingObject = col.gameObject;
    }

    // Update is called once per frame
    void Update () {
        if (Controller.GetHairTriggerDown())
        {
            anim.SetBool("Index Down", true);
            if (collidingObject)
            {
                GrabObject();
            }
        }

        if (Controller.GetHairTriggerUp())
        {
            anim.SetBool("Index Down", false);
            if (objectInHand)
            {
                ReleaseObject();
            }
        }

        if (Controller.GetPressDown(SteamVR_Controller.ButtonMask.Grip))
        {
            anim.SetBool("Grip Down", true);
        }

        if (Controller.GetPressUp(SteamVR_Controller.ButtonMask.Grip))
        {
            anim.SetBool("Grip Down", false);
        }
    }

    private void GrabObject()
    {
        // Move the GameObject inside the player’s hand and remove it from the collidingObject variable.
        objectInHand = collidingObject;
        collidingObject = null;
        // Add a new joint that connects the controller to the object using the AddFixedJoint() method below.
        var joint = AddFixedJoint();
        joint.connectedBody = objectInHand.GetComponent<Rigidbody>();
        headScript.SetTarget(objectInHand, 4);
    }

    // Make a new fixed joint, add it to the controller, and then set it up so it doesn’t break easily.
    private FixedJoint AddFixedJoint()
    {
        FixedJoint fx = gameObject.AddComponent<FixedJoint>();
        fx.breakForce = 20000;
        fx.breakTorque = 20000;
        return fx;
    }

    private void ReleaseObject()
    {
        if (GetComponent<FixedJoint>())
        {
            // Remove the connection to the object held by the joint and destroy the joint.
            GetComponent<FixedJoint>().connectedBody = null;
            Destroy(GetComponent<FixedJoint>());
            // Add the speed and rotation of the controller when the player releases the object
            objectInHand.GetComponent<Rigidbody>().velocity = Controller.velocity;
            objectInHand.GetComponent<Rigidbody>().angularVelocity = Controller.angularVelocity;
        }
        objectInHand = null;
    }

    public void OnTriggerEnter(Collider other)
    {
        SetCollidingObject(other);
    }

    public void OnTriggerStay(Collider other)
    {
        SetCollidingObject(other);
    }

    public void OnTriggerExit(Collider other)
    {
        if (!collidingObject)
        {
            return;
        }

        collidingObject = null;
    }
}
