using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.UI.Image;

//레이로 쏘고 알아낸 정보들 가져오는 구조체
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

        //웨이브 메니저에서 랜덤으로 부여받은 레이어로 몬스터가 충돌처리할 레이어들 세팅
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
        //레이 각 방향으로 쏘기
        RayImfo left = Ray((Vector2)transform.position + new Vector2(-0.5f, 0.2f), new Vector2(-1.0f, 0.0f), 0.2f);
        RayImfo top = Ray((Vector2)transform.position + new Vector2(-0.15f, 0.7f), new Vector2(0.0f, 1.0f), 0.5f);
        RayImfo bottom = Ray((Vector2)transform.position + new Vector2(-0.25f, 0.0f), new Vector2(0.0f, -0.3f), 0.3f);
        RayImfo leftTop = Ray((Vector2)transform.position + new Vector2(-0.5f, 0.5f), new Vector2(-0.7f, 1.0f), 0.5f);
        RayImfo leftBottom = Ray((Vector2)transform.position + new Vector2(-0.5f, 0.0f), new Vector2(-1.0f, -1.0f), 0.5f);


        float moveVecX = moveVec.x;
        float moveVecY = moveVec.y;

        //왼쪽에 박스가 있다면 움직임 멈추고 공격
        if (left.bBox)
        {
            moveVecX = 0.0f;
            moveVecY = 0.0f;
            animator.SetTrigger("Attack");
        }

        //왼쪽에 몬스터가 0.4초 이상 있다면 점프
        if (left.monsterNum > 0)
        {
            jumpTimer += Time.deltaTime;
            if (jumpTimer >= 0.4f)
            {
                moveVecX = 1.0f;
                moveVecY = 1.5f;
                animator.SetTrigger("Jump");
                gameObject.layer = LayerMask.NameToLayer(jumpLayer); //점프하는 몬스터를 잡으며 점프하는 다른 몬스터가 생기는거 방지
            }
        }
        else
        {
            //왼쪽에 몬스터가 없다면 점프타이머 초기화
            if(jumpTimer != 0.0f)
            {
                jumpTimer = 0.0f;
                gameObject.layer = LayerMask.NameToLayer(monsterLayer);
            }
        }

        //위나 왼쪽위에 몬스터가 2초이상 깔고있으면 쓰러지며 미끄러지기
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
        } //몬스터가 왼쪽에 없고 왼쪽에 박스도 없으면 일어나기
        else if (left.monsterNum == 0 && !left.bBox)
        {
            animator.SetTrigger("Up");
            moveVecX = 1.0f;
            downTimer = 0.0f;
        }

        //위 행동들은 자연스럽게 선형보간 시키기
        MoveVec(moveVecX, moveVecY * -1);

        //땅에 아무것도 없으면 그냥 바로 떨어트리기
        if (!bottom.bLand && bottom.monsterNum == 1 && left.monsterNum == 0 && leftBottom.monsterNum == 0)
        {
            moveVecX = 0.0f;
            moveVecY = 2.0f;
            moveVec = new Vector2(moveVecX, moveVecY);
        }

    }

    //보간
    void MoveVec(float x, float y)
    {
        Vector2 target = new Vector2(x, y);
        float smoothSpeed = 5.0f;
        moveVec = Vector2.Lerp(moveVec, target, Time.deltaTime * smoothSpeed);
    }

    //레이쏘기
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
