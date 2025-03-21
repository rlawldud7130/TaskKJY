using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UIElements;
using static UnityEngine.UI.Image;



public class MonsterManager : MonoBehaviour
{
    public float Maxhealth = 10;
    private float health;

    public float damageToTower = 1;

    public Transform targetObject;
    public Vector3 offset;

    [HideInInspector] public PooledObject pooledObject;
    private Animator animator;
    private DamageUiManager damageUiManager;


    public void ResetMonster()
    {
        health = Maxhealth;
        animator = this.GetComponent<Animator>();
        damageUiManager = GameObject.Find("Damage UI Manager").GetComponent<DamageUiManager>();
        this.GetComponent<MonsterMove>().ResetMonster();
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
        this.GetComponent<MonsterMove>().DieMonster();
        StartCoroutine("DestroyMonster");
    }

    //풀에 넣어서 부활준비
    IEnumerator DestroyMonster()
    {
        yield return new WaitForSeconds(0.4f);
        this.transform.position = new Vector3(0.0f, -100.0f, 0.0f);
        health = Maxhealth;

        yield return new WaitForSeconds(2.0f);
        pooledObject.ReturnToPool();
    }
}