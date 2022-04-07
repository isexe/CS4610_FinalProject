using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Part {
    public string name;         //name of part
    public float health;        //health of part
    public string[] protectedBy;//parts protecting it

    [HideInInspector]
    public GameObject go;
    [HideInInspector]
    public Material mat;
}

public class Enemy4 : Enemy
{
    [Header("Set in Inspector: Enemy_4")]
    public Part[] parts;

    private Vector3 p0, p1;
    private float timeStart;
    private float duration = 4;

    private void Start()
    {
        p0 = p1 = pos;

        InitMovement();

        // Cache GameObject & Material of each part in parts
        Transform t;
        foreach (Part prt in parts)
        {
            t = transform.Find(prt.name);
            if (t != null)
            {
                prt.go = t.gameObject;
                prt.mat = prt.go.GetComponent<Renderer>().material;
            }
        }
    }

    void InitMovement()
    {
        p0 = p1; // set old position to new position
        // assign new on-screen location to p1
        float widMinRad = bndCheck.camWidth - bndCheck.radius;
        float hgtMinRad = bndCheck.camHeight - bndCheck.radius;
        p1.x = Random.Range(-widMinRad, widMinRad);
        p1.y = Random.Range(-hgtMinRad, hgtMinRad);

        //reset time
        timeStart = Time.time;
    }

    public override void Move()
    {
        float u = (Time.time - timeStart) / duration;

        if (u >= 1)
        {
            InitMovement();
            u = 0;
        }

        u = 1 - Mathf.Pow(1 - u, 2);
        pos = (1 - u) * p0 + u * p1;
    }

    // These two functions find  a Part in parts based on name or GameObject
    Part FindPart(string n)
    {
        foreach (Part prt in parts)
        {
            if (prt.name == n) return (prt);
        }
        return (null);
    }

    Part FindPart(GameObject go)
    {
        foreach (Part prt in parts)
        {
            if (prt.go == go) return (prt);
        }
        return (null);
    }

    bool Destroyed(GameObject go) { return (Destroyed(FindPart(go))); }
    bool Destroyed(string n) { return (Destroyed(FindPart(n))); }

    bool Destroyed(Part prt)
    {
        if (prt == null) return true;
        return (prt.health <= 0);
    }

    void ShowLocalizedDamage(Material m)
    {
        m.color = Color.red;
        damageDoneTime = Time.time + showDamageDuration;
        showingDamage = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject other = collision.gameObject;
        switch (other.tag)
        {
            case "ProjectileHero":
                Projectile p = other.GetComponent<Projectile>();
                // If enemy is offscreen don't hurt
                if (!bndCheck.isOnScreen)
                {
                    Destroy(other);
                    break;
                }

                // Hurt enemy
                GameObject goHit = collision.contacts[0].thisCollider.gameObject;
                Part prtHit = FindPart(goHit);
                if(prtHit == null)
                {
                    goHit = collision.contacts[0].otherCollider.gameObject;
                    prtHit = FindPart(goHit);
                }

                // check whether this part is still protected
                if (prtHit.protectedBy != null)
                {
                    foreach (string s in prtHit.protectedBy)
                    {
                        // check if parts protecting it are destroyed
                        if (!Destroyed(s))
                        {
                            Destroy(other);
                            return;
                        }
                    }
                }

                // if not protected calculate damage
                prtHit.health -= Main.GetWeaponDefinition(p.type).damageOnHit;
                // Show damage on part
                ShowLocalizedDamage(prtHit.mat);
                if(prtHit.health <= 0)
                {
                    // if destroyed deactivate part
                    prtHit.go.SetActive(false);
                }

                // check if all is destroyed
                bool allDestroyed = true;
                foreach (Part prt in parts)
                {
                    if (!Destroyed(prt))
                    {
                        allDestroyed = false;
                        break;
                    }
                }
                if (allDestroyed)  //if completely destoyed
                {
                    Main.S.ShipDestroyed(this); //tell main
                    Destroy(this.gameObject); //destroy enemy GO
                }
                Destroy(other); //destoy projectile
                break;
        }
    }
}
