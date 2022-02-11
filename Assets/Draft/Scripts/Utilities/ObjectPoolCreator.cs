using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectPoolCreator : MonoBehaviour
{
    [SerializeField] private GameObject m_Prefab;

    private void Awake()
    {
    }


    public void Release(GameObject _gameObject)
    {
        ObjectPoolingManager.Kill(_gameObject);
    }

    public GameObject Get()
    {
        return Get(m_Prefab.transform.position, m_Prefab.transform.rotation, m_Prefab.transform.localScale, false);
    }

    public GameObject Get(Vector3 _position, Quaternion _rotation, Vector3 _scale, bool isLocal = false)
    {
        var go = ObjectPoolingManager.CreateObject(m_Prefab.name, m_Prefab, _position, _rotation, null);
        if (isLocal)
        {
            go.transform.localPosition = _position;
            go.transform.localRotation = _rotation;
            go.transform.localScale = _scale;
        }
        else
        {
            go.transform.position = _position;
            go.transform.rotation = _rotation;
            var parent = go.transform.parent;
            go.transform.SetParent(null);
            go.transform.localScale = _scale;
            go.transform.SetParent(parent, true);
        }
        go.SetActive(true);
        return go;
    }
}