using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.Pool;

public class EnemyController : CharacterBase
{
    [SerializeField] private bool m_ShootingOnStart;
    [SerializeField] private ProjectileBase m_Projectile;

    
    public void Shoot()
    {
        if (IsAlive())
            m_Projectile.Shoot();
    }


   

   
}