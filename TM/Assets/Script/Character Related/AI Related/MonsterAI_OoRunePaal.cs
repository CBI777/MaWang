using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAI_OoRunePaal : MonsterController
{
    [SerializeField]
    private int readyRange1 = 2;
    [SerializeField]
    private int readyRange2 = 9;

    [SerializeField]
    private bool areaFlag = true;

    //주변 2칸 공격
    List<Vector3Int> attackRange1 = new List<Vector3Int>
    { new Vector3Int(1, 0, 0), new Vector3Int(0, 1, 0), new Vector3Int(-1, 0, 0), new Vector3Int(0, -1, 0),
      new Vector3Int(2, 0, 0), new Vector3Int(0, 2, 0), new Vector3Int(-2, 0, 0), new Vector3Int(0, -2, 0),
      new Vector3Int(1, 1, 0), new Vector3Int(1, -1, 0), new Vector3Int(-1, 1, 0), new Vector3Int(-1, -1, 0)
    };

    //십자범위 폭발
    List<Vector3Int> attackRange2 = new List<Vector3Int>
    {
      new Vector3Int(0, 0, 0),
      new Vector3Int(1, 0, 0), new Vector3Int(0, 1, 0), new Vector3Int(-1, 0, 0), new Vector3Int(0, -1, 0),
      new Vector3Int(2, 0, 0), new Vector3Int(2,1,0), new Vector3Int(2,-1,0), new Vector3Int(3,0,0), new Vector3Int(3,1,0), new Vector3Int(3, -1,0),
      new Vector3Int(0, 2, 0), new Vector3Int(1,2,0), new Vector3Int(-1,2,0), new Vector3Int(0,3,0), new Vector3Int(1,3,0), new Vector3Int(-1,3,0),
      new Vector3Int(-2, 0, 0), new Vector3Int(-2,1,0), new Vector3Int(-2,-1,0), new Vector3Int(-3,0,0), new Vector3Int(-3,1,0), new Vector3Int(-3, -1, 0),
      new Vector3Int(0, -2, 0), new Vector3Int(1, -2, 0), new Vector3Int(-1,-2,0), new Vector3Int(0,-3,0), new Vector3Int(1, -3,0), new Vector3Int(-1,-3,0),
      new Vector3Int(1, 1, 0), new Vector3Int(1, -1, 0), new Vector3Int(-1, 1, 0), new Vector3Int(-1, -1, 0)
    };

    /*
    List<Vector3Int> attackRange3 = new List<Vector3Int>
    { new Vector3Int(0, 0, 0),
      new Vector3Int(2, 0, 0), new Vector3Int(0, 2, 0), new Vector3Int(-2, 0, 0), new Vector3Int(0, -2, 0),
      new Vector3Int(1, 1, 0), new Vector3Int(1, -1, 0), new Vector3Int(-1, 1, 0), new Vector3Int(-1, -1, 0)
    };*/
    List<Vector3Int> attackRange3 = new List<Vector3Int>
    { new Vector3Int(0, 0, 0),
      new Vector3Int(3, 0, 0), new Vector3Int(3, 1, 0), new Vector3Int(3, 2, 0), new Vector3Int(3, 3, 0),
      new Vector3Int(2, 3, 0), new Vector3Int(1, 3, 0), new Vector3Int(0, 3, 0),
      new Vector3Int(-1, 3, 0), new Vector3Int(-2, 3, 0), new Vector3Int(-3, 3, 0),
      new Vector3Int(-3, 2, 0), new Vector3Int(-3, 1, 0), new Vector3Int(-3, 0, 0),
      new Vector3Int(-3, -1, 0), new Vector3Int(-3, -2, 0), new Vector3Int(-3, -3, 0),
      new Vector3Int(-2, -3, 0), new Vector3Int(-1, -3, 0), new Vector3Int(0, -3, 0),
      new Vector3Int(1, -3, 0), new Vector3Int(2, -3, 0), new Vector3Int(3, -3, 0),
      new Vector3Int(3, -3, 0), new Vector3Int(3, -2, 0), new Vector3Int(3, -1, 0),
    };


    private void Start()
    {
        StartCoroutine(GolemAI());
    }

    IEnumerator PaalFirstAttack()
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
            gameObject.GetComponentInParent<SpawnManager>().AttackPlayer(transform.position, attackRange1, gameObject.GetComponent<CharacterBase>().getStr() * 2, "Explosion_Blue", "Magic1");
            yield return new WaitForFixedUpdate();
            this.ActFlag = true;

            StartCoroutine(attackCoolTime());
        }
    }

    IEnumerator PaalSecondAttack()
    {
        if (AttackFlag)
        {
            this.ActFlag = false;
            float sec = 1.5f * this.beforeCast;
            Vector3 playerTarget = new Vector3(playerLoc.getX() + 0.5f, playerLoc.getY() + 0.5f);
            StartCoroutine(warnArea(playerTarget, attackRange2, sec));

            while (sec >= 0f)
            {
                sec -= Time.deltaTime;

                yield return new WaitForFixedUpdate();
            }
            gameObject.GetComponentInParent<SpawnManager>().AttackPlayer(playerTarget,
                attackRange2, gameObject.GetComponent<CharacterBase>().getStr(), "Explosion_Orange", "Magic3");
            yield return new WaitForFixedUpdate();
            this.ActFlag = true;

            StartCoroutine(attackCoolTime());
        }
    }

    IEnumerator PaalThirdAttack()
    {
        if (areaFlag)
        {
            this.areaFlag = false;
            float sec = 1.5f * this.beforeCast;
            Vector3 playerTarget = new Vector3(playerLoc.getX() + 0.5f, playerLoc.getY() + 0.5f);
            StartCoroutine(warnArea2(playerTarget, attackRange3, sec));

            while (sec >= 0f)
            {
                sec -= Time.deltaTime;

                yield return new WaitForFixedUpdate();
            }
            gameObject.GetComponentInParent<SpawnManager>().AttackPlayer(playerTarget,
                attackRange3, gameObject.GetComponent<CharacterBase>().getStr(), "Explosion_Darkness", "Magic2");
            yield return new WaitForFixedUpdate();

            StartCoroutine(areaAttackCoolTime());
        }
    }

    protected IEnumerator warnArea2(Vector3 loc, List<Vector3Int> range, float beforeCast)
    {
        foreach (Vector3Int p in range)
        {
            Destroy(GameObject.Instantiate(
                Resources.Load("Effect/Warning2", typeof(GameObject)) as GameObject,
                (loc + new Vector3(0, 0, -0.1f) + p), Quaternion.Euler(new Vector3(0, 0, 0))), beforeCast);
        }
        yield return null;
    }

    protected IEnumerator areaAttackCoolTime()
    {
        
        float sec = 6.0f / attspd;

        while (sec >= 0)
        {
            sec -= Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }
        this.areaFlag = true;
    }

    protected IEnumerator secondAttackCoolTime()
    {
        this.AttackFlag = false;
        float sec = 4.0f / attspd;

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
                if(areaFlag)
                {
                    StartCoroutine(PaalThirdAttack());
                }
                if (calcDist() <= readyRange1)
                {
                    if (Random.Range(0, 10) < 7)
                    {
                        StartCoroutine(PaalFirstAttack());
                    }
                    else
                    {
                        StartCoroutine(PaalSecondAttack());
                    }
                }
                else if (calcDist() <= readyRange2)
                {
                    StartCoroutine(PaalSecondAttack());
                }
                else
                {
                    moveToPlayer();
                }
            }
        }
    }
}
