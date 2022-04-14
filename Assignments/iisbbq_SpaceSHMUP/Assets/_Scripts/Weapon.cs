using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType
{
    none,       // default
    blaster,    // a simple blaster
    spread,     // two shots at a time
    phaser,     // TODO// shots that move in waves
    missile,    // TODO homing missiles
    laser,      // TODO// DoT
    shield      // raise shield level
}

[System.Serializable]
public class WeaponDefinition
{
    public WeaponType type = WeaponType.none;
    public string letter;
    public Color color = Color.white;
    public GameObject projectilePrefab;
    public Color projectileColor = Color.white;
    public float damageOnHit = 0;
    public float continousDamage = 0;
    public float delayBetweenShots = 0;
    public float velocity = 20;
    public float chargeTime = 0;
}

public class Weapon : MonoBehaviour
{
    static public Transform PROJECTILE_ANCHOR;

    [Header("Set Dynamically")] [SerializeField]
    private WeaponType _type = WeaponType.none;
    public WeaponDefinition def;
    public GameObject collar;
    public float lastShotTime;

    private Renderer collarRend;

    private LineRenderer laser;
    private bool laserFiring;

    // Start is called before the first frame update
    void Start()
    {
        laser = gameObject.GetComponent<LineRenderer>();

        collar = transform.Find("Collar").gameObject;
        collarRend = collar.GetComponent<Renderer>();

        SetType(_type);

        if (PROJECTILE_ANCHOR == null)
        {
            GameObject go = new GameObject("_ProjectileAnchor");
            PROJECTILE_ANCHOR = go.transform;
        }

        GameObject rootGO = transform.root.gameObject;
        if(rootGO.GetComponent<Hero>() != null)
        {
            rootGO.GetComponent<Hero>().fireDelegate += Fire;
        }
    }

    public WeaponType type
    {
        get { return (_type); }
        set { SetType(value); }
    }

    public void SetType(WeaponType wt)
    {
        _type = wt;
        if(type == WeaponType.none)
        {
            this.gameObject.SetActive(false);
            return;
        }
        else
        {
            this.gameObject.SetActive(true);
        }
        def = Main.GetWeaponDefinition(_type);
        collarRend.material.color = def.color;
        lastShotTime = 0;
    }

    public void Fire()
    {
        if (!gameObject.activeInHierarchy) return;
        if (Time.time - lastShotTime < def.delayBetweenShots) return;
        Projectile p;
        Vector3 vel = Vector3.up * def.velocity;
        if(transform.up.y < 0)
        {
            vel.y = -vel.y;
        }
        switch (type)
        {
            case WeaponType.blaster:
                p = MakeProjectile();
                p.rigid.velocity = vel;
                break;

            case WeaponType.spread:
                p = MakeProjectile();
                p.rigid.velocity = vel;
                p = MakeProjectile();
                p.transform.rotation = Quaternion.AngleAxis(10, Vector3.back);
                p.rigid.velocity = p.transform.rotation * vel;
                p = MakeProjectile();
                p.transform.rotation = Quaternion.AngleAxis(-10, Vector3.back);
                p.rigid.velocity = p.transform.rotation * vel;
                break;

            // TODO// implement phaser firing
            case WeaponType.phaser:
                p = MakeProjectile();
                p.InterpolateLeft();
                p = MakeProjectile();
                p.InterpolateRight();
                break;

            // TODO implement laser firing
            case WeaponType.laser:
                StartLaser();
                break;

            // TODO implement missile firing
            case WeaponType.missile:
                p = MakeProjectile();
                p.FindTarget();
                p.rigid.velocity = vel;
                break;
        }
    }

    public Projectile MakeProjectile()
    {
        GameObject go = Instantiate<GameObject>(def.projectilePrefab);
        if (transform.parent.gameObject.tag == "Hero")
        {
            go.tag = "ProjectileHero";
            go.layer = LayerMask.NameToLayer("ProjectileHero");
        }
        else
        {
            go.tag = "ProjectileEnemy";
            go.layer = LayerMask.NameToLayer("ProjectileEnemy");
        }
        go.transform.position = collar.transform.position;
        go.transform.SetParent(PROJECTILE_ANCHOR, true);
        Projectile p = go.GetComponent<Projectile>();
        p.type = type;
        lastShotTime = Time.time;
        return (p);
    }

    public void StartLaser()
    {
        Main.S.isFiring = true;
        laser.positionCount = 2;
        laserFiring = true;
        Invoke("StopLaser", Main.GetWeaponDefinition(type).delayBetweenShots);
    }

    public void StopLaser()
    {
        Main.S.isFiring = false;
        laser.positionCount = 0;
        laserFiring = false;
    }

    private void FixedUpdate()
    {
        if (laserFiring)
        {
            laser.SetPosition(0, new Vector3(transform.position.x, transform.position.y+1, transform.position.z));
            RaycastHit hit;
            int layerMask = 1 << 9;
            if(Physics.Raycast(transform.position, Vector3.up, out hit, Main.S.bndCheck.camHeight - transform.position.y, layerMask))
            {
                if(hit.transform.tag == "Enemy")
                {
                    laser.SetPosition(1, hit.point);
                    Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.up) * hit.distance, Color.green);
                    Enemy enemy = hit.transform.gameObject.GetComponent<Enemy>();
                    enemy.HitByLaser(hit.collider.gameObject);
                }
                else
                {
                    laser.SetPosition(1, new Vector3(transform.position.x, Main.S.bndCheck.camHeight, transform.position.z));
                }

            }
            else
            {
                laser.SetPosition(1, new Vector3(transform.position.x, Main.S.bndCheck.camHeight, transform.position.z));
            }
            
        }
    }
}
