using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType
{
    none,
    blaster,
    spread,
    phaser,
    missile,
    laser,
    shield
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
    public float continuousDamage = 0;

    public float delayBetweenShots = 0;
    public float projectileVelocity = 20;
}

public class Weapon : MonoBehaviour
{
    static public Transform PROJECTILE_ANCHOR;
    [SerializeField] private WeaponType _type = WeaponType.none;

    [SerializeField]
    public WeaponDefinition def;
    public GameObject collar;
    public float lastShotsTime;
    private Renderer collarRend;

    public WeaponType type
    {
        get
        {
            return _type;
        }
        set
        {
            SetType(value);
        }
    }

    private void Start()
    {
        collar = transform.Find("Collar").gameObject;
        collarRend = collar.GetComponent<Renderer>();

        // Вызвать SetType(), чтобы заменить тип оружия по умолчанию WeaponType.none
        SetType(_type);
        // Динамически создать точку привязки для всех снарядов
        if (PROJECTILE_ANCHOR == null)
        {
            GameObject go = new GameObject("_ProjectileAnchor");
            PROJECTILE_ANCHOR = go.transform;
        }

        // Найти fireDelegate в корневом игровом объекте
        GameObject rootGO = transform.root.gameObject;
        if (rootGO.GetComponent<Hero>() != null)
        {
            rootGO.GetComponent<Hero>().fireDelegate += Fire;
        }
    }

    public void SetType(WeaponType wt)
    {
        _type = wt;
        if (type == WeaponType.none)
        {
            gameObject.SetActive(false);
            return;
        }
        else
        {
            gameObject.SetActive(true);
        }

        def = Main.GetWeaponDefinition(_type);
        collarRend.material.color = def.color;
        lastShotsTime = 0;
    }

    public void Fire()
    {
        if (!gameObject.activeInHierarchy) return;
        if (Time.time - lastShotsTime < def.delayBetweenShots) return;
        Debug.Log("Shoot");

        Projectile p;
        Vector3 vel = Vector3.up * def.projectileVelocity;
        if (transform.up.y < 0)
        {
            vel.y = -vel.y;
        }

        switch (type)
        {
            case WeaponType.blaster:
                Debug.Log("Blaster Ready");
                p = MakeProjectile();
                p.rb.velocity = vel;
                Debug.Log("blaster projectile make");
                break;
            case WeaponType.spread:
                p = MakeProjectile();
                p.rb.velocity = vel;
                p = MakeProjectile();
                p.transform.rotation = Quaternion.AngleAxis(10, Vector3.back); // Снаряд, летящий вправо
                p.rb.velocity = p.transform.rotation * vel;
                p = MakeProjectile();
                p.transform.rotation = Quaternion.AngleAxis(-10, Vector3.back);// Снаряд, летящий влево
                p.rb.velocity = p.transform.rotation * vel;
                break;
                //case WeaponType.phaser:
                //    break;
                //case WeaponType.missile:
                //    break;
                //case WeaponType.laser:
                //    break;
                //case WeaponType.shield:
                //    break;
                //default:
                //    break;
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
        lastShotsTime = Time.time;
        return p;
    }
}