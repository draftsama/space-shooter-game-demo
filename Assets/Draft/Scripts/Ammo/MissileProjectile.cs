using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

public class MissileProjectile : ProjectileBase
{
    [SerializeField] private GameObject m_LockTargetPrefab;
    [SerializeField] private float m_DistanceLockTarget = 3f;
    [SerializeField] private LockOnTarget[] _LockOnTargets;

    protected override void Start()
    {
        base.Start();
        _LockOnTargets = new LockOnTarget[m_ProjectilePositionTransforms.Count];
        for (int i = 0; i < m_ProjectilePositionTransforms.Count; i++)
        {
            _LockOnTargets[i].m_Projectile = m_ProjectilePositionTransforms[i];
        }
    }


    public override void ShootAuto()
    {
        _CountTime = 0;
        _UpdateDisposable?.Dispose();
        _UpdateDisposable = Observable.EveryUpdate().Subscribe(_ =>
        {
            FindTarget();
            Shoot();
        }).AddTo(this);
    }

    public override void Shoot()
    {
        if(Time.time - _CountTime < m_Delay) return;
        _CountTime = Time.time;
        
        for (int i = 0; i < m_ProjectilePositionTransforms.Count; i++)
        {
            var projectile = m_ProjectilePositionTransforms[i];
            if (!projectile.gameObject.activeSelf) continue;

            var lockTarget = _LockOnTargets[i];


            var go = ObjectPoolingManager.CreateObject(m_Prefab.name, m_Prefab, projectile.position, Quaternion.identity,
                null);
            go.GetComponent<Missile>().SetShooterType(m_Shooter);
            go.GetComponent<Missile>().SetTarget(_LockOnTargets[i].m_Target);
        }
    }

    void FindTarget()
    {
        for (int i = 0; i < _LockOnTargets.Length; i++)
        {
            if (_LockOnTargets[i].m_Target != null)
            {
                if ((_LockOnTargets[i].m_Target.position - _LockOnTargets[i].m_Projectile.position).magnitude >
                    m_DistanceLockTarget)
                {
                    _LockOnTargets[i].m_Target = null;
                    if (_LockOnTargets[i].m_LockTargetFx != null)
                    {
                        ObjectPoolingManager.KillObject(_LockOnTargets[i].m_LockTargetFx);
                        _LockOnTargets[i].m_LockTargetFx = null;
                    }
                }
            }


            var orderTargets = ObjectPoolingManager.GetObjects("enemy")
                .OrderBy(_ => (_.m_Transform.position - _LockOnTargets[i].m_Projectile.position).magnitude).ToArray();

            for (int j = 0; j < orderTargets.Length; j++)
            {
                var target = orderTargets[j].m_Transform;

                if (_LockOnTargets.Select(_ => _.m_Target).Contains(target)) continue;
                if ((target.position - _LockOnTargets[i].m_Projectile.position).magnitude >
                    m_DistanceLockTarget) continue;

                if (_LockOnTargets[i].m_LockTargetFx == null)
                {
                    _LockOnTargets[i].m_Target = target;
                    _LockOnTargets[i].m_LockTargetFx = ObjectPoolingManager.CreateObject("locktarget_fx",
                        m_LockTargetPrefab, target.position, Quaternion.identity, target);
                }

                break;
            }
        }
    }

    [System.Serializable]
    struct LockOnTarget
    {
        public Transform m_Target;
        public Transform m_Projectile;
        public GameObject m_LockTargetFx;
    }
}