using System;
using System.Collections;
using System.Collections.Generic;
using Modules.Utilities;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameController : Singleton<GameController>
{
    void Start()
    {
        Application.targetFrameRate = 60;
        Observable.EveryUpdate().Where(_ => Input.GetKeyDown(KeyCode.Escape)).Subscribe(_ =>
        {
            IsPause = !IsPause;
            PauseGame(IsPause);
        }).AddTo(this);
        // ObjectPoolingManager.CreateObject("enemy", m_EnemyPrefab, Vector3.right + Vector3.up, Quaternion.identity).name = "right";
        // ObjectPoolingManager.CreateObject("enemy", m_EnemyPrefab, Vector3.left+ Vector3.up, Quaternion.identity).name = "left";
    }

    public bool IsPause { private set; get; }
    private float _FixedDeltaTime;
    void PauseGame(bool _pause)
    {
        IsPause = _pause;
        if (_pause)
        {
            Time.timeScale = 0;
            _FixedDeltaTime = Time.fixedDeltaTime;
            Time.fixedDeltaTime = 0;
        }
        else
        {
            Time.timeScale = 1;
            Time.fixedDeltaTime = _FixedDeltaTime;

        }
    }

    private void OnDestroy()
    {
        Instance = null;
    }
}
