using System;
using System.Collections;
using System.Collections.Generic;
using Modules.Utilities;
using UniRx;
using UnityEngine;

public class StartMenuManager : MonoBehaviour
{
    [SerializeField]private Transform m_SpaceShipTransform;
    [SerializeField]private float m_Speed = 10f;
    [SerializeField]private float m_Radius = 2f;
    
    void Start()
    {
        FloatingAnimation(m_SpaceShipTransform,m_Speed, m_Radius).AddTo(this);
        Observable.EveryUpdate().Where(_ => Input.anyKeyDown).Subscribe(_ =>
        {
            SceneLoaderManager.Instance.GotoGame();
        }).AddTo(this);
    }
    
    public  IDisposable FloatingAnimation(Transform _transform, float _speed, float _radius, Easing.Ease _ease = Easing.Ease.EaseInOutQuad,bool _useUnscaleTime = false)
    {

        float progress = 0;

        Vector2 currentPos = _transform.position;
        Vector2 startPos = _transform.position;
        Vector2 targetPos = RandomPosition() + startPos;

        Vector2 RandomPosition()
        {
            float angle = UnityEngine.Random.Range(0, 360);
            float randomRadius = UnityEngine.Random.Range(0, _radius);
            return Quaternion.Euler(0, 0, angle) * new Vector3(0, randomRadius, 0);
        }
        return Observable.EveryUpdate().Subscribe(_ =>
        {
            var deltaTime = _useUnscaleTime ? Time.unscaledDeltaTime : Time.deltaTime;
            progress += deltaTime * _speed * 0.1f;
            _transform.position = EasingFormula.EaseTypeVector(Easing.Ease.EaseInOutQuad, currentPos, targetPos, progress);


            if (progress >= 1)
            {
                targetPos = RandomPosition() + startPos;
                currentPos = _transform.position;
                progress = 0;
            }
        });
    }

   
}
