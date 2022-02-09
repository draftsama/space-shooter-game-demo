using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public enum  Whose
    {
        None,Player,Enemy
    }
    [SerializeField]private Whose m_Whose = Whose.None;
    [SerializeField]private float m_Speed = 300f;
    [SerializeField]private GameObject m_ExplosionFx;
    [SerializeField]private float m_ExplosionScale = 0.2f;
    private Rigidbody2D m_Rigidbody2D;
    private Transform _Transform;
    void Start()
    {
        _Transform = transform;
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
    }

    public void SetWhose(Whose _whose)
    {
        m_Whose = _whose;
    }
    public Whose GetWhose()
    {
        return m_Whose;
    }
    void FixedUpdate()
    {
        m_Rigidbody2D.velocity = Vector2.up * m_Speed * Time.deltaTime;
        if (!IsOnScreen(m_Rigidbody2D.position))
        {
            ObjectPoolingManager.KillObject(gameObject);
            m_Whose = Whose.None;
        }
    }

    public void Terminate()
    {
       var go = ObjectPoolingManager.CreateObject("fx", m_ExplosionFx, _Transform.position, Quaternion.identity);
       go.transform.localScale = Vector3.one * m_ExplosionScale;
        ObjectPoolingManager.KillObject(gameObject);
        m_Whose = Whose.None;
    }
    
    private bool IsOnScreen(Vector2 _input)
    {
        var areaHeight = 2f * Camera.main.orthographicSize;
        var areaWidth = areaHeight * Camera.main.aspect;
        var topPos = (areaHeight / 2f);
        var bottomPos = -(areaHeight / 2f);
        var leftPos = -(areaWidth / 2f);
        var rightPos = (areaWidth / 2f);
        
        return _input.x > leftPos && _input.x < rightPos && _input.y >bottomPos && _input.y < topPos;
    }
}
