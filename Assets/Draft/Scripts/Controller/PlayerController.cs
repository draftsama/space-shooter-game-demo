using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Modules.Utilities;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : CharacterBase
{
    [SerializeField] private float m_RelifeHP = 1f;
    [SerializeField] private float m_Speed = 2f;
    
    [Header("Sprite")] [SerializeField] private Sprite m_SpriteIdle;
    [SerializeField] private Sprite m_SpriteLeft;
    [SerializeField] private Sprite m_SpriteRight;
    [SerializeField] private SpriteRenderer m_RelifeSpriteRenderer;
    [Header("Projectile")]
    [SerializeField] private BulletProjectile m_BulletProjectile;
    [SerializeField] private MissileProjectile m_MissileProjectile;

    private Transform _Transform;
    private Vector2 _UpdatePosition;
    private SpriteRenderer m_SpriteRenderer;
    private Collider2D _Collider2D;
    private Vector2 _InputMovementPos;
    private bool _EnableMove;

    public static PlayerController  Instance { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        Instance = this;
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    protected override void Start()
    {

        _Transform = transform;
        _UpdatePosition = transform.position;
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        _Collider2D = GetComponent<Collider2D>();
        InputManager.Instance.OnInputMovement().Subscribe(_=>_InputMovementPos = _).AddTo(this);
       
        Relife();
        
        Observable.EveryUpdate().Subscribe(_ =>
        {
          if(IsAlive() && _EnableMove)
          {

              var movement = _InputMovementPos;
              movement *= m_Speed * Time.fixedDeltaTime;

              _UpdatePosition += movement;
              _UpdatePosition = ClampAreaScreen(_UpdatePosition);
              _Transform.position = _UpdatePosition;


              if (_InputMovementPos.x > 0)
                  m_SpriteRenderer.sprite = m_SpriteRight;
              else if (_InputMovementPos.x < 0)
                  m_SpriteRenderer.sprite = m_SpriteLeft;
              else
                  m_SpriteRenderer.sprite = m_SpriteIdle;

          }
        }).AddTo(this);
    }

    private void Relife()
    {
        _EnableMove = false;
        _Transform.position = new Vector3(0, -7, 0);
        m_RelifeSpriteRenderer.color = Color.white;
        _UpdatePosition = new Vector3(0, -3.4f, 0);
        _Transform.LerpPosition(1000, _UpdatePosition,false).Subscribe(_ =>
        {
            var color = m_RelifeSpriteRenderer.color;
            m_SpriteRenderer.enabled = true;

            LerpThread.FloatLerp(300, 1, 0).Subscribe(_value =>
            {
                color.a = _value;
                m_RelifeSpriteRenderer.color = color;
            }, () =>
            {
                 SetHealthPower(m_RelifeHP);
                m_BulletProjectile.ShootAuto();
            //extra
                m_MissileProjectile.ShootAuto();

                _EnableMove = true;
                Observable.Timer(TimeSpan.FromMilliseconds(2000)).Subscribe(_ =>
                {
                    _Collider2D.enabled = true;

                }).AddTo(this);


            }).AddTo(this);

        }).AddTo(this);
    }
    private Vector3 ClampAreaScreen(Vector3 _input)
    {
        var areaHeight = 2f * Camera.main.orthographicSize;
        var areaWidth = areaHeight * Camera.main.aspect;
        var topPos = (areaHeight / 2f) - 0.5f;
        var bottomPos = -(areaHeight / 2f) + 0.5f;
        var leftPos = -(areaWidth / 2f) + 0.5f;
        var rightPos = (areaWidth / 2f) - 0.5f;

        _input.x = Mathf.Clamp(_input.x, leftPos, rightPos);
        _input.y = Mathf.Clamp(_input.y, bottomPos, topPos);
        return _input;
    }

    public void EnableMissileProjectile()
    {
        m_MissileProjectile.ShootAuto();
    }
    
    
    protected override void Terminate()
    {
        base.Terminate();
       
        
        m_SpriteRenderer.enabled = false;
        _Collider2D.enabled = false;
        _Transform.position = new Vector3(0, -7, 0);
        m_BulletProjectile.StopShoot();
        m_MissileProjectile.StopShoot();
        
        Observable.Timer(TimeSpan.FromMilliseconds(3000)).Subscribe(_ => Relife()).AddTo(this);
       
    }

}