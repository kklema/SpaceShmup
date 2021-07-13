using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private WeaponType _type;

    private BoundsCheck _boundsCheck;
    private Renderer _renderer;
    public Rigidbody rb;

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

    private void Awake()
    {
        _boundsCheck = GetComponent<BoundsCheck>();
        _renderer = GetComponent<Renderer>();
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (_boundsCheck.offUp)
        {
            Destroy(gameObject);
        }
    }

    public void SetType(WeaponType weaponType)
    {
        _type = weaponType;
        WeaponDefinition def = Main.GetWeaponDefinition(_type);
        _renderer.material.color = def.projectileColor;
    }
}