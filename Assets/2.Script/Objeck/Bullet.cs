using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage = 10;
    public LayerMask hitLayer;

    //�Ѿ��� ���� ������
    private void OnTriggerEnter2D(Collider2D other)
    {
        if ((hitLayer.value & (1 << other.gameObject.layer)) != 0)
        {
            other.GetComponent<MonsterManager>().Hit(damage);
            Destroy(this.gameObject);
        }
    }
}
