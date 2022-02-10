using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class InstructionData
{
<<<<<<< Updated upstream
    /*22-1-22 변동사항
    Start내부의 초기화 함수의 구조를 변경
    */


=======
>>>>>>> Stashed changes
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


/*
             * 2022_02_07_18:30 박경수
             * SpawnManager에서 Player는 Instantiate의 예외로 해서 초기 좌표만 갖다주는 것에 대해
[오후 6:21] 박경수: 예 이점이 많아보여서 그냥 그렇게 하기로 하겠습니다
[오후 6:21] 박경수: Player는 이제 Resources에서 빠질 예정입니다
[오후 6:22] 박경수: SpawnManager의 자식으로 Hierarchy에 미리 넣어 놓겠습니다
[오후 6:23] 박경수: : 1 InputManager/LevelManager등의 (인스펙터)접근 용이
[오후 6:25] 박경수: ; 플레이어의 스탯은 접근할 곳이 많음 -> 근데 오타라던가 Awake 순서라던가 에서 NullException자꾸뜨길래
[오후 6:25] 박경수: 심지어 이름도 Player가 아니고 Player(Clone)으로 나와서 한참 분투한결과
[오후 6:27] 박경수: Player는 좀 차별화할 필요가 있다고 강력히 주장하는 바이고 이미 실행해버렸습니다 XDDD
[오후 6:28] 박경수: 그러니 JSON만들때 웬만하면 PLayer는 최대한 맨 위에 놔주시면 감사 : 첫번째 건 for 밖에서 해버렸거든요

요약: 변동사항! Player는 Instantiate안하고 SpawnManager의 자식이니 JSON최상단에 Player의 정보를 기입할것.
    이유는 접근이 자주 되는 Player 전용 필드&메소드 접근의 용이성을 높이기 위해
             */

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
    //현재 적의 상황을 알려주기 위한 변수
    private int enemyVar;

    private PlayerBase player;

    public int getEnemyVar()
    {
        return this.enemyVar;
    }
    private string fileName;

    [SerializeField]
    private Player player;

    private void Awake()
    {
        /*
        //모든 variation들의 이름은 Stage?_(형식)_Enemy?.json  이다.
        fileName = curStage + "_Enemy" + ((Random.Range(0, (stageVar))).ToString() + ".json");
        */
        //모든 variation들은 Assets/Resources/Stage?_(형식)/EnemyVariation 내에 저장되어있으며,
        //Assets/까지는 JsonFileHandler에서 처리해주니까, 그 뒤를 넣으면 된다.
        //이름은 Enemy?.json이다.
        if (GameObject.FindWithTag("LevelManager").GetComponent<PlayerSaveManager>().getSameCheck())
        {
            enemyVar = GameObject.FindWithTag("LevelManager").GetComponent<PlayerSaveManager>().saving.stageVar2;
        }
        else
        {
            enemyVar = Random.Range(0, (stageVar));
        }
        fileName = "Resources/" + curStage + "/EnemyVariation/Enemy" + enemyVar.ToString() + ".json";
    }

    private void Start()
    {
        instructions = JsonFileHandler.ReadFromJson<InstructionData>(fileName);
        Vector2 loc;
        Vector2 loc2;


        loc = new Vector2(instructions[0].x, instructions[0].y);
        if (tileManager.isTileSafe(loc))
        {
            player.transform.position = loc;
            // 어이쿠 player의 좌표를 spawnmanager에서 직접 바꿔버렸네요 ㅎㅎ Start니까 괜찮지 않을까?
            tileManager.placeObject(player.gameObject, loc);
        }
        else
        {
            Debug.Log("Critical Error!! 플레이어의 위치가 좋지 않아요! 바꿔바꿔 당장바꿔");
        }
        instructions.Remove(instructions[0]);

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
            loc2 = new Vector2(InstructionData.x + 0.5f, InstructionData.y + 0.5f);

            if(!(tileManager.isTileSafe(loc)))
            {
                print("something wrong with the location" + loc);
            }
            else
            {
                tileManager.placeObject(GameObject.Instantiate(
                Resources.Load(curStage + "/Characters/" + InstructionData.wantedObject, typeof(GameObject)) as GameObject,
                loc2, Quaternion.identity, transform), loc);
            }
        }
        player = GameObject.FindWithTag("Player").GetComponent<PlayerBase>();
        player.updatePlayer(GameObject.FindWithTag("LevelManager").GetComponent<PlayerSaveManager>().saving);
    }


    public bool MoveCharacter(Vector3 originalGridPosition, Vector2 amount)
    {
        if(tileManager.moveObject(originalGridPosition, (originalGridPosition + (Vector3)amount)))
        {
            return true;
        }

        //print("Unable to move to " + (originalGridPosition + (Vector3)amount));
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