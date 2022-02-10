using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBase : MonoBehaviour
{
    [SerializeField] protected GameObject m_Prefab;
    [SerializeField] protected List<Transform> m_ProjectilePositionTransforms;
    [SerializeField] protected float m_Delay = 0.2f;
    [SerializeField] protected AmmoBase.Shooter m_Shooter;
    [SerializeField] protected bool m_EnableOnStart;
    protected float _CountTime = 0;
    
    protected virtual void Start()
    {
        if (m_EnableOnStart) ShootAuto();
    }
    protected IDisposable _UpdateDisposable;
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
