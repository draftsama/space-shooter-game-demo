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
            Shoot();
        }).AddTo(this);
    }

    public override void Shoot()
    {
        if (Time.time - _CountTime < m_Delay) return;
        _CountTime = Time.time;
        FindTarget();

        for (int i = 0; i < m_ProjectilePositionTransforms.Count; i++)
        {
            var projectile = m_ProjectilePositionTransforms[i];
            if (!projectile.gameObject.activeSelf) continue;

            var lockTarget = _LockOnTargets[i];
            
            
            var go =  m_AmmoCreator.Get();
            go.transform.position = projectile.position;
            go.transform.rotation = projectile.rotation;
            var messile = go.GetComponent<Missile>();
            messile.SetShooter(m_Shooter);
            messile.SetTarget(lockTarget.m_Target);
            IDisposable disposable = null;
            disposable = messile.OnTerminateAsObservable().Subscribe(_ =>
            {
                disposable?.Dispose();
                m_AmmoCreator.Release(_);
            }).AddTo(messile);
        }
    }

    void FindTarget()
    {
        for (int i = 0; i < _LockOnTargets.Length; i++)
        {
            if (_LockOnTargets[i].m_Target != null)
            {
                
                if (!_LockOnTargets[i].m_Target.IsAlive() ||
                    (_LockOnTargets[i].m_Target.m_Transform.position - _LockOnTargets[i].m_Projectile.position).magnitude >
                    m_DistanceLockTarget || 
                    !_LockOnTargets[i].m_Target.IsOnScreen() )
                {
                    _LockOnTargets[i].m_Target = null;
                    if (_LockOnTargets[i].m_LockTargetFx != null)
                    {
                        ObjectPoolingManager.Kill(_LockOnTargets[i].m_LockTargetFx);
                        _LockOnTargets[i].m_LockTargetFx = null;
                    }
                }
            }

            CharacterBase[] orderTargets = null;

            if (m_Shooter == CharacterBase.CharacterType.Player)
            {
                orderTargets = ObjectPoolingManager.GetObjects("enemy")
                    .OrderBy(_ => (_.m_Transform.position - _LockOnTargets[i].m_Projectile.position).magnitude)
                    .Select(_ => _.m_Transform.GetComponent<CharacterBase>()).ToArray();
            }
            else if (m_Shooter == CharacterBase.CharacterType.Enemy)
            {
                orderTargets = new CharacterBase[1] { PlayerController.Instance };
            }

            for (int j = 0; j < orderTargets.Length; j++)
            {
                var target = orderTargets[j];
                if(!target.gameObject.activeSelf)continue;
                
                if (_LockOnTargets.Select(_ => _.m_Target).Contains(target)) continue;
                if ((target.m_Transform.position - _LockOnTargets[i].m_Projectile.position).magnitude >
                    m_DistanceLockTarget) continue;
               if( !target.IsOnScreen())continue;
               

                if (_LockOnTargets[i].m_LockTargetFx == null && m_LockTargetPrefab != null && _LockOnTargets[i].m_Target == null)
                {
                    _LockOnTargets[i].m_Target = target;
                    _LockOnTargets[i].m_LockTargetFx = ObjectPoolingManager.CreateObject("locktarget",
                        m_LockTargetPrefab, target.m_Transform.position, target.m_Transform.rotation, target.m_Transform);
                }

                break;
            }
        }
    }

    [System.Serializable]
    struct LockOnTarget
    {
        public CharacterBase  m_Target;
        public Transform m_Projectile;
        public GameObject m_LockTargetFx;
    }
}