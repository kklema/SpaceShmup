using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Part
{
    public string name;
    public float health;
    public string[] protectedBy;

    [HideInInspector] public GameObject go;
    [HideInInspector] public Material mat;
}

public class Enemy_4 : Enemy
{
    [Header("Set in Inspector: Enemy_4")]
    public Part[] parts;

    private Vector3 p0, p1;
    private float timeStart;
    private float duration = 4f;

    private void Start()
    {
        p0 = p1 = pos;

        Transform t;
        foreach (Part part in parts)
        {
            t = transform.Find(part.name);
            if (t != null)
            {
                part.go = t.gameObject;
                part.mat = part.go.GetComponent<Renderer>().material;
            }
        }
    }

    private void InitMovement()
    {
        p0 = p1;
        float widMinRad = boundsCheck.camWidth - boundsCheck.radius;
        float hgtMinRad = boundsCheck.camHeight - boundsCheck.radius;
        p1.x = Random.Range(-widMinRad, widMinRad);
        p1.y = Random.Range(-hgtMinRad, hgtMinRad);

        timeStart = Time.time;
    }

    public override void Move()
    {
        float u = (Time.time - timeStart) / duration;

        if (u > 1)
        {
            InitMovement();
            u = 0;
        }

        u = 1 - Mathf.Pow(1 - u, 2); // Применить плавное замедление
        pos = (1 - u) * p0 + u * p1; // Простая линейная интерполяция
    }

    private Part FindPart(string n)
    {
        foreach (Part part in parts)
        {
            if (part.name == n)
            {
                return part;
            }
        }

        return null;
    }

    private Part FindPart(GameObject go)
    {
        foreach (Part part in parts)
        {
            if (part.go == go)
            {
                return part;
            }
        }

        return null;
    }

    private bool Destroyed(GameObject go)
    {
        return Destroyed(FindPart(go));
    }

    private bool Destroyed(string n)
    {
        return Destroyed(FindPart(n));
    }

    private bool Destroyed(Part prt)
    {
        if (prt == null)
        {
            return true;
        }

        return prt.health <= 0;
    }

    private void ShowLocalizeDamage(Material material)
    {
        material.color = Color.red;
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
                if (!boundsCheck.isOnScreen)
                {
                    Destroy(other);
                    break;
                }

                GameObject goHit = collision.contacts[0].thisCollider.gameObject;
                Part prtHit = FindPart(goHit);
                if (prtHit == null)
                {
                    goHit = collision.contacts[0].otherCollider.gameObject;
                    prtHit = FindPart(goHit);
                }

                if (prtHit.protectedBy != null)
                {
                    foreach (string s in prtHit.protectedBy)
                    {
                        if (!Destroyed(s))
                        {
                            Destroy(other);
                            return;
                        }
                    }
                }

                prtHit.health -= Main.GetWeaponDefinition(p.type).damageOnHit;

                ShowLocalizeDamage(prtHit.mat);
                if (prtHit.health <= 0)
                {
                    prtHit.go.SetActive(false);
                }

                bool allDestroyed = true;
                foreach (Part part in parts)
                {
                    if (!Destroyed(part))
                    {
                        allDestroyed = false;
                        break;
                    }
                }
                if (allDestroyed)
                {
                    Main.S.ShipDestroyed(this);
                    Destroy(gameObject);
                }
                Destroy(other);
                break;
        }
    }
}