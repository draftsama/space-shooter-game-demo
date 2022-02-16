using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Draft.Manager
{
    public class GameEventManager : MonoBehaviour
    {
        [SerializeField] private List<GameEventInfo> m_GameEventInfoList = new List<GameEventInfo>();
        [SerializeField] private int m_Index = 0;
        private GameEventInfo _CurrentEvent;
        private Action<Unit> _OnEndEvent;
        private Action<int> _OnUpdateEvent;

        void Start()
        {
            m_Index = 0;
            _CurrentEvent = m_GameEventInfoList[m_Index];
        }

        public void PlayGameEvent(int _index = 0)
        {
            m_Index = _index;
            _CurrentEvent = m_GameEventInfoList[m_Index];
            Debug.Log(_CurrentEvent.m_GameEventHandler.name);
            IDisposable disposable = null;
            _OnUpdateEvent?.Invoke(m_Index);
            disposable = Observable.Timer(TimeSpan.FromMilliseconds(_CurrentEvent.m_Delay)).Subscribe(_ =>
            {
                disposable?.Dispose();

                _CurrentEvent.m_GameEventHandler.StartEvent();
                disposable = _CurrentEvent.m_GameEventHandler.OnStopEventAsObservable().Subscribe(_ =>
                {
                    disposable?.Dispose();
                    _index++;
                    if (_index < m_GameEventInfoList.Count)
                    {
                        //next
                        Debug.Log($"Next Event {_index}");
                        PlayGameEvent(_index);
                    }
                    else
                    {
                        _OnEndEvent?.Invoke(default);
                        Debug.Log("End Event");
                    }
                }).AddTo(this);
            }).AddTo(this);
        }

        public int GetCountEvent()
        {
            return m_GameEventInfoList.Count;
        }

        public IObservable<Unit> OnEndEventAsObservable()
        {
            return Observable.FromEvent<Unit>(_e => _OnEndEvent += _e, _e => _OnEndEvent -= _e);
        }

        public IObservable<int> OnUpdateEventAsObservable()
        {
            return Observable.FromEvent<int>(_e => _OnUpdateEvent += _e, _e => _OnUpdateEvent -= _e);
        }

        [Serializable]
        public struct GameEventInfo
        {
            public int m_Delay;
            public GameEventBase m_GameEventHandler;
        }
    }
}