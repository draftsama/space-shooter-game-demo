using UnityEngine;

namespace Draft.Ammo
{
    public class Missile : AmmoBase
    {
        [SerializeField] private CharacterBase m_Target;
        [SerializeField] private float m_RotationSpeed = 50f;

        public void SetTarget(CharacterBase _target)
        {
            m_Target = _target;
        }

        protected override void FixedUpdate()
        {

            var up = _Transform.up;
            m_Rigidbody2D.velocity = up * m_Speed * Time.fixedDeltaTime;

            if (m_Target != null && m_Target.IsAlive())
            {
                var direction = (Vector2)m_Target.m_Transform.position - m_Rigidbody2D.position;
                var rotAmount = -Vector3.Cross(direction.normalized, up).z;
                m_Rigidbody2D.angularVelocity = rotAmount * m_RotationSpeed;
            }
            else
            {
                m_Rigidbody2D.angularVelocity = 0;
            }

            base.FixedUpdate();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var ammo = other.gameObject.GetComponent<AmmoBase>();
            if (ammo != null && m_Shooter == CharacterBase.CharacterType.Enemy &&
                ammo.GetShooterType() == CharacterBase.CharacterType.Player)
            {
                ammo.Terminate();
                Terminate();

            }
        }
    }
}
