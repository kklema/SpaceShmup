using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public Vector2 rotMinMax = new Vector2(15, 90);
    public Vector2 driftMinMax = new Vector2(0.25f, 2);
    public float lifeTime = 6f;
    public float fadetime = 4f;

    public WeaponType type;
    public GameObject cube;
    public TextMesh letter;
    public Vector3 rotPerSecond; // Скорость вращения
    public float birthTime;

    private Rigidbody _rb;
    private BoundsCheck _bndCheck;
    private Renderer _cubeRend;

    private void Awake()
    {
        cube = transform.Find("Cube").gameObject;
        letter = GetComponent<TextMesh>();
        _rb = GetComponent<Rigidbody>();
        _bndCheck = GetComponent<BoundsCheck>();
        _cubeRend = GetComponent<Renderer>();

        Vector3 vel = Random.onUnitSphere;
        vel.z = 0;
        vel.Normalize();
        vel *= Random.Range(driftMinMax.x, driftMinMax.y);
        _rb.velocity = vel;

        transform.rotation = Quaternion.identity;

        rotPerSecond = new Vector3(Random.Range(rotMinMax.x, rotMinMax.y),Random.Range(rotMinMax.x,rotMinMax.y), Random.Range(rotMinMax.x, rotMinMax.y));

        birthTime = Time.time;
    }

    private void Start()
    {
        SetType(type);
    }

    private void Update()
    {
        cube.transform.rotation = Quaternion.Euler(rotPerSecond * Time.time);

        float u = (Time.time - (birthTime + lifeTime)) / fadetime;

        if (u >= 1)
        {
            Destroy(gameObject);

            return;
        }

        if (u>0)
        {
            Color c = _cubeRend.material.color;
            c.a = 1f - u;
            _cubeRend.material.color = c;
            c = letter.color;
            c.a = 1f - (u * 0.5f);
            letter.color = c;
        }

        if (!_bndCheck.isOnScreen)
        {
            Destroy(gameObject);
        }
    }

    public void SetType(WeaponType weaponType)
    {
        WeaponDefinition def = Main.GetWeaponDefinition(weaponType);
        _cubeRend.material.color = def.color;
        letter.text = def.letter;
        type = weaponType;
    }

    public void AbsorbedBy(GameObject target)
    {
        Destroy(gameObject);
    }
}