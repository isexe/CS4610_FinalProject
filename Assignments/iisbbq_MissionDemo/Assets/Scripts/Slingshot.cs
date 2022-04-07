using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slingshot : MonoBehaviour
{
    static private Slingshot S;

    [Header("Set in Inspector")]
    public  GameObject  prefabProjectile;
    public  float       velocityMult = 8f;

    [Header("Set Dynamically")]
    public  GameObject  launchPoint;
    public  Vector3     launchPos;
    public  GameObject  projectile;
    public  bool        aimingMode;

    private Rigidbody   projectileRigidbody;

    public GameObject leftAnchor;
    public GameObject rightAnchor;
    private LineRenderer rubberband;

    static public Vector3 LAUNCH_POS
    {
        get
        {
            if (S == null) return Vector3.zero;
            return S.launchPos;
        }
    }

    void Awake() {
        S = this;

        //Finds the LaunchPoint GO that is a child of attached game object, returns its transform
        Transform launchPointTrans = transform.Find("LaunchPoint");
        
        //Sets found GO and deactivates it
        launchPoint = launchPointTrans.gameObject;
        launchPoint.SetActive(false);

        //Sets launchPos to found GO position
        launchPos = launchPointTrans.position;

        //Sets up the rubberband anchors
        leftAnchor = transform.Find("LeftAnchor").gameObject;
        rightAnchor = transform.Find("RightAnchor").gameObject;

        rubberband = gameObject.GetComponentInChildren<LineRenderer>();
        SetupRubberband();
    }

    void SetupRubberband()
    {
        rubberband.positionCount = 3;
        rubberband.SetPosition(0, rightAnchor.transform.position);
        rubberband.SetPosition(2, leftAnchor.transform.position);

        Vector3 tempMid = rightAnchor.transform.position + leftAnchor.transform.position;
        tempMid *= .5f;

        rubberband.SetPosition(1, tempMid);

        rubberband.enabled = true;
    }

    void OnMouseEnter() {
        // print("Slingshot:OnMouseEnter()");

        //Activates LaunchPoint GO
        launchPoint.SetActive(true);
    }

    void OnMouseExit() {
        // print("Slingshot:OnMouseExit()");

        //Deactivates LaunchPoint GO if not aiming
        launchPoint.SetActive(false);
    }

    void OnMouseDown() {
        //The player has pressed the mouse button while in launch zone
        aimingMode = true;
        //Instantiates a Projectile
        projectile = Instantiate(prefabProjectile) as GameObject;
        //Start it at the launchPoint
        projectile.transform.position = launchPos;
        //Set it to isKinematic for now
        projectileRigidbody = projectile.GetComponent<Rigidbody>();
        projectileRigidbody.isKinematic = true;
    }

    void Update() {
        //If the slingshot isn't aiming don't run Update()
        if(!aimingMode) return;

        //Gets the mouse position in 2D screen coordinates and converts them to 3D coordinates
        Vector3 mousePos2D = Input.mousePosition;
        mousePos2D.z = -Camera.main.transform.position.z;
        Vector3 mousePos3D = Camera.main.ScreenToWorldPoint(mousePos2D);

        //Finds the delta from the launchPos to the mousePos3D
        Vector3 mouseDelta = mousePos3D - launchPos;

        //Limits mouseDelta to the radius of the Slingshot SphereCollider
        float maxMagnitude = this.GetComponent<SphereCollider>().radius;
        if(mouseDelta.magnitude > maxMagnitude){
            mouseDelta.Normalize();
            mouseDelta *= maxMagnitude;
        }

        //Move the projectile to this new position
        Vector3 projPos = launchPos + mouseDelta;
        projectile.transform.position = projPos;

        //Adjusts the rubber band to match projectile position
        rubberband.SetPosition(1, projPos);

        //Checks if mouse button is released
        if (Input.GetMouseButtonUp(0)){
            aimingMode = false;
            projectileRigidbody.isKinematic = false;
            projectileRigidbody.velocity = -mouseDelta * velocityMult;
            FollowCam.POI = projectile;
            projectile = null;
            MissionDemolition.ShotFired();  // Updates the GUI in MissionDemolition.cs
            ProjectileLine.S.poi = projectile;

            SetupRubberband();
        }
    }
}
