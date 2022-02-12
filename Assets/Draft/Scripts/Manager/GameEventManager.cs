using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class GameEventManager : MonoBehaviour
{
    [SerializeField]private List<GameEventInfo> m_GameEventInfoList = new List<GameEventInfo>();
    [SerializeField] private int m_Index = 0;
    private GameEventInfo _CurrentEvent;
    void Start()
    {
        m_Index = 0;
        _CurrentEvent = m_GameEventInfoList[m_Index];
        PlayGameEvent(m_Index);
    }

    void PlayGameEvent(int _index)
    {
        _CurrentEvent = m_GameEventInfoList[m_Index];
        Observable.Timer(TimeSpan.FromMilliseconds(_CurrentEvent.m_Delay)).Subscribe(_ =>
        {
            _CurrentEvent.m_GameEventHandler.StartEvent();
            _CurrentEvent.m_GameEventHandler.OnStopEventAsObservable().Subscribe(_ =>
            {          
                _index++;
                if (_index < m_GameEventInfoList.Count)
                {
                    //next
                    Debug.Log("Next Event");
                    PlayGameEvent(_index);
                }
                else
                {
                    
                    Debug.Log("End Event");
                }
            }).AddTo(this);

        }).AddTo(this);
        

    
    }
   
    [System.Serializable]
    public struct  GameEventInfo
    {
        public int m_Delay;
        public GameEventBase m_GameEventHandler;
    }
}
