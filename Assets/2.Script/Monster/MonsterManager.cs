using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UIElements;
using static UnityEngine.UI.Image;



public class MonsterManager : MonoBehaviour
{

    public float health = 10;
    public float damageToTower = 1;

    public Transform targetObject;
    public Vector3 offset;

    private Animator animator;
    private DamageUiManager damageUiManager;

    private void Start()
    {
        animator = this.GetComponent<Animator>();
        damageUiManager = GameObject.Find("Damage UI Manager").GetComponent<DamageUiManager>();
    }

    //몬스터가 데미지 입게 할때
    public void Hit(float damage)
    {
        health -= damage;
        damageUiManager.DamageUI(this.transform.position, damage);

        if(health <= 0)
        {
            Die();
        }
    }

    //몬스터 사망
    private void Die()
    {
        animator.SetTrigger("Die");
        StartCoroutine("DestroyMonster");
    }

    IEnumerator DestroyMonster()
    {
        yield return new WaitForSeconds(0.4f);

        Destroy(this.gameObject);
    }
}