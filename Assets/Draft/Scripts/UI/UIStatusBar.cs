using System;
using System.Collections;
using System.Collections.Generic;
using Modules.Utilities;
using UnityEngine;
using TMPro;
using UniRx;

public class UIStatusBar : Singleton<UIStatusBar>
{
    [SerializeField] private TextMeshProUGUI m_LifeText;
    [SerializeField] private TextMeshProUGUI m_ScoreText;
    private float m_Score = 0;
    
    [SerializeField] private float m_multipleText = 1.5f;

    private float m_FontDefaultSize;
    void Start()
    {
        m_FontDefaultSize = m_ScoreText.fontSize;
    }

    public void SetScore(float _score)
    {
        m_ScoreText.text = _score.ToString();
        m_Score = _score;
    }

    private IDisposable _Disposable;
    public void AddScore(float _addScore)
    {
        _Disposable?.Dispose();
        var startScore = m_Score;
        m_Score += _addScore;

        _Disposable =  UpdateTextValue(300, m_ScoreText, startScore, m_Score, m_multipleText).Subscribe(_ =>
        {
        }).AddTo(this);
    }

    public void SetLife(int _life)
    {
        Debug.Log($"SetLife : {_life}");
        m_LifeText.text = $"x {_life.ToString()}" ;
    }

    private IObservable<Unit> UpdateTextValue(int _milliseccond,TextMeshProUGUI _textMeshPro, float _start,float _target,float _sizeMultiple)
    {
        return Observable.Create<Unit>(observer =>
        {
            CompositeDisposable disposable = new CompositeDisposable();
            disposable.Add(LerpFloat(_milliseccond, _start, _target).Subscribe(
                _ =>
                {
                    _textMeshPro.text = Mathf.FloorToInt(_).ToString();
                }, () =>
                {
                   
                }));

            disposable.Add(LerpFloat(_milliseccond, 0, 2).Subscribe(_ =>
            {
                var size = EasingFormula.EasingFloat(Easing.Ease.EaseInOutQuad, m_FontDefaultSize,
                    m_FontDefaultSize*_sizeMultiple, Mathf.PingPong(_, 1));
                _textMeshPro.fontSize = size;
            }, () =>
            {
                observer.OnNext(default);
                observer.OnCompleted();
            }));


            return Disposable.Create(() => { disposable.Dispose(); });
        });
    }

    private IObservable<float> LerpFloat(int _milliseccond, float _start, float _target,
        Easing.Ease _ease = Easing.Ease.EaseInOutQuad)
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

    private void OnDestroy()
    {
        Instance = null;
    }
}