using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FxCreator : MonoBehaviour
{
    private Transform m_Target;
    [SerializeField] private GameObject m_Prefab;
    [SerializeField] private float m_Size;

    private void Awake()
    {
        m_Target = transform;
    }
    
    public void CreateNow()
    {
        var go = ObjectPoolingManager.CreateObject(m_Prefab.name, m_Prefab, m_Target.position, m_Target.rotation);
        go.transform.localScale = Vector3.one * m_Size;
    }

   
}