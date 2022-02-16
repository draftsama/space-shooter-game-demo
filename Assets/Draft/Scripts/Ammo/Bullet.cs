using UnityEngine;

namespace Draft.Ammo
{
    public class Bullet : AmmoBase
    {
        protected  override void FixedUpdate()
        {
            m_Rigidbody2D.velocity =_Transform.up * m_Speed * Time.deltaTime;

            base.FixedUpdate();
       
        }
        
    }
}
