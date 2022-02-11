using System.Collections;
using System.Collections.Generic;
using Modules.Utilities;
using UniRx;
using UnityEngine;

public class EnemyDropItemController : MonoBehaviour
{
    [SerializeField] private GameObject m_Prefab;
    [SerializeField] private GameObject m_ItemPrefab;
    [SerializeField] private MovementQueueController m_MovementQueueController;

    private Transform _Transform;
    private GameObject _EnemyObj;
    void Start()
    {
        _Transform = transform;
        Init();
    }

    public void Init()
     {
    //     var g_EnemyObj = ObjectPoolingManager.CreateObject("enemy", m_Prefab, Vector3.zero, Quaternion.identity, null);
    //
    //    var enemy = _EnemyObj.GetComponent<EnemyController>();
    //    enemy.OnTerminateAsObservable().Subscribe(_ =>
    //    {
    //        ObjectPoolingManager.CreateObject(m_ItemPrefab.name,m_ItemPrefab, _Transform.position, Quaternion.identity, null);
    //    }).AddTo(this);
    }

}
    
  