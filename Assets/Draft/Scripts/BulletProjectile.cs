using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletProjectile : MonoBehaviour
{
    [SerializeField] private GameObject m_BulletPrefab;
    [SerializeField] private int m_MaxProjectile = 2;
    [SerializeField] private float m_Space = 0.1f;
    [SerializeField] private float m_Delay = 0.2f;
    private float _CountTime = 0;

    private Transform _Transform;

    void Start()
    {
        _Transform = transform;
    }

    private void Update()
    {
        _CountTime -= Time.deltaTime;
        if (_CountTime > 0)
        {
            return;
        }

        _CountTime = m_Delay;

        for (int i = 0; i < m_MaxProjectile; i++)
        {
            var x = (_Transform.position.x + (m_Space * i)) - (m_Space * (m_MaxProjectile - 1)) / 2f;
            var pos = new Vector2(x, _Transform.position.y);
            var go = ObjectPoolingManager.CreateObject("bullet", m_BulletPrefab, pos, Quaternion.identity, null);
            go.GetComponent<Bullet>().SetWhose(Bullet.Whose.Player);
        }
    }
}