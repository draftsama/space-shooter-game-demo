using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.Pool;

public class EnemyController : CharacterBase
{
    [SerializeField] public ProjectileBase m_Projectile;

    
    public void Shoot()
    {
        if (IsAlive())
            m_Projectile.Shoot();
    }


   

   
}