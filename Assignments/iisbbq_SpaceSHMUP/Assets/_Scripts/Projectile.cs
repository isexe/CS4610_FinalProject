using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private BoundsCheck bndCheck;
    private Renderer rend;

    [Header("Set Dynamically")]
    public Rigidbody rigid;
    [SerializeField]
    private WeaponType _type;

    private float birthTime;
    private GameObject target;
    private float waveFrequency = 1f;
    private bool isHoming = false;
    private float vel;

    // This public prop makes the field _type and takes ation when it is set
    public WeaponType type
    {
        get
        {
            return (_type);
        }
        set
        {
            SetType(value);
        }
    }


    private void Awake()
    {
        bndCheck = GetComponent<BoundsCheck>();
        rend = GetComponent<Renderer>();
        rigid = GetComponent<Rigidbody>();

        birthTime = Time.time;
        vel = Main.GetWeaponDefinition(type).velocity;
    }

    void Update()
    {
        if(bndCheck.offUp)
        {
            Destroy(gameObject);
        }
        if (type == WeaponType.missile){
            if(target != null) Tracking();
            else{
                if(isHoming){
                    rigid.velocity = transform.rotation.eulerAngles.normalized * vel;
                } else{
                    rigid.velocity = Vector3.forward * vel;
                }
            }
            
        }
    }

    public void SetType(WeaponType eType)
    {
        _type = eType;
        WeaponDefinition def = Main.GetWeaponDefinition(_type);
        rend.material.color = def.projectileColor;
    }


    public void InterpolateLeft()
    {
        float age = Time.time - birthTime;

        float theta = Mathf.PI * 3 * age / (waveFrequency);
        float sin = Mathf.Sin(theta);

        Vector3 tempV = new Vector3(-sin, 1, 0);
        tempV.Normalize();
        tempV *= vel;
        rigid.velocity = tempV;

        Invoke("InterpolateLeft", .1f);
    }

    public void InterpolateRight()
    {
        float age = Time.time - birthTime;

        float theta = Mathf.PI * 3 * age / waveFrequency;
        float sin = Mathf.Sin(theta);

        Vector3 tempV = new Vector3(sin, 1, 0);
        tempV.Normalize();
        tempV *= vel;
        rigid.velocity = tempV;

        Invoke("InterpolateRight", .1f);
    }

    public void FindTarget()
    {
        target = null;
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies.Length > 0)
        {
            float closestDistance = Mathf.Infinity;
            foreach (GameObject enemy in enemies)
            {
                float distance = Vector3.Distance(gameObject.transform.position, enemy.transform.position);
                if (distance < closestDistance)
                {
                    target = enemy;
                    closestDistance = distance;
                    isHoming = true;
                }
            }
        }
    }

    public void Tracking()
    {
        rigid.velocity = Vector3.zero;
        //transform.position = Vector3.Lerp(transform.position, target.transform.position, .001f*Main.GetWeaponDefinition(type).velocity);

        // TODO Need to add turning
        // try swapping out lerp with move towards
        float vel = Time.deltaTime * this.vel;
        transform.position = Vector3.MoveTowards(transform.position, target.transform.position, vel);

        // encapsulate the bullet in empty to offset rotation
        transform.LookAt(target.transform);

        ////Failed stuff
        // Vector3 dir = target.transform.position - transform.position;
        // Vector3 vel = dir.normalized * Main.GetWeaponDefinition(type).velocity;
        // rigid.velocity = vel;

        //// https://answers.unity.com/questions/39031/how-can-i-rotate-a-rigid-body-based-on-its-velocit.html
        //transform.rotation = Quaternion.LookRotation(vel);
    }
}
