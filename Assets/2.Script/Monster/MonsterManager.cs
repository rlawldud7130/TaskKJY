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

    private void Start()
    {
        health = Maxhealth;
        animator = this.GetComponent<Animator>();
        damageUiManager = GameObject.Find("Damage UI Manager").GetComponent<DamageUiManager>();
    }

    //���Ͱ� ������ �԰� �Ҷ�
    public void Hit(float damage)
    {
        health -= damage;
        damageUiManager.DamageUI(this.transform.position, damage);

        if(health <= 0)
        {
            Die();
        }
    }

    //���� ���
    private void Die()
    {
        animator.SetTrigger("Die");
        StartCoroutine("DestroyMonster");
    }

    //Ǯ�� �־ ��Ȱ�غ�
    IEnumerator DestroyMonster()
    {
        yield return new WaitForSeconds(0.4f);
        health = Maxhealth;
        pooledObject.ReturnToPool();
    }
}