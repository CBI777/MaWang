using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class InstructionData
{
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

public class InstructionDataArray
{
    public InstructionData[] Items;
}

public class SpawnManager : MonoBehaviour
{
    InstructionDataArray instData;

    [SerializeField]
    private TileManager tileManager;
    //2022_02_09 - inspector가 좋다고 하셔서 levelManager도 inspector로 넣도록 했습니다.
    [SerializeField]
    private LevelManager levelManager;

    //이 SpawnManager는 어느 scene에 있는 아이인지를 inspector로 넣어줌
    [SerializeField]
    private string curStage;

    //2022_02_09 이름 때문에 혼동을 주지 않기 위해서 StageVar -> RoomVar로 이름을 변경
    //참고로, 이건 enemyVaration의 갯수가 얼마나되는지를 알려주는 변수다.
    [SerializeField]
    private int roomVar;

    //2022_02_09 - getEnemyVar 추가
    private int enemyVar;
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
        if (levelManager.GetComponent<SaveManager>().getSameCheck())
        {
            enemyVar = levelManager.GetComponent<SaveManager>().saving.stageVar2;
        }
        else
        {
            enemyVar = Random.Range(0, (roomVar));
        }
        fileName = "" + curStage + "/EnemyVariation/Enemy" + enemyVar.ToString();

    }

    private void Start()
    {
        if (curStage.Equals("Shop") || curStage.Equals("Stage1_Event"))
        {

        }
        else
        {
            /*
            instructions.Add(new InstructionData(1.5f, 1.5f, "Enemy_Slime"));
            instructions.Add(new InstructionData(3f, 2f, "Obstacle1"));
            instructions.Add(new InstructionData(4f, 5f, "Obstacle1"));
            instructions.Add(new InstructionData(-6f, -8f, "Enemy_Slime"));

            JsonFileHandler.SaveToJson<InstructionData>(instructions, fileName);
            */
            TextAsset textData = Resources.Load(fileName, typeof(TextAsset)) as TextAsset;
            Debug.Log(textData);
            instData = JsonUtility.FromJson<InstructionDataArray>(textData.ToString());
            Debug.Log(instData.Items[0].x);
            //2022_02_09 - 실제로 보여지는 것과 좌표계는 다르기 때문에...
            //loc은 실제 좌표계를 담당, loc2는 우리가 보기에 좋게 하기 위한 좌표임.
            //즉, tileManager에게 들어가는건 loc, instantiate나 position은 loc2
            //이게 들어가야했는데 실수입니다. 죄송합니다
            Vector2 loc;
            Vector2 loc2;

            loc = new Vector2(instData.Items[0].x, instData.Items[0].y);
            loc2 = new Vector2(instData.Items[0].x + 0.5f, instData.Items[0].y + 0.5f);
            if (tileManager.isTileSafe(loc))
            {
                tileManager.placeObject(player.gameObject, loc);
                player.transform.position = loc2;
                // 2022_02_09 player좌표 여기서 바꿔도 상관 없습니다
            }
            else
            {
                Debug.Log("Critical Error!! 플레이어의 위치가 좋지 않아요! 바꿔바꿔 당장바꿔");
                Debug.Log(loc + ", " + loc2);
            }

            for(int i = 1; i<instData.Items.Length; i++)
            {
                loc = new Vector2(instData.Items[i].x, instData.Items[i].y);
                loc2 = new Vector2(instData.Items[i].x + 0.5f, instData.Items[i].y + 0.5f);

                if (!(tileManager.isTileSafe(loc)))
                {
                    print("something wrong with the location" + loc);
                }
                else
                {
                    tileManager.placeObject(GameObject.Instantiate(
                    Resources.Load(curStage + "/Characters/" + instData.Items[i].wantedObject, typeof(GameObject)) as GameObject,
                    loc2, Quaternion.identity, transform), loc);
                }
            }
            
        }
        player.updatePlayer(levelManager.GetComponent<SaveManager>().saving);
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

    //2022_02_13 effect를 character 내부에서 적용하기 위해서 bool로 변경
    //한 번이라도 공격이 성공했다면 tileManager에서 true가 반환될거고, 그러면 true를 반환
    public bool AttackCharacter(Vector3 originalGridPosition, List<Vector3Int> attackRange, int damage)
    {
        bool temp = false;
        foreach (Vector3Int range in attackRange)
        {
            temp = temp | (tileManager.damageObject(damage, (originalGridPosition + range)));
        }

        return temp;
    }

    public void AttackPlayer(Vector3 originalGridPosition, List<Vector3Int> attackRange, int damage, string effectName, string soundName)
    {
        foreach (Vector3Int range in attackRange)
        {
            if(tileManager.damagePlayer(damage, (originalGridPosition + range)))
            {
                EffectHelper.printEffect(effectName, (originalGridPosition + range), DirectionChange.dirToRotation(range));
                SoundEffecter.playSFX(soundName);
            }
        }
    }

    
}