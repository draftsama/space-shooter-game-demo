using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Modules.Utilities;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameController : Singleton<GameController>
{
    [SerializeField]private GameEventManager m_GameEventManager;
    [SerializeField]private Animator m_TextAnimator;
    
    [Header("Audios")]
    [SerializeField]private AudioSource m_BGMAudio;
    [SerializeField]private AudioSource m_BossAudio;

    void Start()
    {
        Application.targetFrameRate = 60;
        m_BGMAudio.Play();
        Observable.EveryUpdate().Where(_ => Input.GetKeyDown(KeyCode.Escape) && UIPauseMenu.Instance.IsReady && !UIResultMenu.Instance.m_IsShowing).Subscribe(_ =>
        {
            IsPause = !IsPause;
            PauseGame(IsPause);
        }).AddTo(this);

       var triggers = m_TextAnimator.GetBehaviours<ObservableStateMachineTrigger>();
       triggers.Select(_ => _.OnStateExitAsObservable()).Merge().Subscribe(_ =>
       {
           if (_.StateInfo.IsName("start"))
           { 
               StartGame();

           }else  if (_.StateInfo.IsName("clear"))
           {
              UIResultMenu.Instance.PlayResult(PlayerController.Instance.m_Score, ExitGame);
           }else  if (_.StateInfo.IsName("fail"))
           {
               ExitGame();
           }
       }).AddTo(this);
       
      
       m_GameEventManager.OnEndEventAsObservable().Subscribe(_ =>
       {
           PlayerController.Instance.MissionClear(() =>
           {
               m_TextAnimator.SetTrigger("clear");
           });

       }).AddTo(this);

       m_GameEventManager.OnUpdateEventAsObservable().Subscribe(_index =>
       {
           if (_index == m_GameEventManager.GetCountEvent() - 1)
           {
               //boss
               PlayBossAudio();
           }
           
       }).AddTo(this);

       PlayerController.Instance.m_ReActiveLifeValue.Subscribe(_life =>
       {
           if (_life <= 0)
           {
               m_TextAnimator.SetTrigger("fail");

           }
       }).AddTo(this);
       
       
       m_TextAnimator.SetTrigger("start");
    }

    public bool IsPause { private set; get; }

    private float _FixedDeltaTime;

    public void StartGame()
    {
        m_GameEventManager.PlayGameEvent();
        PlayerController.Instance.Relife();
        
    }
    
    
    public void PauseGame(bool _pause)
    {

        if (_pause)
        {
            Time.timeScale = 0;
            _FixedDeltaTime = Time.fixedDeltaTime;
            Time.fixedDeltaTime = 0;
            IsPause = _pause;

            UIPauseMenu.Instance.ShowMenu();
        }
        else
        {
            UIPauseMenu.Instance.HideMenu(() =>
            {
                IsPause = _pause;
                Time.timeScale = 1;
                Time.fixedDeltaTime = _FixedDeltaTime;
            });
        }
    }

    private void PlayBossAudio()
    {
        m_BossAudio.volume = 0;
        m_BossAudio.Play();
        LerpThread.FloatLerp(1000, 0, 1, Easing.Ease.Linear).Subscribe(_p =>
        {
            m_BossAudio.volume = _p;
            m_BGMAudio.volume = 1 - _p;
        }, () =>
        {
            m_BGMAudio.Stop();
            
        }).AddTo(this);
    }

    public void ExitGame()
    {
        if (IsPause)
        {
            Time.timeScale = 1;
            Time.fixedDeltaTime = _FixedDeltaTime;
        }

        SceneLoaderManager.Instance.GotoMenu();   
        
    }

    

    private void OnDestroy()
    {
        Instance = null;
    }
}