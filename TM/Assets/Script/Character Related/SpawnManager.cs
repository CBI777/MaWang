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
    //2022_02_09 - inspector�� ���ٰ� �ϼż� levelManager�� inspector�� �ֵ��� �߽��ϴ�.
    [SerializeField]
    private LevelManager levelManager;

    //�� SpawnManager�� ��� scene�� �ִ� ���������� inspector�� �־���
    [SerializeField]
    private string curStage;

    //2022_02_09 �̸� ������ ȥ���� ���� �ʱ� ���ؼ� StageVar -> RoomVar�� �̸��� ����
    //�����, �̰� enemyVaration�� ������ �󸶳��Ǵ����� �˷��ִ� ������.
    [SerializeField]
    private int roomVar;

    //2022_02_09 - getEnemyVar �߰�
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
        //��� variation���� �̸��� Stage?_(����)_Enemy?.json  �̴�.
        fileName = curStage + "_Enemy" + ((Random.Range(0, (stageVar))).ToString() + ".json");
        */
        //��� variation���� Assets/Resources/Stage?_(����)/EnemyVariation ���� ����Ǿ�������,
        //Assets/������ JsonFileHandler���� ó�����ִϱ�, �� �ڸ� ������ �ȴ�.
        //�̸��� Enemy?.json�̴�.
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
            //2022_02_09 - ������ �������� �Ͱ� ��ǥ��� �ٸ��� ������...
            //loc�� ���� ��ǥ�踦 ���, loc2�� �츮�� ���⿡ ���� �ϱ� ���� ��ǥ��.
            //��, tileManager���� ���°� loc, instantiate�� position�� loc2
            //�̰� �����ߴµ� �Ǽ��Դϴ�. �˼��մϴ�
            Vector2 loc;
            Vector2 loc2;

            loc = new Vector2(instData.Items[0].x, instData.Items[0].y);
            loc2 = new Vector2(instData.Items[0].x + 0.5f, instData.Items[0].y + 0.5f);
            if (tileManager.isTileSafe(loc))
            {
                tileManager.placeObject(player.gameObject, loc);
                player.transform.position = loc2;
                // 2022_02_09 player��ǥ ���⼭ �ٲ㵵 ��� �����ϴ�
            }
            else
            {
                Debug.Log("Critical Error!! �÷��̾��� ��ġ�� ���� �ʾƿ�! �ٲ�ٲ� ����ٲ�");
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

    //2022_02_13 effect�� character ���ο��� �����ϱ� ���ؼ� bool�� ����
    //�� ���̶� ������ �����ߴٸ� tileManager���� true�� ��ȯ�ɰŰ�, �׷��� true�� ��ȯ
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