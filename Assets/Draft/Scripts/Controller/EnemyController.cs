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
    [SerializeField] protected ObjectPoolCreator m_ExplosionFxCreator;

    //TODO new ExplosionFX System


    public void Shoot()
    {
        if (IsAlive())
            m_Projectile.Shoot();
    }


   

   
}