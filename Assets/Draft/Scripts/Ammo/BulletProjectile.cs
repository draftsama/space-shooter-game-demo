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
          var go =  m_AmmoCreator.Get();
          go.transform.position = m_ProjectilePositionTransforms[i].position;
          go.transform.rotation = m_ProjectilePositionTransforms[i].rotation;
          var bullet = go.GetComponent<Bullet>();
          IDisposable disposable = null;
          disposable = bullet.OnTerminateAsObservable().Subscribe(_ =>
          {
              disposable?.Dispose();
              m_AmmoCreator.Release(_);
          }).AddTo(bullet);
          bullet.SetShooter(m_Shooter);
        }
    }
}