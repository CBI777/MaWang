using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAI_Phantom : MonsterController
{
    [SerializeField]
    private int readyRange = 8;

    [SerializeField]
    private int escapeRange = 3;

    [SerializeField]
    List<Vector3Int> attackRange = new List<Vector3Int>
    { new Vector3Int(0, 0, 0), new Vector3Int(1, 0, 0), new Vector3Int(0, 1, 0), new Vector3Int(-1, 0, 0), new Vector3Int(0, -1, 0),
      new Vector3Int(2, 0, 0), new Vector3Int(0, 2, 0), new Vector3Int(-2, 0, 0), new Vector3Int(0, -2, 0),
      new Vector3Int(1, 1, 0), new Vector3Int(1, -1, 0), new Vector3Int(-1, 1, 0), new Vector3Int(-1, -1, 0)
    };

    private void Start()
    {
        StartCoroutine(PhantomAI());
    }

    IEnumerator PhantomEscape()
    {
        if(MoveFlag)
        {
            awayFromPlayer();
        }
        else
        {
            StartCoroutine(PhantomAttack());
        }
        yield return new WaitForFixedUpdate();
    }


    IEnumerator PhantomAttack()
    {
        if (AttackFlag)
        {
            this.ActFlag = false;
            float sec = 1.5f * this.beforeCast;
            Vector3 playerTarget = new Vector3(playerLoc.getX()+0.5f, playerLoc.getY() + 0.5f);
            StartCoroutine(warnArea(playerTarget, attackRange, sec));
            
            while (sec >= 0f)
            {
                sec -= Time.deltaTime;

                yield return new WaitForFixedUpdate();
            }
            gameObject.GetComponentInParent<SpawnManager>().AttackPlayer(playerTarget, 
                attackRange, gameObject.GetComponent<CharacterBase>().getStr(), "Explosion_Orange");
            yield return new WaitForFixedUpdate();
            this.ActFlag = true;

            StartCoroutine(attackCoolTime());
        }
    }

    IEnumerator PhantomAI()
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
                else if (calcDist() <= readyRange)
                {
                    StartCoroutine(PhantomEscape());
                }
                else
                {
                    moveToPlayer();
                }
            }
        }
    }
}
