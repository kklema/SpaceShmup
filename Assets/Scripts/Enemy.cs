using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    protected BoundsCheck boundsCheck;

    public float speed = 10f;
    public float fireRate = 0.3f;
    public float health = 10f;
    public int score = 100;
    public float showDamageDuration = 0.1f;
    public float powerUpDropChance = 1f;

    public Color[] originalColors;
    public Material[] materials;
    public bool showingDamage;
    public float damageDoneTime;
    public bool notifiedOfDestruction = false;

    public Vector3 pos
    {
        get
        {
            return transform.position;
        }
        set
        {
            transform.position = value;
        }
    }

    private void Awake()
    {
        boundsCheck = GetComponent<BoundsCheck>();
        materials = Utils.GetAllMaterials(gameObject);
        originalColors = new Color[materials.Length];
        for (int i = 0; i < materials.Length; i++)
        {
            originalColors[i] = materials[i].color;
        }
    }

    private void Update()
    {
        Move();

        if (boundsCheck != null && boundsCheck.offDown)
        {
            Destroy(gameObject);
        }
    }

    public virtual void Move()
    {
        if (showingDamage && Time.time > damageDoneTime)
        {
            UnShowDamage();
        }

        Vector3 tempPos = pos;
        tempPos.y -= speed * Time.deltaTime;
        pos = tempPos;
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject collGO = collision.gameObject;
        switch (collGO.tag)
        {
            case "ProjectileHero":
                Projectile p = collGO.GetComponent<Projectile>();
                if (!boundsCheck.isOnScreen)
                {
                    Destroy(collGO);
                    break;
                }
                ShowDamage();
                health -= Main.GetWeaponDefinition(p.type).damageOnHit;
                if (health <= 0)
                {
                    if (!notifiedOfDestruction)
                    {
                        Main.S.ShipDestroyed(this);
                    }
                    notifiedOfDestruction = true;
                    Destroy(gameObject);
                }
                Destroy(collGO);
                break;

            default:
                print("Enemy hit by non-ProjectileHero:" + collGO.name);
                break;
        }
    }

    private void ShowDamage()
    {
        foreach (Material mat in materials)
        {
            mat.color = Color.red;
        }
        showingDamage = true;
        damageDoneTime = Time.time + showDamageDuration;
    }

    private void UnShowDamage()
    {
        for (int i = 0; i < materials.Length; i++)
        {
            materials[i].color = originalColors[i];
        }
        showingDamage = false;
    }
}