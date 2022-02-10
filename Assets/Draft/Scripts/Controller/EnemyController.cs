using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    
    private Collider2D m_Collider2D;
    [SerializeField]public float m_Health = 1200;
    [SerializeField]private ProjectileBase m_Projectile;
    [SerializeField]protected GameObject m_ExplosionFx;
    [SerializeField]protected float m_ExplosionScale = 0.2f;
    public Transform m_Transform { get; private set; }
    void Start()
    {
        m_Transform = transform;
    }

    public void Shoot()
    {
        if (m_Health <= 0) return;
        m_Projectile.Shoot(); 
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var ammo = other.gameObject.GetComponent<AmmoBase>();
        if (ammo != null && ammo.GetShooterType() == AmmoBase.Shooter.Player)
        {
            m_Health -= ammo.m_Damage;

            ammo.Terminate();
            if (m_Health <= 0)
            {
                Terminate();
            }

        }
    }
    
    public void Terminate()
    {
        var go = ObjectPoolingManager.CreateObject("explosion.fx", m_ExplosionFx, m_Transform.position, Quaternion.Euler(0,0,UnityEngine.Random.Range(0,360)));
        go.transform.localScale = Vector3.one * m_ExplosionScale;
      

        ObjectPoolingManager.KillObject(gameObject);
       
    }
}