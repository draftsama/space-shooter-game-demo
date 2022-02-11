using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.Pool;

public class AmmoBase : MonoBehaviour
{
   
    [SerializeField]public float m_Damage = 100f;
    [SerializeField]protected CharacterBase.CharacterType m_Shooter = CharacterBase.CharacterType.None;
    [SerializeField]protected float m_Speed = 300f;
    [SerializeField]protected ObjectPoolCreator m_ExplosionFxCreator;
    [SerializeField]protected float m_ExplosionScale = 0.2f;
    protected Rigidbody2D m_Rigidbody2D;
    protected Transform _Transform;

    private Action<GameObject> OnTerminate;



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
            OnTerminate?.Invoke(gameObject);
            gameObject.SetActive(false);
            m_Shooter = CharacterBase.CharacterType.None;
        }
    }

   
    public void SetShooter(CharacterBase.CharacterType shooter)
    {
        m_Shooter = shooter;
    }
    public CharacterBase.CharacterType GetShooterType()
    {
        return m_Shooter;
    }
    
    public void Terminate()
    {
        
        var go = m_ExplosionFxCreator.Get(
            _Transform.position,Quaternion.Euler(0, 0, UnityEngine.Random.Range(0, 360))
            ,Vector3.one * m_ExplosionScale);
        var explosion = go.GetComponent<ExplotionFXController>();
        IDisposable disposable = null;
        disposable = explosion.OnTerminateAsObservable().Subscribe(_ =>
        {
            disposable?.Dispose();
            m_ExplosionFxCreator.Release(_);
        }).AddTo(explosion);
        
        
        
        OnTerminate?.Invoke(gameObject);
        m_Shooter = CharacterBase.CharacterType.None;
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

    public IObservable<GameObject> OnTerminateAsObservable()
    {
        return Observable.FromEvent<GameObject>(_e => OnTerminate += _e , _e => OnTerminate -= _e);
    }


}
