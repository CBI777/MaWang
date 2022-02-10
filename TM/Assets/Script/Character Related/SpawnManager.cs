using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class InstructionData
{
    /*22-1-22 변동사항
    Start내부의 초기화 함수의 구조를 변경
    */
    public InstructionData(float x, float y, string wanted)
    {
        this.x = x;
        this.y = y;
        this.wantedObject = wanted;
    }
    [SerializeField] public float x;
    [SerializeField] public float y;
    [SerializeField] public string wantedObject;
}

public class SpawnManager : MonoBehaviour
{
    List<InstructionData> instructions = new List<InstructionData>();

    [SerializeField]
    private TileManager tileManager;

    //이 SpawnManager는 어디에 있는 아이인지를 inspector로 넣어줌
    [SerializeField]
    private string curStage;

    //이 SpawnManager의 구역에는 얼마나 많은 variation이 존재하는지를 알려줌
    //inspector로 넣어준다.
    //0, 1, 2, 3이 있다면 4개.
    [SerializeField]
    private int stageVar;
    
    private string fileName;

    private void Awake()
    {
        /*
        //모든 variation들의 이름은 Stage?_(형식)_Enemy?.json  이다.
        fileName = curStage + "_Enemy" + ((Random.Range(0, (stageVar))).ToString() + ".json");
        */
        //모든 variation들은 Assets/Resources/Stage?_(형식)/EnemyVariation 내에 저장되어있으며,
        //Assets/까지는 JsonFileHandler에서 처리해주니까, 그 뒤를 넣으면 된다.
        //이름은 Enemy?.json이다.
        fileName = "Resources/" + curStage + "/EnemyVariation/Enemy" + ((Random.Range(0, (stageVar))).ToString() + ".json");

    }

    private void Start()
    {
        /*
        instructions.Add(new InstructionData(1.5f, 1.5f, "Enemy_Slime"));
        instructions.Add(new InstructionData(3f, 2f, "Obstacle1"));
        instructions.Add(new InstructionData(4f, 5f, "Obstacle1"));
        instructions.Add(new InstructionData(-6f, -8f, "Enemy_Slime"));

        JsonFileHandler.SaveToJson<InstructionData>(instructions, fileName);
        */
        instructions = JsonFileHandler.ReadFromJson<InstructionData>(fileName);
        Vector2 loc;

        foreach (var InstructionData in instructions)
        {
            //모든 character prefab들은 Resources 폴더 내의 Stage?/Characters/에 존재한다.

            /*기존 방식의 가장 큰 문제점은 Instantiate한 값을 pass해주기 때문에 해당 위치가 올바른 위치가 아니라도
             * 적절한 조치를 취할 수 없다는 것이었다.
             * 즉, valid한 좌표가 아니면 애초에 생성도 하지 못하게 하기 위해서는 SpawnManager가 애초부터 확인을 하고,
             * 그 다음에 Instantiate한 값을 pass해주는 식으로 바꾸었다.
             * 이러면 한 번 체크를 하는 step을 밟아야하긴 하지만, 생성하고 없애는 것보다는 덜 복잡하다.
             */
            loc = new Vector2(InstructionData.x, InstructionData.y);

            if(!(tileManager.isTileSafe(loc)))
            {
                print("something wrong with the location" + loc);
            }
            else
            {
                tileManager.placeObject(GameObject.Instantiate(
                Resources.Load(curStage + "/Characters/" + InstructionData.wantedObject, typeof(GameObject)) as GameObject,
                loc, Quaternion.identity, transform), loc);
            }
        }
    }

    public bool MoveCharacter(Vector3 originalGridPosition, Vector2 amount)
    {
        if(tileManager.moveObject(originalGridPosition, (originalGridPosition + (Vector3)amount)))
        {
            return true;
        }

        print("Unable to move to " + (originalGridPosition + (Vector3)amount));
        return false;
    }

    public void AttackCharacter(Vector3 originalGridPosition, List<Vector3Int> attackRange, int damage)
    {
        foreach (Vector3Int range in attackRange)
        {
            tileManager.damageObject(damage, (originalGridPosition + range));
        }
    }
}