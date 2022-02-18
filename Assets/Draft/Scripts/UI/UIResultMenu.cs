using System;
using System.Collections;
using System.Collections.Generic;
using Modules.Utilities;
using UniRx;
using UnityEngine;
using TMPro;

namespace Draft.UI
{
    public class UIResultMenu : Singleton<UIResultMenu>
    {
        [SerializeField] private CanvasGroup m_CanvasGroup;
        [SerializeField] private RectTransform m_LineTransform;
        [SerializeField] private RectTransform m_TextTransform;
        [SerializeField] private TextMeshProUGUI m_ScoreText;

        private IDisposable _Disposable;

        private const float _LINE_WIDTH_START = 0f;
        private const float _LINE_WIDTH_TARGET = 790f;
        private const float _TEXT_POS_Y_START = -113f;
        private const float _TEXT_POS_Y_TARGET = -28f;

        public bool m_IsShowing;

        void Start()
        {
            m_LineTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _LINE_WIDTH_START);
            m_TextTransform.anchoredPosition = Vector2.up * _TEXT_POS_Y_START;
            m_CanvasGroup.SetAlpha(GlobalConstant.ALPHA_VALUE_INVISIBLE);

        }

        private void OnDestroy()
        {
            _Disposable?.Dispose();
            Instance = null;
        }

        public void PlayResult(int _score, Action _oncompleted)
        {
            Debug.Log($"PlayResult {_score}");
            m_IsShowing = true;
            _Disposable?.Dispose();
            _Disposable = m_CanvasGroup.LerpAlpha(500, GlobalConstant.ALPHA_VALUE_VISIBLE, _ignoreTimeScale: true,
                () =>
                {
                    _Disposable = m_LineTransform.LerpWidth(_LINE_WIDTH_TARGET, 500, Easing.Ease.EaseInOutQuad, true,
                        () =>
                        {
                            _Disposable = m_TextTransform.LerpAnchorPosition(Vector2.up * _TEXT_POS_Y_TARGET, 800,
                                _useUnscaleTime: true, _completed:
                                () =>
                                {
                                    _Disposable = UpdateTextValue(5000, m_ScoreText, 0f, (float)_score)
                                        .Subscribe(_ =>
                                        {
                                            _Disposable = Observable.Timer(TimeSpan.FromMilliseconds(3000))
                                                .Subscribe(_ => _oncompleted?.Invoke()).AddTo(this);
                                        }).AddTo(this);
                                }).AddTo(this);
                        }).AddTo(this);
                }).AddTo(this);


        }

        private IObservable<Unit> UpdateTextValue(int _milliseccond, TextMeshProUGUI _textMeshPro, float _startValue,
            float _targetValue)
        {
            return Observable.Create<Unit>(observer =>
            {
                CompositeDisposable disposable = new CompositeDisposable();
                disposable.Add(LerpFloat(_milliseccond, _startValue, _targetValue).Subscribe(
                    _ => { _textMeshPro.text = Mathf.FloorToInt(_).ToString(); }, () =>
                    {
                        observer.OnNext(default);
                        observer.OnCompleted();
                    }));

                return Disposable.Create(() => { disposable.Dispose(); });
            });
        }

        private IObservable<float> LerpFloat(int _milliseccond, float _start, float _target,
            Easing.Ease _ease = Easing.Ease.Linear)
        {
            return Observable.Create<float>(_obserser =>
            {
                float progress = 0;

                IDisposable disposable = LerpThread.Execute(_milliseccond, _ =>
                {
                    progress += Time.deltaTime / (_milliseccond / 1000f);

                    _obserser.OnNext(EasingFormula.EasingFloat(_ease, _start, _target, progress));
                }, () =>
                {
                    _obserser.OnNext(_target);
                    _obserser.OnCompleted();
                });

                return Disposable.Create(() => { disposable?.Dispose(); });
            });
        }
    }

}