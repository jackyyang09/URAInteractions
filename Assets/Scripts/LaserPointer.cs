using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserPointer : MonoBehaviour {

    [SerializeField]
    public Transform cameraRigTransform;

    [SerializeField]
    public GameObject teleportReticlePrefab;

    GameObject reticle;

    Transform teleportReticleTransform;

    [SerializeField]
    public Transform headTransform;

    [SerializeField]
    public Vector3 teleportReticleOffset;

    [SerializeField]
    public LayerMask teleportMask;

    bool canTeleport;

    private SteamVR_TrackedObject trackedObj;

    [SerializeField]
    private GameObject laser;

    private Vector3 hitPoint;

    Animator anim;

    private SteamVR_Controller.Device Controller
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }

    private void Start()
    {
        reticle = Instantiate(teleportReticlePrefab);
        teleportReticleTransform = reticle.transform;
        anim = transform.GetChild(0).GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update () {
        // If the touchpad is held down…
        if (Controller.GetPress(SteamVR_Controller.ButtonMask.Touchpad))
        {
            anim.SetBool("Thumb Down", true);
            RaycastHit hit;

            // Shoot a ray from the controller.
            // If it hits something, make it store the point where it hit and show the laser.
            if (Physics.Raycast(trackedObj.transform.position, transform.forward, out hit, 100, teleportMask))
            {
                hitPoint = hit.point;
                ShowLaser(hit);
                reticle.SetActive(true);
                teleportReticleTransform.position = hitPoint + teleportReticleOffset;
                canTeleport = true;
            }
        }
        else
        {
            laser.SetActive(false);
            reticle.SetActive(false);
            anim.SetBool("Thumb Down", false);
        }
        if (Controller.GetPressUp(SteamVR_Controller.ButtonMask.Touchpad) && canTeleport)
        {
            Teleport();
        }
    }

    private void ShowLaser(RaycastHit hit)
    {

        laser.SetActive(true);

        // Position the laser between the controller and the point where the raycast hits.
        // You use Lerp because you can give it two positions and the percent it should travel. 
        // If you pass it 0.5f, which is 50%, it returns the precise middle point.
        laser.transform.position = Vector3.Lerp(trackedObj.transform.position, hitPoint, .5f);

        // Point the laser at the position where the raycast hit.
        laser.transform.LookAt(hitPoint);

        // Scale the laser so it fits perfectly between the two positions.
        laser.transform.localScale = new Vector3(laser.transform.localScale.x, laser.transform.localScale.y,
            hit.distance);
    }

    private void Teleport()
    {
        canTeleport = false;
        reticle.SetActive(false);
        //Need a difference to anticipate position of entire rig and the player's position
        Vector3 difference = cameraRigTransform.position - headTransform.position;
        difference.y = 0;
        cameraRigTransform.position = hitPoint + difference;
    }
}
