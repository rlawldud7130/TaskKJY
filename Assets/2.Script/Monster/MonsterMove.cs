using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.UI.Image;

//���̷� ��� �˾Ƴ� ������ �������� ����ü
public class RayImfo
{
    public int monsterNum;
    public bool bBox;
    public bool bLand;
}

public class MonsterMove : MonoBehaviour
{
    [HideInInspector] public int layer;

    private LayerMask objLayerMask;

    private float moveSpeed = -2f;
    private Vector2 moveVec = new Vector2(1, 0);
    private Vector2 deltaMoveVec = new Vector2(1, 0);
    private Rigidbody2D rb;
    private Animator animator;
    private string monsterLayer;
    private string boxLayer;
    private string jumpLayer;
    private string landLayer;

    private float preTime;

    void Update()
    {
        Move();
    }

    //Ǯ������ ��ŸƮ�� �ִ��� ����� ���α�
    public void ResetMonster()
    {
        rb = this.GetComponent<Rigidbody2D>();
        animator = this.GetComponent<Animator>();

        //���̺� �޴������� �������� �ο����� ���̾�� ���Ͱ� �浹ó���� ���̾�� ����
        objLayerMask = (1 << (6 + layer)) | (1 << (9 + layer)) | (1 << (15 + layer));
        monsterLayer = "MonsterLayer" + layer.ToString();
        boxLayer = "BoxLayer" + layer.ToString();
        jumpLayer = "JumpLayer" + layer.ToString();
        landLayer = "LandLayer" + layer.ToString();

        this.gameObject.layer = LayerMask.NameToLayer(monsterLayer);

        preTime = Time.time;
        moveVec = new Vector2(1, 0);
        StartCoroutine(CheakObj());
    }

    private void Move()
    {
        MoveVec(deltaMoveVec.x, deltaMoveVec.y * -1, deltaTime);
        Debug.DrawLine(this.transform.position, this.transform.position + (Vector3)moveVec * -1, Color.blue);
        rb.velocity = moveVec * moveSpeed;
    }

    private float jumpTimer = 0.0f;
    private float downTimer = 0.0f;

    private float deltaTime = 0.0f;

    //����ȭ) ����Ž���� 0.3�ʿ� �ѹ������� ����! �� �����ӿ� ����ؾ� �ϴ� �� ���̱� ������ ��������Ƿ� ����Ž�� �Լ����� ������Ű��
    IEnumerator CheakObj()
    {
        while (true)
        {
            deltaTime = Time.time - preTime;

            //���� �� �������� ���
            RayImfo left = Ray((Vector2)transform.position + new Vector2(-0.5f, 0.2f), new Vector2(-1.0f, 0.0f), 0.2f);
            RayImfo top = Ray((Vector2)transform.position + new Vector2(-0.15f, 0.7f), new Vector2(0.0f, 1.0f), 0.5f);
            RayImfo bottom = Ray((Vector2)transform.position + new Vector2(-0.25f, 0.0f), new Vector2(0.0f, -0.3f), 0.3f);
            RayImfo leftTop = Ray((Vector2)transform.position + new Vector2(-0.5f, 0.5f), new Vector2(-0.7f, 1.0f), 0.5f);
            RayImfo leftBottom = Ray((Vector2)transform.position + new Vector2(-0.5f, 0.0f), new Vector2(-1.0f, -1.0f), 0.5f);

            //���ʿ� �ڽ��� �ִٸ� ������ ���߰� ����
            if (left.bBox)
            {
                deltaMoveVec.x = 0.0f;
                deltaMoveVec.y = 0.0f;
                animator.SetTrigger("Attack");
            }

            //���ʿ� ���Ͱ� 0.4�� �̻� �ִٸ� ����
            if (left.monsterNum > 0)
            {
                jumpTimer += deltaTime;
                if (jumpTimer >= 0.4f)
                {
                    deltaMoveVec.x = 1.0f;
                    deltaMoveVec.y = 1.5f;
                    animator.SetTrigger("Jump");
                    gameObject.layer = LayerMask.NameToLayer(jumpLayer); //�����ϴ� ���͸� ������ �����ϴ� �ٸ� ���Ͱ� ����°� ����
                }
            }
            else
            {
                //���ʿ� ���Ͱ� ���ٸ� ����Ÿ�̸� �ʱ�ȭ
                if (jumpTimer != 0.0f)
                {
                    jumpTimer = 0.0f;
                    gameObject.layer = LayerMask.NameToLayer(monsterLayer);
                }

                deltaMoveVec.x = 1.0f;
                deltaMoveVec.y = 0.0f;
            }

            //���� �������� ���Ͱ� 2���̻� ��������� �������� �̲�������
            if (top.monsterNum > 0 || leftTop.monsterNum > 0)
            {
                downTimer += deltaTime;
                if (downTimer >= 2.0f)
                {
                    animator.SetTrigger("Down");
                    deltaMoveVec.x = -1.0f;
                }
            } //���Ͱ� ���ʿ� ���� ���ʿ� �ڽ��� ������ �Ͼ��
            else if (left.monsterNum == 0 && !left.bBox)
            {
                animator.SetTrigger("Up");
                deltaMoveVec.x = 1.0f;
                downTimer = 0.0f;
            }

            //�� �ൿ���� �ڿ������� �������� ��Ű��

            //���� �ƹ��͵� ������ �׳� �ٷ� ����Ʈ����
            if (!bottom.bLand && bottom.monsterNum == 0 && left.monsterNum == 0 && leftBottom.monsterNum == 0)
            {
                moveVec = deltaMoveVec = new Vector2(0, -2);
            }

            preTime = Time.time;

            yield return new WaitForSeconds(0.3f);
        }
    }

    //����
    void MoveVec(float x, float y, float deltaTime)
    {
        Vector2 target = new Vector2(x, y);
        float smoothSpeed = 5.0f;
        moveVec = Vector2.Lerp(moveVec, target, deltaTime * smoothSpeed);
    }

    //���̽��
    private RayImfo Ray(Vector2 origin, Vector2 rayVec, float distance)
    {
        int monsterCount = 0;
        bool bBox = false;
        bool bLand = false;

        RayImfo rayImfo = new RayImfo();

        RaycastHit2D[] hits = Physics2D.RaycastAll(origin, rayVec, distance, objLayerMask);
        Debug.DrawLine(origin, origin + rayVec * distance, Color.red);

        for (int i = 0; i < hits.Length; i++)
        {
            string layerName = LayerMask.LayerToName(hits[i].collider.gameObject.layer);

            if (layerName.Equals(boxLayer))
            {
                bBox = true;
            }
            else if(layerName.Equals(monsterLayer))
            {
                monsterCount++;
            }
            else if (layerName.Equals(landLayer))
            {
                bLand = true;
            }
        }

        rayImfo.monsterNum = monsterCount;
        rayImfo.bBox = bBox;
        rayImfo.bLand = bLand;

        return rayImfo;
    }

    public void DieMonster()
    {
        StopCoroutine(CheakObj());
    }
}
