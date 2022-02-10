using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterController : MonoBehaviour
{
    /*22-1-19 ��������
    Start�Լ� ���� startCoroutine���� characterbase���� movespeed �������� �κ���
    get�Լ��� ���߾� ����
     */
    void Start()
    {
        StartCoroutine(randomMove(2.0f/transform.GetComponent<CharacterBase>().getMoveSpd()));
    }

    IEnumerator randomMove(float coolTime)
    {
        Vector2 temp = new Vector2(0, 0);
        //2022_02_09 - direction���� �߰�. �Ŀ� UI���� ����� ��. Switch���ο����� ������.
        Directions d = Directions.E;

        while (true)
        {
            switch (Random.Range(0, 3))
            {
                case 0:
                    temp = new Vector2(1, 0);
                    d = Directions.E;
                    break;
                case 1:
                    temp = new Vector2(0, 1);
                    d = Directions.N;
                    break;
                case 2:
                    temp = new Vector2(-1, 0);
                    d = Directions.W;
                    break;
                case 3:
                    temp = new Vector2(0, -1);
                    d = Directions.S;
                    break;
            }

            if (transform.GetComponentInParent<SpawnManager>().MoveCharacter(transform.position, temp))
            {
                transform.position = transform.position + (Vector3)temp;
                //2022_02_09 - �Ŀ� UI ��� �� ����� ��.
                transform.GetComponent<CharacterBase>().setDirection(d, transform.position);
            }
            yield return new WaitForSeconds(coolTime);
        }
    }
}
