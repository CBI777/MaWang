using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAI__EliteGolem : MonsterController
{
    [SerializeField]
    private int readyRange1 = 1;
    [SerializeField]
    private int readyRange2 = 3;

    List<Vector3Int> attackRange1 = new List<Vector3Int> { new Vector3Int(1, 0, 0), new Vector3Int(0, 1, 0), new Vector3Int(-1, 0, 0), new Vector3Int(0, -1, 0) };
    List<Vector3Int> attackRange2 = new List<Vector3Int>
    {
      new Vector3Int(1, 0, 0), new Vector3Int(0, 1, 0), new Vector3Int(-1, 0, 0), new Vector3Int(0, -1, 0),
      new Vector3Int(2, 0, 0), new Vector3Int(2,1,0), new Vector3Int(2,-1,0), new Vector3Int(3,0,0), new Vector3Int(3,1,0), new Vector3Int(3, -1,0),
      new Vector3Int(0, 2, 0), new Vector3Int(1,2,0), new Vector3Int(-1,2,0), new Vector3Int(0,3,0), new Vector3Int(1,3,0), new Vector3Int(-1,3,0),
      new Vector3Int(-2, 0, 0), new Vector3Int(-2,1,0), new Vector3Int(-2,-1,0), new Vector3Int(-3,0,0), new Vector3Int(-3,1,0), new Vector3Int(-3, -1, 0),
      new Vector3Int(0, -2, 0), new Vector3Int(1, -2, 0), new Vector3Int(-1,-2,0), new Vector3Int(0,-3,0), new Vector3Int(1, -3,0), new Vector3Int(-1,-3,0),
      new Vector3Int(1, 1, 0), new Vector3Int(1, -1, 0), new Vector3Int(-1, 1, 0), new Vector3Int(-1, -1, 0)
    };

    private void Start()
    {
        StartCoroutine(GolemAI());
    }

    IEnumerator GolemFirstAttack()
    {
        if (AttackFlag)
        {
            this.ActFlag = false;
            float sec = 1.0f * this.beforeCast;
            StartCoroutine(warnArea(transform.position, attackRange1, sec));

            while (sec >= 0f)
            {
                sec -= Time.deltaTime;

                yield return new WaitForFixedUpdate();
            }
            gameObject.GetComponentInParent<SpawnManager>().AttackPlayer(transform.position, attackRange1, gameObject.GetComponent<CharacterBase>().getStr()*2, "FistSlash");
            yield return new WaitForFixedUpdate();
            this.ActFlag = true;

            StartCoroutine(attackCoolTime());
        }
    }
    IEnumerator GolemSecondAttack()
    {
        if (AttackFlag)
        {
            this.ActFlag = false;
            float sec = 1.5f * this.beforeCast;
            StartCoroutine(warnArea(transform.position, attackRange2, sec));

            while (sec >= 0f)
            {
                sec -= Time.deltaTime;

                yield return new WaitForFixedUpdate();
            }
            //22_03_02 넓은 범위 데미지가 제대로 들어가지 않던 버그를 수정
            gameObject.GetComponentInParent<SpawnManager>().AttackPlayer(transform.position, attackRange2, (gameObject.GetComponent<CharacterBase>().getStr() * 2), "Explosion_Orange");
            yield return new WaitForFixedUpdate();
            this.ActFlag = true;

            StartCoroutine(secondAttackCoolTime());
        }
    }

    protected IEnumerator secondAttackCoolTime()
    {
        this.AttackFlag = false;
        float sec = 6.0f / attspd;

        while (sec >= 0)
        {
            sec -= Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }
        this.AttackFlag = true;
    }

    IEnumerator GolemAI()
    {
        yield return new AsyncOperation();
        while (true)
        {
            yield return new WaitForFixedUpdate();
            if (ActFlag)
            {
                if (calcDist() > alertRange)
                {
                    moveRandom();
                }
                else if(calcDist() <= readyRange1)
                {
                    if (Random.Range(0, 10) < 7)
                    {
                        StartCoroutine(GolemFirstAttack());
                    }
                    else
                    {
                        StartCoroutine(GolemSecondAttack());
                    }
                }
                else if(calcDist() <= readyRange2)
                {
                    StartCoroutine(GolemSecondAttack());
                }
                else
                {
                    moveToPlayer();
                }
            }
        }
    }
}
