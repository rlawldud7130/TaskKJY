using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage = 10;
    public LayerMask hitLayer;

    private Rigidbody2D rb;
    private Vector2 previousPosition;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        previousPosition = rb.position;
    }

    //레이로 총알 쏜거 맞는판정 확인
    private void FixedUpdate()
    {
        Vector2 currentPosition = rb.position;
        Vector2 direction = currentPosition - previousPosition;
        float distance = direction.magnitude;

        if (distance > 0)
        {
            RaycastHit2D hit = Physics2D.Raycast(previousPosition, direction.normalized, distance, hitLayer);
            if (hit.collider != null)
            {
                MonsterManager monsterManager = hit.collider.GetComponent<MonsterManager>();
                if (monsterManager != null)
                {
                    monsterManager.Hit(damage);
                }

                Destroy(gameObject);
                return;
            }
        }

        previousPosition = currentPosition;
    }
}
