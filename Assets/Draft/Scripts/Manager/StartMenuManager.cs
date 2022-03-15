using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using Modules.Utilities;
using UnityEngine;
using UnityEngine.Events;

namespace Draft.Manager
{
    public class StartMenuManager : MonoBehaviour
    {
        [SerializeField] private Transform m_SpaceShipTransform;
        [SerializeField] private float m_Speed = 10f;
        [SerializeField] private float m_Radius = 2f;
        [SerializeField] private UnityEngine.UI.Button m_Button;

        private Action<int> _Action;

        private int _Number = 0;
        void Start()
        {
            var token = this.GetCancellationTokenOnDestroy();
            AudioManager.PlayBGM("start.menu", 0.5f);

            m_SpaceShipTransform.FloatingAnimationAsyncEnumerable(m_Speed, m_Radius).Subscribe(_ =>
                {
                    _Number++;
                    _Action?.Invoke(_Number);
                })
                .AddTo(token);
            UniTaskAsyncEnumerable.EveryUpdate().Where(_ => Input.anyKeyDown).ForEachAsync(_ =>
            {
                SceneLoaderManager.Instance.GotoGame();
            }, cancellationToken: token);
            
        }

      

    }

}