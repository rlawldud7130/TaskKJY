using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroAttack : MonoBehaviour
{
    [SerializeField] private float attackSpeed;
    public GameObject bulletPrefab;
    public float bulletSpeed = 10f;
    private Rigidbody2D rb;
    private GameObject bullet;
    private Animator animator;

    private float attackTimer = 0.0f;

    private void Start()
    {
        animator = this.GetComponent<Animator>();
    }

    void Update()
    {
        FireBullet();
    }

    //ÃÑ½î±â
    private void FireBullet()
    {
        if (attackTimer < 1.0f)
        {
            attackTimer += Time.deltaTime * attackSpeed;
        }

        if (Input.GetMouseButtonDown(0))
        {
            if(attackTimer > 1.0f)
            {
                attackTimer = 0.0f;

                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePos.z = 0f;

                Vector2 direction = (mousePos - transform.position).normalized;

                bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
                rb = bullet.GetComponent<Rigidbody2D>();

                if (rb != null)
                {
                    rb.velocity = direction * bulletSpeed;
                }

                animator.SetTrigger("Attack");
            }
        }
    }
}
