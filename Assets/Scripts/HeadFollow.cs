using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadFollow : MonoBehaviour {

    [SerializeField]
    protected GameObject target; // An arbitrary target to look at

    [SerializeField]
    protected GameObject defaultTarget; // This is the point in space we look at when nothing else is there

    [SerializeField]
    protected GameObject head; // Corresponds to the avatar's head bone
    [SerializeField]
    protected GameObject eyeL; // Corresponds to the avatar's left eye
    [SerializeField]
    protected GameObject eyeR; // Corresponds to the avatar's right eye

    [SerializeField]
    protected GameObject playerHead; // Corresponds to the HMD

    [SerializeField]
    protected float minDistance; // Minimum distance toward avatar to start looking
    [SerializeField]
    protected float viewAngle;

    [SerializeField]
    protected Vector3 minRot;

    [SerializeField]
    protected Vector3 maxRot;

    [SerializeField]
    protected Vector3 eyeMinRot;

    [SerializeField]
    protected Vector3 eyeMaxRot;

    float focusTimer;

    [SerializeField]
    protected float unfocusTime;
    [SerializeField]
    float unfocusTimer;

    [SerializeField]
    float blinkTimer;
    [SerializeField]
    float blinkTime;
    [SerializeField]
    float blinkDelay; //Amount of time blink should happen before looking

    Animator anim;

    // Use this for initialization
    void Start () {
        playerHead = GameObject.Find("Camera (eye)");
        unfocusTimer = unfocusTime; //Reset unfocus time
        SetTargetRandom(); //Start with a random look direction
        anim = transform.GetChild(0).GetComponent<Animator>();
    }
	
	// Update is called once per frame
	void Update () {
        // Look towards the current target constantly
        Vector3 dir = (target.transform.position - head.transform.position).normalized;
        head.transform.forward = Vector3.Lerp(head.transform.forward, dir, 0.025f);
        head.transform.Rotate(0, 0, -90);
        ClampHeadRotation();

        if (isTargetWithinFOV())
        {
            eyeL.transform.right = dir * -1;
            eyeR.transform.right = dir * -1;
        }
        else
        {
            eyeL.transform.right = head.transform.forward * -1;
            eyeR.transform.right = head.transform.forward * -1;
            SetTargetRandom();
        }

        // Regardless of where you are, if you pick up an item, the avatar will look at it for a while
        if (focusTimer > 0)
        {
            focusTimer -= Time.deltaTime;
            if (focusTimer < 0)
            {
                // After a while the avatar will go back to looking elsewhere
                SetTargetRandom();
            }
        }
        else
        {
            // If within range of avatar, avatar will stare at you
            if (isPlayerWithinRange())
            {
                SetTargetHead();
            }
            // Look in a random direction every so often
            else
            {
                unfocusTimer -= Time.deltaTime;
                if (unfocusTimer < 0)
                {
                    unfocusTimer = unfocusTime;
                    blinkTimer = 0; //Force a blink to happen
                    SetTargetRandom();
                }
            }
        }

        // Blink periodically
        blinkTimer -= Time.deltaTime;
        if (blinkTimer < 0)
        {
            anim.SetTrigger("Blink");
            blinkTimer = blinkTime;
        }
	}

    void ClampHeadRotation()
    {
        Vector3 newRot = head.transform.localEulerAngles;

        newRot.x = ClampAngle(newRot.x, minRot.x, maxRot.x);
        newRot.y = ClampAngle(newRot.y, minRot.y, maxRot.y);
        newRot.z = ClampAngle(newRot.z, minRot.z, maxRot.z);
        
        head.transform.localEulerAngles = newRot;
    }

    /*
     * Helps limit rotations
     * Thanks to users lmapler and Parziphal
     * https://stackoverflow.com/questions/25818897/problems-limiting-object-rotation-with-mathf-clamp
     * */
    public static float ClampAngle(float currentValue, float minAngle, float maxAngle, float clampAroundAngle = 0)
    {
        float angle = currentValue - (clampAroundAngle + 180);

        while (angle < 0)
        {
            angle += 360;
        }

        angle = Mathf.Repeat(angle, 360);

        return Mathf.Clamp(
            angle - 180,
            minAngle,
            maxAngle
        ) + 360 + clampAroundAngle;
    }

    /*
     * Look at in random direction
     * */
    public void SetTargetRandom()
    {
        defaultTarget.transform.position = head.transform.position + Random.insideUnitSphere + transform.forward * 2;
        target = defaultTarget;
    }

    /*
     * Look at the object for an x amount of time
     * 
     * _target -> A gameobject to look at
     * _time -> Amount of time to stare at object for, 0 by default
     * checkDist -> Whether or not we make sure target is within range
     * 
     * */
    public void SetTarget(GameObject _target, float _time = 0, bool checkDist = true)
    {
        if (checkDist)
        {
            if (!isPlayerWithinRange()) return; // Assume we didn't see due to distance
        }
        target = _target;
        if (_time > 0)
        {
            focusTimer = _time;
        }
    }

    public void SetTargetHead()
    {
        target = playerHead;
    }

    /*
     * Reference to [Homework 1 solutions.pdf question 1]
     * */
    bool isTargetWithinFOV()
    {
        Vector3 dir = (target.transform.position - head.transform.position).normalized;
        float theDot = Vector3.Dot(dir, head.transform.forward);
        float referenceVal = Mathf.Cos(Mathf.Deg2Rad * viewAngle / 2);
        if (theDot > referenceVal)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    bool isPlayerWithinRange()
    {
        if (Vector3.Distance(head.transform.position, playerHead.transform.position) < minDistance)
        {
            return true;
        }
        return false;
    }
}
