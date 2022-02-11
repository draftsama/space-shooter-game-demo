using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ProjectileBase : MonoBehaviour
{
    [SerializeField] protected ObjectPoolCreator m_AmmoCreator;

    [SerializeField] protected List<Transform> m_ProjectilePositionTransforms;
    [SerializeField] protected float m_Delay = 0.2f;
    [SerializeField] protected CharacterBase.CharacterType m_Shooter;
    protected float _CountTime = 0;
    protected IDisposable _UpdateDisposable;
    

    protected virtual void Start()
    {
       
    }
    public virtual void Shoot()
    {
    }
    public virtual void ShootAuto()
    {
    }
    public virtual void StopShoot()
    {
        _UpdateDisposable?.Dispose();
    }

   
   
}
