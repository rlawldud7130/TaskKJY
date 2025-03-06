using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage = 10;
    public LayerMask hitLayer;

    //총알이 몬스터 때리기
    private void OnTriggerEnter2D(Collider2D other)
    {
        if ((hitLayer.value & (1 << other.gameObject.layer)) != 0)
        {
            other.GetComponent<MonsterManager>().Hit(damage);
            Destroy(this.gameObject);
        }
    }
}
