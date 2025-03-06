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
    private Rigidbody2D rb;
    private Animator animator;
    private string monsterLayer;
    private string boxLayer;
    private string jumpLayer;
    private string landLayer;

    void Start()
    {
        rb = this.transform.GetComponent<Rigidbody2D>();
        animator = this.transform.GetComponent<Animator>();

        //���̺� �޴������� �������� �ο����� ���̾�� ���Ͱ� �浹ó���� ���̾�� ����
        objLayerMask = (1 << (6 + layer)) | (1 << (9 + layer)) | (1 << (15 + layer));
        monsterLayer = "MonsterLayer" + layer.ToString();
        boxLayer = "BoxLayer" + layer.ToString();
        jumpLayer = "JumpLayer" + layer.ToString();
        landLayer = "LandLayer" + layer.ToString();

        this.gameObject.layer = LayerMask.NameToLayer(monsterLayer);
    }

    void Update()
    {
        Move();
        CheackObj();
    }

    private void Move()
    {
        Debug.DrawLine(this.transform.position, this.transform.position + (Vector3)moveVec * -1, Color.blue);
        rb.velocity = moveVec * moveSpeed;
    }

    private float jumpTimer = 0.0f;
    private float downTimer = 0.0f;
    private float downTimer2 = 0.0f;
    private void CheackObj()
    {
        //���� �� �������� ���
        RayImfo left = Ray((Vector2)transform.position + new Vector2(-0.5f, 0.2f), new Vector2(-1.0f, 0.0f), 0.2f);
        RayImfo top = Ray((Vector2)transform.position + new Vector2(-0.15f, 0.7f), new Vector2(0.0f, 1.0f), 0.5f);
        RayImfo bottom = Ray((Vector2)transform.position + new Vector2(-0.25f, 0.0f), new Vector2(0.0f, -0.3f), 0.3f);
        RayImfo leftTop = Ray((Vector2)transform.position + new Vector2(-0.5f, 0.5f), new Vector2(-0.7f, 1.0f), 0.5f);
        RayImfo leftBottom = Ray((Vector2)transform.position + new Vector2(-0.5f, 0.0f), new Vector2(-1.0f, -1.0f), 0.5f);


        float moveVecX = moveVec.x;
        float moveVecY = moveVec.y;

        //���ʿ� �ڽ��� �ִٸ� ������ ���߰� ����
        if (left.bBox)
        {
            moveVecX = 0.0f;
            moveVecY = 0.0f;
            animator.SetTrigger("Attack");
        }

        //���ʿ� ���Ͱ� 0.4�� �̻� �ִٸ� ����
        if (left.monsterNum > 0)
        {
            jumpTimer += Time.deltaTime;
            if (jumpTimer >= 0.4f)
            {
                moveVecX = 1.0f;
                moveVecY = 1.5f;
                animator.SetTrigger("Jump");
                gameObject.layer = LayerMask.NameToLayer(jumpLayer); //�����ϴ� ���͸� ������ �����ϴ� �ٸ� ���Ͱ� ����°� ����
            }
        }
        else
        {
            //���ʿ� ���Ͱ� ���ٸ� ����Ÿ�̸� �ʱ�ȭ
            if(jumpTimer != 0.0f)
            {
                jumpTimer = 0.0f;
                gameObject.layer = LayerMask.NameToLayer(monsterLayer);
            }
        }

        //���� �������� ���Ͱ� 2���̻� ��������� �������� �̲�������
        if (top.monsterNum > 0 || leftTop.monsterNum > 0)
        {
            downTimer += Time.deltaTime;
            if (downTimer >= 2.0f)
            {
                downTimer2 += Time.deltaTime;
                if (downTimer2 > 0.5f)
                {
                    downTimer = 0.0f;
                    downTimer2 = 0.0f;
                }
                animator.SetTrigger("Down");
                moveVecX = -1.0f;
            }
        } //���Ͱ� ���ʿ� ���� ���ʿ� �ڽ��� ������ �Ͼ��
        else if (left.monsterNum == 0 && !left.bBox)
        {
            animator.SetTrigger("Up");
            moveVecX = 1.0f;
            downTimer = 0.0f;
        }

        //�� �ൿ���� �ڿ������� �������� ��Ű��
        MoveVec(moveVecX, moveVecY * -1);

        //���� �ƹ��͵� ������ �׳� �ٷ� ����Ʈ����
        if (!bottom.bLand && bottom.monsterNum == 1 && left.monsterNum == 0 && leftBottom.monsterNum == 0)
        {
            moveVecX = 0.0f;
            moveVecY = 2.0f;
            moveVec = new Vector2(moveVecX, moveVecY);
        }

    }

    //����
    void MoveVec(float x, float y)
    {
        Vector2 target = new Vector2(x, y);
        float smoothSpeed = 5.0f;
        moveVec = Vector2.Lerp(moveVec, target, Time.deltaTime * smoothSpeed);
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
}
