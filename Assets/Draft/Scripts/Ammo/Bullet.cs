using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : AmmoBase
{
    protected  override void FixedUpdate()
    {
        m_Rigidbody2D.velocity =_Transform.up * m_Speed * Time.deltaTime;

        base.FixedUpdate();
       
    }

    
   
}
