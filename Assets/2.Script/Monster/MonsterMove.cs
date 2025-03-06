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

    //풀링땜에 스타트에 있던거 여기로 빼두기
    public void ResetMonster()
    {
        rb = this.GetComponent<Rigidbody2D>();
        animator = this.GetComponent<Animator>();

        //웨이브 메니저에서 랜덤으로 부여받은 레이어로 몬스터가 충돌처리할 레이어들 세팅
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

    //최적화) 레이탐색은 0.3초에 한번씩으로 변경! 한 프레임에 계산해야 하는 양 줄이기 보간은 어색해지므로 레이탐색 함수에서 독립시키기
    IEnumerator CheakObj()
    {
        while (true)
        {
            deltaTime = Time.time - preTime;

            //레이 각 방향으로 쏘기
            RayImfo left = Ray((Vector2)transform.position + new Vector2(-0.5f, 0.2f), new Vector2(-1.0f, 0.0f), 0.2f);
            RayImfo top = Ray((Vector2)transform.position + new Vector2(-0.15f, 0.7f), new Vector2(0.0f, 1.0f), 0.5f);
            RayImfo bottom = Ray((Vector2)transform.position + new Vector2(-0.25f, 0.0f), new Vector2(0.0f, -0.3f), 0.3f);
            RayImfo leftTop = Ray((Vector2)transform.position + new Vector2(-0.5f, 0.5f), new Vector2(-0.7f, 1.0f), 0.5f);
            RayImfo leftBottom = Ray((Vector2)transform.position + new Vector2(-0.5f, 0.0f), new Vector2(-1.0f, -1.0f), 0.5f);

            //왼쪽에 박스가 있다면 움직임 멈추고 공격
            if (left.bBox)
            {
                deltaMoveVec.x = 0.0f;
                deltaMoveVec.y = 0.0f;
                animator.SetTrigger("Attack");
            }

            //왼쪽에 몬스터가 0.4초 이상 있다면 점프
            if (left.monsterNum > 0)
            {
                jumpTimer += deltaTime;
                if (jumpTimer >= 0.4f)
                {
                    deltaMoveVec.x = 1.0f;
                    deltaMoveVec.y = 1.5f;
                    animator.SetTrigger("Jump");
                    gameObject.layer = LayerMask.NameToLayer(jumpLayer); //점프하는 몬스터를 잡으며 점프하는 다른 몬스터가 생기는거 방지
                }
            }
            else
            {
                //왼쪽에 몬스터가 없다면 점프타이머 초기화
                if (jumpTimer != 0.0f)
                {
                    jumpTimer = 0.0f;
                    gameObject.layer = LayerMask.NameToLayer(monsterLayer);
                }

                deltaMoveVec.x = 1.0f;
                deltaMoveVec.y = 0.0f;
            }

            //위나 왼쪽위에 몬스터가 2초이상 깔고있으면 쓰러지며 미끄러지기
            if (top.monsterNum > 0 || leftTop.monsterNum > 0)
            {
                downTimer += deltaTime;
                if (downTimer >= 2.0f)
                {
                    animator.SetTrigger("Down");
                    deltaMoveVec.x = -1.0f;
                }
            } //몬스터가 왼쪽에 없고 왼쪽에 박스도 없으면 일어나기
            else if (left.monsterNum == 0 && !left.bBox)
            {
                animator.SetTrigger("Up");
                deltaMoveVec.x = 1.0f;
                downTimer = 0.0f;
            }

            //위 행동들은 자연스럽게 선형보간 시키기

            //땅에 아무것도 없으면 그냥 바로 떨어트리기
            if (!bottom.bLand && bottom.monsterNum == 0 && left.monsterNum == 0 && leftBottom.monsterNum == 0)
            {
                moveVec = deltaMoveVec = new Vector2(0, -2);
            }

            preTime = Time.time;

            yield return new WaitForSeconds(0.3f);
        }
    }

    //보간
    void MoveVec(float x, float y, float deltaTime)
    {
        Vector2 target = new Vector2(x, y);
        float smoothSpeed = 5.0f;
        moveVec = Vector2.Lerp(moveVec, target, deltaTime * smoothSpeed);
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

    public void DieMonster()
    {
        StopCoroutine(CheakObj());
    }
}
