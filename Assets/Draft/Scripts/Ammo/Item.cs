using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class Item : MonoBehaviour,IPoolObjectEvent
{
    public enum ItemType
    {
        Missile,Shield 
    }
    private Transform _Transform;
    [SerializeField]private ItemType m_ItemType = ItemType.Missile;
   
    void Start()
    {
       
    }

    private Vector3 ClampAreaScreen(Vector3 _input)
    {
        var areaHeight = 2f * Camera.main.orthographicSize;
        var areaWidth = areaHeight * Camera.main.aspect;
        var topPos = (areaHeight / 2f) - 0.5f;
        var bottomPos = -(areaHeight / 2f) + 0.5f;
        var leftPos = -(areaWidth / 2f) + 0.5f;
        var rightPos = (areaWidth / 2f) - 0.5f;

        _input.x = Mathf.Clamp(_input.x, leftPos, rightPos);
        _input.y = Mathf.Clamp(_input.y, bottomPos, topPos);
        return _input;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.gameObject.GetComponent<PlayerController>();
        if (player)
        {
            if(m_ItemType == ItemType.Missile)
                PlayerController.Instance.EnableMissileProjectile();
            else if(m_ItemType == ItemType.Shield)
                PlayerController.Instance.EnableShield();

            ObjectPoolingManager.Kill(gameObject);
        }
    }

    private IDisposable UpdateDisposable;
    public void OnStartObject()
    {
        _Transform = transform;

        _Transform.position = ClampAreaScreen(transform.position);

        var areaHeight = 2f * Camera.main.orthographicSize;
        var areaWidth = areaHeight * Camera.main.aspect;
        var bottomPos = -(areaHeight / 2f) -0.5f;

        UpdateDisposable =  Observable.EveryUpdate().Subscribe(_ =>
        { 
            _Transform.position += Vector3.down  * Time.deltaTime;
            
            _Transform.Rotate(Vector3.forward, 100f * Time.deltaTime);

            if (_Transform.position.y < bottomPos)
            {
                ObjectPoolingManager.Kill(gameObject);
            }
        }).AddTo(this);
    }

    public void OnEndObject()
    {
        UpdateDisposable?.Dispose();
    }
}