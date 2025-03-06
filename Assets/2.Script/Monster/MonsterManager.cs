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

    private Animator animator;

    private void Start()
    {
        animator = this.transform.GetComponent<Animator>();
    }

    //몬스터가 데미지 입게 할때
    public void Hit(float damage)
    {
        health -= damage;

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
        Debug.Log(1);
        yield return new WaitForSeconds(0.4f);

        Destroy(this.gameObject);
        yield return null;
    }
}