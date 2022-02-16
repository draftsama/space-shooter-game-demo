
using System;
using UniRx;
using UnityEngine;


    public abstract class GameEventBase : MonoBehaviour
    {
        protected Action<Unit> OnStopEvent;

        public virtual void StartEvent()
        {

        }

        public virtual void StopEvent()
        {
            OnStopEvent?.Invoke(default);
        }

        public IObservable<Unit> OnStopEventAsObservable()
        {
            return Observable.FromEvent<Unit>(_e => OnStopEvent += _e, _e => OnStopEvent -= _e);
        }
    }

