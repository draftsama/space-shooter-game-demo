using System;
using System.Collections;
using System.Collections.Generic;
using Modules.Utilities;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

public class UIPauseMenu : Singleton<UIPauseMenu>
{
    [SerializeField] private CanvasGroup m_CanvasGroup;
    [SerializeField] private RectTransform m_Line;
    [SerializeField] private Button m_ResultBtn;
    [SerializeField] private Button m_ExitBtn;

    private bool _IsShow;
    private const float _LINE_WIDTH_START = 100f;
    private const float _LINE_WIDTH_TARGET = 790f;
    public bool IsReady { get; private set; }

    void Start()
    {
        IsReady = true;
        m_Line.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _LINE_WIDTH_START);

        var resultSubmit = m_ResultBtn.OnSubmitAsObservable().AsUnitObservable();
        var resultClick = m_ResultBtn.OnClickAsObservable();
        resultSubmit.Merge(resultClick).Subscribe(_ =>GameController.Instance.PauseGame(false)).AddTo(this);
        
        
        var exitSubmit = m_ExitBtn.OnSubmitAsObservable().AsUnitObservable();
        var exitClick = m_ExitBtn.OnClickAsObservable();
        exitSubmit.Merge(exitClick).Subscribe(_ =>GameController.Instance.ExitGame()).AddTo(this);
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    private CompositeDisposable _CompositeDisposable;

    public void ShowMenu()
    {
        if (_IsShow || !IsReady) return;
        IsReady = false;
        m_ResultBtn.Select();
        _CompositeDisposable?.Dispose();
        _CompositeDisposable = new CompositeDisposable();
        _CompositeDisposable.Add(m_CanvasGroup.LerpAlpha(500, GlobalConstant.ALPHA_VALUE_VISIBLE, true, () =>
        {
            _IsShow = true;
            IsReady = true;
        }));
        _CompositeDisposable.Add(m_Line.LerpWidth(_LINE_WIDTH_TARGET, 500, Easing.Ease.EaseInOutQuad, true));
    }

    public void HideMenu(Action _oncompleted = null)
    {
        if (!_IsShow || !IsReady) return;

        IsReady = false;
        _CompositeDisposable?.Dispose();
        _CompositeDisposable = new CompositeDisposable();
        _CompositeDisposable.Add(m_CanvasGroup.LerpAlpha(500, GlobalConstant.ALPHA_VALUE_INVISIBLE, true, () =>
        {
            IsReady = true;
            _IsShow = false;
            _oncompleted?.Invoke();
            m_Line.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _LINE_WIDTH_START);
        }));
    }
}