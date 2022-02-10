using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    private Rigidbody2D m_Rigidbody2D;
    void Start()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        m_Rigidbody2D.AddForce(Vector2.left * 20f);
        m_Rigidbody2D.angularVelocity = 45f;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var ammo = other.gameObject.GetComponent<AmmoBase>();
        if (ammo != null && ammo.GetShooterType() == AmmoBase.Shooter.Player)
        {
            ammo.Terminate();
            PlayerController.Instance.EnableMissileProjectile();
            Destroy(gameObject);
        }
    }
}
