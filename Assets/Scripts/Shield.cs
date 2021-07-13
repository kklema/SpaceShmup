using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    public float rotationPerSecond = 0.1f;

    public int levelShown = 0;

    private Material mat;

    private void Start()
    {
        mat = GetComponent<Renderer>().material;
    }

    private void Update()
    {
        int currenLevel = Mathf.FloorToInt(Hero.S.ShieldLevel);
        if (levelShown != currenLevel)
        {
            levelShown = currenLevel;
            // ��������������� �������� � ��������, ����� ���������� ���� � ������ ���������
            mat.mainTextureOffset = new Vector2(0.2f * levelShown, 0);
        }

        // ������������ ���� � ������ ����� � ���������� ���������
        float rotationZ = -(rotationPerSecond * Time.time * 360) % 360f;
        transform.rotation = Quaternion.Euler(0, 0, rotationZ);
    }
}