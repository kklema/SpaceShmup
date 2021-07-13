using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_1 : Enemy
{
    public float waveFrequency = 2f;

    // ������ ��������� � ������
    public float waveWidth = 4f;
    public float waveRotY = 45f;

    private float x0; // ������ ��������� � ������
    private float birthTime;

    private void Start()
    {
        x0 = pos.x;

        birthTime = Time.time;
    }

    public override void Move()
    {
        // ��� ��� pos * ��� ��������, ������ �������� �������� pos.x
        // ������� ������� pos � ���� ������� Vector3, ���������� ��� ���������
        Vector3 tempPos = pos;

        //� �������� theta ���������� � �������� �������
        float age = Time.time - birthTime;
        float theta = Mathf.PI * 2 * age / waveFrequency;
        float sin = Mathf.Sin(theta);
        tempPos.x = x0 + waveWidth * sin;
        pos = tempPos;

        // ��������� ������� ������������ ��� Y
        Vector3 rot = new Vector3(0, sin * waveRotY, 0);
        transform.rotation = Quaternion.Euler(rot);

        base.Move();
    }
}
