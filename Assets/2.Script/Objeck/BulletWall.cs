using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletWall : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        //ƨ�ܳ��� �Ѿ� �����
        if (other.gameObject.layer == LayerMask.NameToLayer("Bullet"))
        {
            Destroy(other.gameObject);
        }
    }
}
