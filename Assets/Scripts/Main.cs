using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour
{
    static public Main S;
    static Dictionary<WeaponType, WeaponDefinition> weapons;

    public WeaponDefinition[] weaponDefinitions;
    public GameObject prefabPowerUp;
    public GameObject[] prefabEnemies;

    public WeaponType[] powerUpFrequency = new WeaponType[] { WeaponType.blaster, WeaponType.blaster, WeaponType.spread, WeaponType.shield };

    public float enemySpawnedPerSecond = 0.5f;

    public float enemyDefaultPadding = 1.5f;

    private BoundsCheck _boundsCheck;

    private void Awake()
    {
        S = this;
        _boundsCheck = GetComponent<BoundsCheck>();
        Invoke("SpawnEnemy", enemySpawnedPerSecond);

        weapons = new Dictionary<WeaponType, WeaponDefinition>();
        foreach (WeaponDefinition weapon in weaponDefinitions)
        {
            weapons[weapon.type] = weapon;
        }
    }

    static public WeaponDefinition GetWeaponDefinition(WeaponType type)
    {
        if (weapons.ContainsKey(type))
        {
            return weapons[type];
        }

        return new WeaponDefinition();
    }

    public void ShipDestroyed(Enemy enemy)
    {
        if (Random.value <= enemy.powerUpDropChance)
        {
            int ndx = Random.Range(0, powerUpFrequency.Length);
            WeaponType puType = powerUpFrequency[ndx];

            GameObject go = Instantiate(prefabPowerUp) as GameObject;
            PowerUp pu = go.GetComponent<PowerUp>();
            pu.SetType(puType);

            pu.transform.position = enemy.transform.position;
        }
    }

    public void SpawnEnemy()
    {
        int ndx = Random.Range(0, prefabEnemies.Length);
        GameObject go = Instantiate<GameObject>(prefabEnemies[ndx]);

        float enemyPadding = enemyDefaultPadding;
        if (go.GetComponent<BoundsCheck>() != null)
        {
            enemyPadding = Mathf.Abs(go.GetComponent<BoundsCheck>().radius);
        }

        Vector3 pos = Vector3.zero;
        float xMin = -_boundsCheck.camWidth + enemyPadding;
        float xMax = _boundsCheck.camWidth - enemyPadding;
        pos.x = Random.Range(xMin, xMax);
        pos.y = _boundsCheck.camHeight + enemyPadding;
        go.transform.position = pos;

        Invoke("SpawnEnemy", enemySpawnedPerSecond);
    }

    public void DelayedRestart(float delay)
    {
        Invoke("Restart", delay);
    }

    public void Restart()
    {
        SceneManager.LoadScene("_Scene_0");
    }
}