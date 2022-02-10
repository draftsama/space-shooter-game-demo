using System.Collections;
using System.Collections.Generic;
using Modules.Utilities;
using UniRx;
using UnityEngine;

public class Missile : AmmoBase
{
    [SerializeField]private Transform m_TargetTransform;
    [SerializeField]private float m_RotationSpeed = 50f;
    
    public void SetTarget(Transform _tatget)
    {
        m_TargetTransform = _tatget;
    }
    protected override void FixedUpdate()
    {
        
        var up = _Transform.up;
        m_Rigidbody2D.velocity = up * m_Speed *Time.fixedDeltaTime;

        if (m_TargetTransform != null)
        {
            var direction = (Vector2)m_TargetTransform.position - m_Rigidbody2D.position;
            var rotAmount = -Vector3.Cross(direction.normalized, up).z;
            m_Rigidbody2D.angularVelocity = rotAmount * m_RotationSpeed;
        }
        base.FixedUpdate();
    }
    
}
