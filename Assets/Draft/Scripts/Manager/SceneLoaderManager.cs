using System;
using System.Collections;
using System.Collections.Generic;
using Modules.Utilities;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Draft.Manager
{
    public class SceneLoaderManager : Singleton<SceneLoaderManager>
    {

        private const string _GAME_SCENE = "gameplay";
        private const string _MENU_SCENE = "menu";

        [SerializeField] private CanvasGroup m_CanvasGroup;

        private bool _IsLoading = false;

        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(this);
        }


        void Start()
        {
            Observable.FromCoroutine(() => SceneLoading(_MENU_SCENE)).Subscribe().AddTo(this);
        }

        public void GotoGame()
        {
            if (_IsLoading) return;
            _IsLoading = true;
            LoadScene(_GAME_SCENE);
        }

        public void GotoMenu()
        {
            Debug.Log("GotoMenu");
            if (_IsLoading) return;
            _IsLoading = true;
            LoadScene(_MENU_SCENE);
        }

        private void LoadScene(string _sceneName)
        {
            m_CanvasGroup.LerpAlpha(1000, GlobalConstant.ALPHA_VALUE_VISIBLE,
                _onComplete: () =>
                {
                    Observable.FromCoroutine(() => SceneLoading(_sceneName)).Subscribe(_ =>
                    {
                        m_CanvasGroup.LerpAlpha(1000, GlobalConstant.ALPHA_VALUE_INVISIBLE,
                            _onComplete: () => { _IsLoading = false; }).AddTo(this);
                    }).AddTo(this);
                }).AddTo(this);
        }

        IEnumerator SceneLoading(string _sceneName)
        {
            var asyncOperation = SceneManager.LoadSceneAsync(_sceneName, LoadSceneMode.Single);
            while (!asyncOperation.isDone)
            {
                yield return null;
            }
        }

    }
}