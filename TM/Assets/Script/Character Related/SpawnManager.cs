using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class InstructionData
{
<<<<<<< Updated upstream
    /*22-1-22 ��������
    Start������ �ʱ�ȭ �Լ��� ������ ����
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
             * 2022_02_07_18:30 �ڰ��
             * SpawnManager���� Player�� Instantiate�� ���ܷ� �ؼ� �ʱ� ��ǥ�� �����ִ� �Ϳ� ����
[���� 6:21] �ڰ��: �� ������ ���ƺ����� �׳� �׷��� �ϱ�� �ϰڽ��ϴ�
[���� 6:21] �ڰ��: Player�� ���� Resources���� ���� �����Դϴ�
[���� 6:22] �ڰ��: SpawnManager�� �ڽ����� Hierarchy�� �̸� �־� ���ڽ��ϴ�
[���� 6:23] �ڰ��: : 1 InputManager/LevelManager���� (�ν�����)���� ����
[���� 6:25] �ڰ��: ; �÷��̾��� ������ ������ ���� ���� -> �ٵ� ��Ÿ����� Awake ��������� ���� NullException�ڲٶ߱淡
[���� 6:25] �ڰ��: ������ �̸��� Player�� �ƴϰ� Player(Clone)���� ���ͼ� ���� �����Ѱ��
[���� 6:27] �ڰ��: Player�� �� ����ȭ�� �ʿ䰡 �ִٰ� ������ �����ϴ� ���̰� �̹� �����ع��Ƚ��ϴ� XDDD
[���� 6:28] �ڰ��: �׷��� JSON���鶧 �����ϸ� PLayer�� �ִ��� �� ���� ���ֽø� ���� : ù��° �� for �ۿ��� �ع��Ȱŵ��

���: ��������! Player�� Instantiate���ϰ� SpawnManager�� �ڽ��̴� JSON�ֻ�ܿ� Player�� ������ �����Ұ�.
    ������ ������ ���� �Ǵ� Player ���� �ʵ�&�޼ҵ� ������ ���̼��� ���̱� ����
             */

public class SpawnManager : MonoBehaviour
{
    List<InstructionData> instructions = new List<InstructionData>();

    [SerializeField]
    private TileManager tileManager;

    //�� SpawnManager�� ��� �ִ� ���������� inspector�� �־���
    [SerializeField]
    private string curStage;

    //�� SpawnManager�� �������� �󸶳� ���� variation�� �����ϴ����� �˷���
    //inspector�� �־��ش�.
    //0, 1, 2, 3�� �ִٸ� 4��.
    [SerializeField]
    private int stageVar;
    //���� ���� ��Ȳ�� �˷��ֱ� ���� ����
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
        //��� variation���� �̸��� Stage?_(����)_Enemy?.json  �̴�.
        fileName = curStage + "_Enemy" + ((Random.Range(0, (stageVar))).ToString() + ".json");
        */
        //��� variation���� Assets/Resources/Stage?_(����)/EnemyVariation ���� ����Ǿ�������,
        //Assets/������ JsonFileHandler���� ó�����ִϱ�, �� �ڸ� ������ �ȴ�.
        //�̸��� Enemy?.json�̴�.
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
            // ������ player�� ��ǥ�� spawnmanager���� ���� �ٲ���ȳ׿� ���� Start�ϱ� ������ ������?
            tileManager.placeObject(player.gameObject, loc);
        }
        else
        {
            Debug.Log("Critical Error!! �÷��̾��� ��ġ�� ���� �ʾƿ�! �ٲ�ٲ� ����ٲ�");
        }
        instructions.Remove(instructions[0]);

        foreach (var InstructionData in instructions)
        {
            //��� character prefab���� Resources ���� ���� Stage?/Characters/�� �����Ѵ�.

            /*���� ����� ���� ū �������� Instantiate�� ���� pass���ֱ� ������ �ش� ��ġ�� �ùٸ� ��ġ�� �ƴ϶�
             * ������ ��ġ�� ���� �� ���ٴ� ���̾���.
             * ��, valid�� ��ǥ�� �ƴϸ� ���ʿ� ������ ���� ���ϰ� �ϱ� ���ؼ��� SpawnManager�� ���ʺ��� Ȯ���� �ϰ�,
             * �� ������ Instantiate�� ���� pass���ִ� ������ �ٲپ���.
             * �̷��� �� �� üũ�� �ϴ� step�� ��ƾ��ϱ� ������, �����ϰ� ���ִ� �ͺ��ٴ� �� �����ϴ�.
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