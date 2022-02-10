using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterController : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(randomMove(2.0f/transform.GetComponent<CharacterBase>().getMoveSpd()));
    }

    IEnumerator randomMove(float coolTime)
    {
        Vector2 temp = new Vector2(0, 0);
        Directions d = Directions.E;

        while(true)
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

            if(transform.GetComponentInParent<SpawnManager>().MoveCharacter(transform.position, temp))
            {
                transform.position = transform.position + (Vector3)temp;
                transform.GetComponent<CharacterBase>().setDirection(d);
            }
            yield return new WaitForSeconds(coolTime);
        }
    }
}
