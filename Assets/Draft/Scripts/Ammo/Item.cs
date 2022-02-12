using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    private Rigidbody2D m_Rigidbody2D;
    void Start()
    {
        transform.position = ClampAreaScreen(transform.position);
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        m_Rigidbody2D.angularVelocity = 45f;
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
            PlayerController.Instance.EnableMissileProjectile();
            ObjectPoolingManager.Kill(gameObject);
        }
    }
}
