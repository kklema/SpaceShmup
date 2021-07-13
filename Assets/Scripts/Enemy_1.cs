using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_1 : Enemy
{
    public float waveFrequency = 2f;

    // ширина синусоиды в метрах
    public float waveWidth = 4f;
    public float waveRotY = 45f;

    private float x0; // ширина синусоиды в метрах
    private float birthTime;

    private void Start()
    {
        x0 = pos.x;

        birthTime = Time.time;
    }

    public override void Move()
    {
        // Так как pos * это свойство, нельзя напрямую изменить pos.x
        // поэтому получим pos в виде вектора Vector3, доступного для изменения
        Vector3 tempPos = pos;

        //И значение theta изменяется с течением времени
        float age = Time.time - birthTime;
        float theta = Mathf.PI * 2 * age / waveFrequency;
        float sin = Mathf.Sin(theta);
        tempPos.x = x0 + waveWidth * sin;
        pos = tempPos;

        // повернуть немного относительно оси Y
        Vector3 rot = new Vector3(0, sin * waveRotY, 0);
        transform.rotation = Quaternion.Euler(rot);

        base.Move();
    }
}
