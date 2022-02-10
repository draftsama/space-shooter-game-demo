using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameController : MonoBehaviour
{
    public GameObject m_EnemyPrefab;
    void Start()
    {
        Application.targetFrameRate = 60;

        // ObjectPoolingManager.CreateObject("enemy", m_EnemyPrefab, Vector3.right + Vector3.up, Quaternion.identity).name = "right";
        // ObjectPoolingManager.CreateObject("enemy", m_EnemyPrefab, Vector3.left+ Vector3.up, Quaternion.identity).name = "left";
    }
}
