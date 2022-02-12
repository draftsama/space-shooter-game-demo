using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ProjectileBase : MonoBehaviour
{
    [SerializeField] protected GameObject m_AmmoPrefab;

    [SerializeField] protected List<Transform> m_ProjectilePositionTransforms;
    [SerializeField] protected float m_Delay = 0.2f;
    [SerializeField] protected CharacterBase.CharacterType m_Shooter;
    protected float _CountTime = 0;
    protected IDisposable _UpdateDisposable;

    public Transform m_Transform { private set; get; }

    protected virtual void Awake()
    {
        m_Transform = transform;
        if(m_ProjectilePositionTransforms.Count  == 0)m_ProjectilePositionTransforms.Add(m_Transform);
    }

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
