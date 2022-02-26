using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAI__SmallGolem : MonsterController
{
    [SerializeField]
    private int readyRange = 1;

    [SerializeField]
    List<Vector3Int> attackRange = new List<Vector3Int> { new Vector3Int(1, 0, 0), new Vector3Int(0, 1, 0), new Vector3Int(-1, 0, 0), new Vector3Int(0, -1, 0) };

    private void Start()
    {
        StartCoroutine(GolemAI());
    }

    IEnumerator GolemAttack()
    {
        if(AttackFlag)
        {
            this.ActFlag = false;
            float sec = 1.0f * this.beforeCast;

            while (sec >= 0f)
            {
                sec -= Time.deltaTime;

                yield return new WaitForFixedUpdate();
            }
            gameObject.GetComponentInParent<SpawnManager>().AttackPlayer(transform.position, attackRange, gameObject.GetComponent<CharacterBase>().getStr(), "FistSlash");
            yield return new WaitForFixedUpdate();
            this.ActFlag = true;

            StartCoroutine(attackCoolTime());
        }
    }

    IEnumerator GolemAI()
    {
        yield return new AsyncOperation();
        while (true)
        {
            yield return new WaitForFixedUpdate();
            if (ActFlag)
            {
                if(calcDist() > alertRange)
                {
                    moveRandom();
                }
                else if(calcDist() <= readyRange)
                {
                    StartCoroutine(GolemAttack());
                }
                else
                {
                    moveToPlayer();
                }
            }
        }
    }
}
