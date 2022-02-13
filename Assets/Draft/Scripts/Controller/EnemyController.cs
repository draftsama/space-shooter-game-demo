using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.Pool;

public class EnemyController : CharacterBase
{
    [SerializeField] public ProjectileBase m_Projectile;

    [SerializeField] public List<ItemDropInfo> m_ItemDropInfos;

    public void Shoot()
    {
        if (IsAlive())
            m_Projectile.Shoot();
    }

    public void TerminateWithoutDropItem()
    {
        base.Terminate();
    }
    protected override void Terminate()
    {
        base.Terminate();

        if (m_ItemDropInfos.Count > 0)
        {
            var sum = m_ItemDropInfos.Sum(_ => _.m_DropRate);
            var randomRate = UnityEngine.Random.Range(0f, 1);

            var rateCount = 0f;
            for (int i = 0; i < m_ItemDropInfos.Count; i++)
            {
                var info = m_ItemDropInfos[i];
                rateCount += info.m_DropRate / sum;
                if (randomRate < rateCount)
                {
                    if (info.m_Prefab != null)
                        ObjectPoolingManager.CreateObject($"{info.m_Prefab.name}", info.m_Prefab, m_Transform.position,
                            Quaternion.identity);
                    break;
                }
            }
        }
    }

    [Serializable]
    public struct ItemDropInfo
    {
        public GameObject m_Prefab;
        public float m_DropRate;
    }
}