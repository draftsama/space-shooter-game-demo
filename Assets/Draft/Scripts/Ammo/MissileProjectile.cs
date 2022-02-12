using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

public class MissileProjectile : ProjectileBase
{
    [SerializeField] private GameObject m_MarkingFxPrefab;
    [SerializeField] private float m_MarkingLenght = 3f;
    [SerializeField] private MarkingInfo[] _MarkingInfos;


    protected override void Start()
    {
        base.Start();

        _MarkingInfos = new MarkingInfo[m_ProjectilePositionTransforms.Count];
        for (int i = 0; i < m_ProjectilePositionTransforms.Count; i++)
        {
            _MarkingInfos[i].m_Projectile = m_ProjectilePositionTransforms[i];
        }
    }


    public override void ShootAuto()
    {
        _CountTime = -1;
        _UpdateDisposable?.Dispose();
        _UpdateDisposable = Observable.EveryUpdate().Subscribe(_ => { Shoot(); }).AddTo(this);
    }

    public override void Shoot()
    {

        if (Time.time - _CountTime < m_Delay && _CountTime > 0) return;
        _CountTime = Time.time;
       
        FindTarget();


        for (int i = 0; i < m_ProjectilePositionTransforms.Count; i++)
        {
            var projectile = m_ProjectilePositionTransforms[i];
            if (!projectile.gameObject.activeSelf) continue;

            var lockTarget = _MarkingInfos[i];

            var go = ObjectPoolingManager.CreateObject($"missile_{GetInstanceID()}", m_AmmoPrefab);
            go.transform.position = projectile.position;
            go.transform.rotation = projectile.rotation;
            var messile = go.GetComponent<Missile>();
            messile.SetShooter(m_Shooter);
            messile.SetTarget(lockTarget.m_Target);
            IDisposable disposable = null;
            disposable = messile.OnTerminateAsObservable().Subscribe(_ =>
            {
                disposable?.Dispose();
                ObjectPoolingManager.Kill(_);
            }).AddTo(messile);
        }
    }

    public override void StopShoot()
    {
        base.StopShoot();
        ClearMark();
    }

    public void ClearMark()
    {
        Debug.Log("ClearMark");

        for (int i = 0; i < _MarkingInfos.Length; i++)
        {
            if (_MarkingInfos[i].m_Target != null)
            {
                _MarkingInfos[i].m_Target = null;
                if (_MarkingInfos[i].m_MarkFx != null)
                {
                    ObjectPoolingManager.Kill(_MarkingInfos[i].m_MarkFx);
                    _MarkingInfos[i].m_MarkFx = null;
                }
            }
        }

        ObjectPoolingManager.KillGroup("locktarget");
    }

    void FindTarget()
    {
        //   Debug.Log("find");
        for (int i = 0; i < _MarkingInfos.Length; i++)
        {
            if (_MarkingInfos[i].m_Target != null)
            {
                //check condition marking
                if (!_MarkingInfos[i].m_Target.IsAlive() ||
                    (_MarkingInfos[i].m_Target.m_Transform.position - _MarkingInfos[i].m_Projectile.position)
                    .magnitude >
                    m_MarkingLenght ||
                    !_MarkingInfos[i].m_Target.IsOnScreen())
                {
                    _MarkingInfos[i].m_Target = null;
                    if (_MarkingInfos[i].m_MarkFx != null)
                    {
                        ObjectPoolingManager.Kill(_MarkingInfos[i].m_MarkFx);
                        _MarkingInfos[i].m_MarkFx = null;
                    }
                }
                else
                {
                    //pass
                    continue;
                }
            }

            //find target
            CharacterBase detectTarget = null;
            if (m_Shooter == CharacterBase.CharacterType.Player)
            {
                detectTarget = ObjectPoolingManager
                    .GetObjects("enemy")
                    .OrderBy(_ => (_.m_Transform.position - _MarkingInfos[i].m_Projectile.position).magnitude)
                    .Select(_ => _.m_Transform.GetComponent<CharacterBase>()).FirstOrDefault(_o =>
                        !_MarkingInfos.Select(_ => _.m_Target).Contains(_o) && _o.IsAlive() &&
                        _o.IsOnScreen() &&
                        (_o.m_Transform.position - _MarkingInfos[i].m_Projectile.position).magnitude <=
                        m_MarkingLenght);
            }
            else if (m_Shooter == CharacterBase.CharacterType.Enemy)
            {
                var distance = (PlayerController.Instance.m_Transform.position - _MarkingInfos[i].m_Projectile.position)
                    .magnitude;

                if (PlayerController.Instance.IsAlive() && PlayerController.Instance.IsOnScreen() &&
                    distance <= m_MarkingLenght)
                    detectTarget = PlayerController.Instance;
            }

            if (detectTarget != null)
            {
                _MarkingInfos[i].m_Target = detectTarget;

                if (_MarkingInfos[i].m_MarkFx == null && m_MarkingFxPrefab != null)
                {
                    _MarkingInfos[i].m_MarkFx = ObjectPoolingManager.CreateObject("locktarget",
                        m_MarkingFxPrefab, detectTarget.m_Transform.position, detectTarget.m_Transform.rotation,
                        detectTarget.m_Transform);
                }
            }
        }
    }

    [System.Serializable]
    struct MarkingInfo
    {
        public CharacterBase m_Target;
        public Transform m_Projectile;
        public GameObject m_MarkFx;
    }
}