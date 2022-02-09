using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float m_Speed = 2f;

    [Header("Sprite")] [SerializeField] private Sprite m_SpriteIdle;
    [SerializeField] private Sprite m_SpriteLeft;
    [SerializeField] private Sprite m_SpriteRight;

    private Transform _Transform;
    private Vector2 _UpdatePosition;
    private SpriteRenderer m_SpriteRenderer;

    private Vector2 _InputMovementPos;
    
    void Start()
    {
        _Transform = transform;
        _UpdatePosition = transform.position;
        m_SpriteRenderer = GetComponent<SpriteRenderer>();

        InputManager.Instance.OnInputMovement().Subscribe(_=>_InputMovementPos = _).AddTo(this);
        
        Observable.EveryUpdate().Subscribe(_ =>
        {
            
          
            var movement  = _InputMovementPos;
            movement *= m_Speed * Time.fixedDeltaTime;

            _UpdatePosition += movement;
            _UpdatePosition = ClampAreaScreen(_UpdatePosition);
            _Transform.position = _UpdatePosition;


            if (_InputMovementPos.x > 0)
                m_SpriteRenderer.sprite = m_SpriteRight;
            else if (_InputMovementPos.x < 0)
                m_SpriteRenderer.sprite = m_SpriteLeft;
            else
                m_SpriteRenderer.sprite = m_SpriteIdle;
            
          //  _InputMovementPos = Vector2.zero;
            
        }).AddTo(this);
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
}