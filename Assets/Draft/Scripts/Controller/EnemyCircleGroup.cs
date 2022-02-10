using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Modules.Utilities;
using UniRx;
using UnityEngine;

public class EnemyCircleGroup : MonoBehaviour
{
    [SerializeField] private GameObject m_Prefab;

    [SerializeField] private float m_Radius = 2f;
    [SerializeField] private int m_Max = 6;
    [SerializeField] private float m_Speed = 16f;

    private float _Axis = 0;
    private float _AnglePart = 0;
    private Transform _Transform;

    private List<EnemyController> m_EnemyList;

    private int DieCount = 0;
    void Start()
    {
        _Transform = transform;
        _AnglePart = 360f / m_Max;
        m_EnemyList = new List<EnemyController>(m_Max);
        for (int i = 0; i < m_Max; i++)
        {
            var rot = Quaternion.Euler(0, 0, (_AnglePart * i) + _Axis);
            var go = ObjectPoolingManager.CreateObject($"enemy{gameObject.GetInstanceID()}", m_Prefab, Vector3.zero, Quaternion.identity, null);
            go.transform.position = rot * new Vector3(0, m_Radius, 0) + _Transform.position;
            go.name = $"enemy{i}";
            m_EnemyList.Add(go.GetComponent<EnemyController>());
        }
    }


    private void Update()
    {
        DieCount = m_EnemyList.Count(_ => _.m_Health <= 0);

        if (DieCount >= m_Max)
        {
            for (int i = 0; i < m_EnemyList.Count; i++)
            {
                var enemy = m_EnemyList[i];
                enemy.m_Health = 3000;
                enemy.gameObject.SetActive(true);
                DieCount--;
            }
        }
        
        _Axis += Time.deltaTime * m_Speed;
        if (_Axis >= 360) _Axis = 0;
        for (int i = 0; i < m_EnemyList.Count; i++)
        {
            var enemy = m_EnemyList[i];
            if (enemy.m_Health <= 0 || !enemy.gameObject.activeSelf)
            {
                continue;
            }
            var angle = (_AnglePart * i) + _Axis;
            var rot = Quaternion.Euler(0, 0, angle);
            enemy.m_Transform.position = rot * new Vector3(0, m_Radius, 0) + _Transform.position;
        }
    }

    private IDisposable _ShootDisposable;

    public void ShootPattern1()
    {
        _ShootDisposable?.Dispose();
        var index = 0;
        var repeatTime = 0.02f;
        var countDown = 0f;
        var loopCount = 0;
        var loopTarget = 2;
        _ShootDisposable = Observable.EveryUpdate().Subscribe(_ =>
        {
            if(m_EnemyList.Count == 0)_ShootDisposable?.Dispose();
            
            countDown -= Time.deltaTime;
            _Axis += Time.deltaTime * m_Speed * 2f;
            if (countDown <= 0)
            {
                if (index >= m_EnemyList.Count)
                {
                    index = 0;
                    countDown = repeatTime;
                    loopCount++;
                    if (loopCount >= loopTarget)
                        _ShootDisposable?.Dispose();
                }
                else
                {
                    countDown = repeatTime;
                }
                
                if(m_EnemyList.Count == 0)_ShootDisposable?.Dispose();
                m_EnemyList[index].Shoot();
                index++;
            }
        }).AddTo(this);
    }
    private IDisposable _ScaleAnimationDisposable;
    private float _OldRadius;
    public void UpRadius()
    {
        _OldRadius = m_Radius;
        _ScaleAnimationDisposable?.Dispose();
        _ScaleAnimationDisposable = LerpThread.FloatLerp(500, m_Radius, _OldRadius+1).Subscribe(_ =>
        {
            m_Radius = _;
        }).AddTo(this);
    }
    public void RobackRadius()
    {
        _ScaleAnimationDisposable?.Dispose();
        _ScaleAnimationDisposable = LerpThread.FloatLerp(500, m_Radius, _OldRadius).Subscribe(_ =>
        {
            m_Radius = _;
        }).AddTo(this);
    }
    
}