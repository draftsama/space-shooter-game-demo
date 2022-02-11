using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class ExplotionFXController : MonoBehaviour
{
    private Animator m_Animator;
    private IDisposable _Disposable;

    void OnEnable()
    {
        transform.rotation = Quaternion.Euler(0, 0, UnityEngine.Random.Range(0, 360));
        m_Animator = GetComponent<Animator>();
        var trigger = m_Animator.GetBehaviour<ObservableStateMachineTrigger>();

        if (trigger != null)
        {
            _Disposable?.Dispose();
            _Disposable = trigger.OnStateExitAsObservable().Subscribe(_ =>
            {
                ObjectPoolingManager.Kill(gameObject);
            }).AddTo(this);
        }
    }

  
}