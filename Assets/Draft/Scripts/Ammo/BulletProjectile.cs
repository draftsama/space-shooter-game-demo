using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class BulletProjectile : ProjectileBase
{
    public override void ShootAuto()
    {
        _CountTime = 0;
        _UpdateDisposable?.Dispose();
        _UpdateDisposable = Observable.EveryUpdate().Subscribe(_ =>
        {
            Shoot();
        }).AddTo(this);
    }

    public override void Shoot()
    {
        if(Time.time - _CountTime < m_Delay) return;
        _CountTime = Time.time;
        for (int i = 0; i < m_ProjectilePositionTransforms.Count; i++)
        {
            var go = ObjectPoolingManager.CreateObject(m_Prefab.name, m_Prefab, m_ProjectilePositionTransforms[i].position,
                m_ProjectilePositionTransforms[i].rotation, null);

            go.GetComponent<Bullet>().SetShooterType(m_Shooter);
        }
    }
}