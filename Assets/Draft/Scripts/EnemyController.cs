using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private Collider2D m_Collider2D;
    void Start()
    {
      
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var bullet = other.gameObject.GetComponent<Bullet>();
        if (bullet != null && bullet.GetWhose() == Bullet.Whose.Player)
        {
            bullet.Terminate();
        }
    }
}