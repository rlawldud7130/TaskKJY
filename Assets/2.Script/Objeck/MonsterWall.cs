using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterWall : MonoBehaviour
{
    [SerializeField] private LayerMask monsterMask;

    //∏ÛΩ∫≈Õ ¥Í¿∏∏È ¡◊¿Ã±‚
    private void OnTriggerEnter2D(Collider2D other)
    {
        if ((monsterMask.value & (1 << other.gameObject.layer)) != 0)
        {
            other.GetComponent<MonsterManager>().Hit(10000);
        }
    }
}
