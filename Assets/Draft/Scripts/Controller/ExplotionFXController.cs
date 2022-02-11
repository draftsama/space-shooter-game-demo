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
        m_Animator = GetComponent<Animator>();
        var trigger = m_Animator.GetBehaviour<ObservableStateMachineTrigger>();

        if (trigger != null)
        {
            _Disposable?.Dispose();
            _Disposable = trigger.OnStateExitAsObservable().Subscribe(_ =>
            {
                if (OnTerminate == null)
                    gameObject.SetActive(false);
                else
                    OnTerminate?.Invoke(gameObject);
            }).AddTo(this);
        }
    }

    private Action<GameObject> OnTerminate;

    public IObservable<GameObject> OnTerminateAsObservable()
    {
        return Observable.FromEvent<GameObject>(_e => OnTerminate += _e, _e => OnTerminate -= _e);
    }
}