using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour
{
    [Header("Set in Inspector")]
    [Header("Cannon Controls")]
    public float rotateSpeed = 5f;
    public float maxAngle = 90f;
    public float angleOffset = 90f;
    [Header("Projectile Controls")]
    public float bulletVelocity = 10f;
    public Transform endOfBarrel;
    public GameObject projectilePrefab;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        CannonRotation();
        CannonFire();
    }

    void CannonRotation()
    {
        Vector2 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - angleOffset;
        //Debug.Log(angle);
        if(Mathf.Abs(angle) >= maxAngle)
        {
            if(angle >= -270 && angle < -180)
            {
                angle = maxAngle;
            }
            else
            {
                angle = -maxAngle;
            }
        }
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotateSpeed * Time.deltaTime);
    }

    void CannonFire()
    {
        bool keyF = Input.GetKeyDown(KeyCode.F);
        bool keyD = Input.GetKeyDown(KeyCode.D);
        bool keyS = Input.GetKeyDown(KeyCode.S);

        if(keyF || keyD || keyS){
            // Create new projectile
            GameObject tempGO = Instantiate<GameObject>(projectilePrefab);
            tempGO.transform.position = endOfBarrel.position;
            tempGO.transform.rotation = endOfBarrel.rotation;
            
            // Calculate velocity
            Rigidbody tempRB = tempGO.GetComponent<Rigidbody>();
            Vector3 velo = tempGO.transform.position;
            // Offset cannon position
            velo.y -= 2;
            // Apply velocity
            tempRB.velocity = bulletVelocity * velo;

            if (keyF)
            {
                tempGO.tag = "ProjectileF";
            }
            else if (keyD)
            {
                tempGO.tag = "ProjectileD";
            }
            else if (keyS)
            {
                tempGO.tag = "ProjectileS";
            }
        }
    }
}
