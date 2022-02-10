using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoBase : MonoBehaviour
{
    public enum  Shooter
    {
        None,Player,Enemy
    }
    [SerializeField]public float m_Damage = 100f;
    [SerializeField]protected Shooter m_Shooter = Shooter.None;
    [SerializeField]protected float m_Speed = 300f;
    [SerializeField]protected GameObject m_ExplosionFx;
    [SerializeField]protected float m_ExplosionScale = 0.2f;
    protected Rigidbody2D m_Rigidbody2D;
    protected Transform _Transform;

    protected virtual void Awake()
    {
        _Transform = transform;
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        
    }

    protected virtual void FixedUpdate()
    {
        if (!IsOnScreen(m_Rigidbody2D.position))
        {
            ObjectPoolingManager.KillObject(gameObject);
            m_Shooter = Shooter.None;
        }
    }

    public void SetShooterType(Shooter shooter)
    {
        m_Shooter = shooter;
    }
    public Shooter GetShooterType()
    {
        return m_Shooter;
    }
    
    public void Terminate()
    {
        var go = ObjectPoolingManager.CreateObject("explosion.fx", m_ExplosionFx, _Transform.position, Quaternion.Euler(0,0,UnityEngine.Random.Range(0,360)));
        go.transform.localScale = Vector3.one * m_ExplosionScale;
        ObjectPoolingManager.KillObject(gameObject);
        m_Shooter = Shooter.None;
    }
    
    private bool IsOnScreen(Vector2 _input)
    {
        var areaHeight = 2f * Camera.main.orthographicSize;
        var areaWidth = areaHeight * Camera.main.aspect;
        var topPos = (areaHeight / 2f);
        var bottomPos = -(areaHeight / 2f);
        var leftPos = -(areaWidth / 2f);
        var rightPos = (areaWidth / 2f);
        
        return _input.x > leftPos && _input.x < rightPos && _input.y >bottomPos && _input.y < topPos;
    }

   
}
