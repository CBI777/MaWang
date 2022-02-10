using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterController : MonoBehaviour
{
    /*22-1-19 변동사항
    Start함수 내의 startCoroutine내의 characterbase에서 movespeed 가져오는 부분을
    get함수에 맞추어 수정
     */
    void Start()
    {
        StartCoroutine(randomMove(2.0f/transform.GetComponent<CharacterBase>().getMoveSpd()));
    }

    IEnumerator randomMove(float coolTime)
    {
        Vector2 temp = new Vector2(0, 0);

        while(true)
        {
            switch (Random.Range(0, 3))
            {
                case 0:
                    temp = new Vector2(1, 0);
                    break;
                case 1:
                    temp = new Vector2(0, 1);
                    break;
                case 2:
                    temp = new Vector2(-1, 0);
                    break;
                case 3:
                    temp = new Vector2(0, -1);
                    break;
            }

            if(transform.GetComponentInParent<SpawnManager>().MoveCharacter(transform.position, temp))
            {
                transform.position = transform.position + (Vector3)temp;
            }
            yield return new WaitForSeconds(coolTime);
        }
    }
}
