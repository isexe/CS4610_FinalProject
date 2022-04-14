using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour
{
    static public Hero S; // Singleton

    [Header("Set in Inspector")]
    // These fields control the movement of the ship
    public float speed = 30;
    public float rollMult = -45;
    public float pitchMult = 30;

    // Add game restart delay in public set in header section
    public float gameRestartDelay = 2f;
    public GameObject projectilePrefab;
    public float projectileSpeed = 40;
    public Weapon[] weapons;

    // [Header("Set Dynamically")]
    // public float shieldLevel = 1;

    // Replace public float shieldLevel with private float _shieldLevel = 1;
    [SerializeField]
    private float _shieldLevel = 1;

    // This variable holds a reference to the last triggering GameObject
    private GameObject lastTriggerGO = null;

    // Declare a new delegate
    public delegate void WeaponFireDelegate();
    // Create a WeaponFireDelegate filed named fireDelegate
    public WeaponFireDelegate fireDelegate;

    // private variable to charge weapon
    private float chargeTime = 0;

    public float shieldLevel
    {
        get
        {
            return (_shieldLevel);
        }
        set
        {
            _shieldLevel = Mathf.Min(value, 4); // Sets shield to value less than or equal to 4
            if(value < 0)                                   // If shield is less than 0
            {
                Destroy(this.gameObject);                   // Destroys object
                Main.S.DelayedRestart(gameRestartDelay);    // and restarts the game
            }
        }
    }

    void Start()
    {
        if (S == null)
        {
            S = this; // Set the Singleton
        }
        else
        {
            Debug.LogError("Hero.Awake() - Attempted to assign second Hero.S!");
        }

        ClearWeapons();
        weapons[0].SetType(WeaponType.blaster);
    }

    void Update()
    {
        // Pull in information from the Input class
        float xAxis = Input.GetAxis("Horizontal");
        float yAxis = Input.GetAxis("Vertical");

        // Change transform.position based on the axes
        Vector3 pos = transform.position;
        pos.x += xAxis * speed * Time.deltaTime;
        pos.y += yAxis * speed * Time.deltaTime;
        transform.position = pos;

        // Rotate the ship to make it feel more dynamic
        transform.rotation = Quaternion.Euler(yAxis * pitchMult, xAxis * rollMult, 0);

        if(Input.GetAxis("Jump") == 1 && fireDelegate != null && !Main.S.isFiring)
        {
            chargeTime += Time.deltaTime;
            if (chargeTime > Main.GetWeaponDefinition(weapons[0].type).chargeTime)
            {
                chargeTime = 0;
                fireDelegate();
            }
        }
        else
        {
            chargeTime = 0;
        }
    }

    // From page 233
    void OnTriggerEnter(Collider other)
    {
        Transform rootT = other.gameObject.transform.root;
        GameObject go = rootT.gameObject;

        // Make sure it's a new object
        if (go == lastTriggerGO)
        {
            return;
        }
        lastTriggerGO = go;

        if (go.tag == "Enemy")  // If the shield was triggered by an enemy
        {
            shieldLevel--;      // Decrement the shield level
            Destroy(go);        // and destroy the enemy
        } else if (go.tag == "PowerUp") {
            AbsorbPowerUp(go);
        }
        else
        {
            print("Trigggered by non-Enemy: " + go.name);
        }
    }

    public void AbsorbPowerUp(GameObject go)
    {
        PowerUp pu = go.GetComponent<PowerUp>();
        switch (pu.type)
        {
            case WeaponType.shield:
                shieldLevel++;
                break;

            default:
                if(pu.type == weapons[0].type)
                {
                    Weapon w = GetEmptyWeaponSlot();
                    if(w!= null)
                    {
                        w.SetType(pu.type);
                    }
                }
                else
                {
                    ClearWeapons();
                    weapons[0].SetType(pu.type);
                }
                break;
        }
        pu.AbsorbedBy(this.gameObject);
    }

    Weapon GetEmptyWeaponSlot()
    {
        for(int i=0; i<weapons.Length; i++)
        {
            if (weapons[i].type == WeaponType.none)
            {
                return weapons[i];
            }
        }
        return (null);
    }

    void ClearWeapons()
    {
        foreach (Weapon w in weapons)
        {
            w.SetType(WeaponType.none);
        }
    }
}
