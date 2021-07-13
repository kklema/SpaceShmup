using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour
{
    private GameObject lastTriggerGo = null;

    public delegate void WeaponFireDelegate();
    public WeaponFireDelegate fireDelegate;

    static public Hero S;

    public float speed = 30;
    public float rollMult = -45;
    public float pitchMult = 30;
    public float gameRestartDelay = 2f;

    public GameObject projectilePrefab;
    public float projectileSpeed = 40;
    public Weapon[] weapons;

    [SerializeField] private float _shieldLevel = 1;

    public float ShieldLevel
    {
        get
        {
            return _shieldLevel;
        }
        set
        {
            _shieldLevel = Mathf.Min(value, 4); //Mathf.Min() гарантирует, что никогда не получит значение выше 4
            if (value < 0)
            {
                Destroy(this.gameObject);
                Main.S.DelayedRestart(gameRestartDelay);
            }
        }
    }

    private void Start()
    {
        if (S == null)
        {
            S = this;
        }
        else
        {
            Debug.LogError("Hero.Awake() - second Hero.S");
        }

        ClearWeapons();
        weapons[0].SetType(WeaponType.blaster);

        //fireDelegate += TempFire;
    }

    private void Update()
    {
        float xAxis = Input.GetAxis("Horizontal");
        float yAxis = Input.GetAxis("Vertical");

        Vector3 pos = transform.position;
        pos.x += xAxis * speed * Time.deltaTime;
        pos.y += yAxis * speed * Time.deltaTime;
        transform.position = pos;

        // Повернуть корабль, чтобы придать ощущение динамизма
        transform.rotation = Quaternion.Euler(yAxis * pitchMult, xAxis * rollMult, 0);

        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    TempFire();
        //}

        if (Input.GetAxis("Jump") == 1 && fireDelegate != null)
        {
            fireDelegate();
        }
    }

    //private void TempFire()
    //{
    //    GameObject projGo = Instantiate<GameObject>(projectilePrefab);
    //    projGo.transform.position = transform.position;
    //    Rigidbody rb = projGo.GetComponent<Rigidbody>();

    //    Projectile proj = projGo.GetComponent<Projectile>();
    //    proj.type = WeaponType.blaster;
    //    float tempSpeed = Main.GetWeaponDefinition(proj.type).projectileVelocity;
    //    rb.velocity = Vector3.up * tempSpeed;
    //}

    private void OnTriggerEnter(Collider other)
    {
        Transform rootTransform = other.gameObject.transform.root;
        GameObject go = rootTransform.gameObject;
        //print("Triggerred: " + go.name);

        // Гарантировать невозможность повторного столкновения с тем же объектом
        if (go == lastTriggerGo) return;
        lastTriggerGo = go;

        if (go.tag == "Enemy")
        {
            ShieldLevel--;
            Destroy(go);
        }
        else if (go.tag == "PowerUp")
        {
            AbsorbPowerUp(go);
        }        
    }

    private Weapon GetEmptyWeaponSlot()
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i].type == WeaponType.none)
            {
                return weapons[i];
            }
        }
        return null;
    }

    private void ClearWeapons()
    {
        foreach (Weapon weapon in weapons)
        {
            weapon.SetType(WeaponType.none);
        }
    }

    public void AbsorbPowerUp(GameObject go)
    {
        PowerUp pu = go.GetComponent<PowerUp>();
        switch (pu.type)
        {
            case WeaponType.shield:
                _shieldLevel++;
                break;
            default:
                if (pu.type == weapons[0].type)
                {
                    Weapon w = GetEmptyWeaponSlot();
                    if (w != null)
                    {
                        w.SetType(pu.type);
                    }
                }
                break;
        }

        pu.AbsorbedBy(gameObject);
    }
}