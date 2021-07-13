using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_2 : Enemy
{
    // Определяют, насколько ярко будет выражен синусоидальный характер движения
    public float sinEccentricity = 0.6f;
    public float lifeTime = 10;

    public Vector3 p0;
    public Vector3 p1;
    public float birthTime;

    private void Start()
    {
        // Выбрать случайную точку на левой границе экрана
        p0 = Vector3.zero;
        p0.x = -boundsCheck.camWidth - boundsCheck.radius;
        p0.y = Random.Range(-boundsCheck.camHeight, boundsCheck.camHeight);

        // Выбрать случайную точку на правой границе экрана
        p1 = Vector3.zero;
        p1.x = boundsCheck.camWidth + boundsCheck.radius;
        p1.y = Random.Range(-boundsCheck.camWidth, boundsCheck.camHeight);

        // Случайно поменять начальную и конечную точку местами
        if (Random.value > 0.5f)
        {
            p0.x *= -1;
            p1.x *= -1;
        }

        birthTime = Time.time;
    }

    public override void Move()
    {
        // Кривые Безье вычисляются на основе значения и между 0 и 1
        float u = (Time.time - birthTime) / lifeTime;

        if (u > 1)
        {
            Destroy(gameObject);
            return;
        }

        // Скорректировать u добавлением значения кривой, изменяющейся по синусоиде
        u = u + sinEccentricity * (Mathf.Sin(u * Mathf.PI * 2));

        //Интерполировать местоположение между двумя точками
        pos = (1 - u) * p0 + u * p1;
    }
}