using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PlayerSaveManager : MonoBehaviour
{
    private string filePath;
    //2022_02_11 map ����� ���ϸ�
    //private string mapFilePath;

    private Player player;
    //2022_02_11 map �����
    ////��¥�� ����ũ���, �̷��� list�� ���°� ���ϴ�.
    public List<park.cell> mapInfo;
    public PlayerSaveBase saving;

    //2022_02_09
    //�� room�� �̹� room�� ������ Ȯ���ϴ� ������ sameCheck.
    //�� room�� �̹� room�� ���ٸ� (�ҷ�����)�� �����ѰŶ�,
    //������ stageVar�� ���߾ ���¸� �ٸ���,
    //���� �� ���������� �̹� room�� �ٸ��� (���� ���������� ���� ����)
    //random���� �������Ѵ�.
    private bool sameCheck;
    public bool getSameCheck()
    {
        return sameCheck;
    }

    //2022_02_09
    //Player�� data�� �� �������� ���Խÿ� load�ȴ�.
    public void Awake()
    {
        //2022_02_11 awake����
        filePath = Application.dataPath + "/PlayerSave.json";
        //mapFilePath = "/MapSave.json";
        //mapInfo = new List<park.cell>(52);
        saving = new PlayerSaveBase();
        loadPlayer();
        if (saving.prevRoomNumber != saving.curRoomNumber) { sameCheck = false; }
        else { sameCheck = true; }
    }

    //2022_02_09
    //�ߵ� Ÿ�̹��� �Ϻη� start�� �Ͽ� ����
    //start�� �������, tile�� spawn���� variation�� ������ awake���� �ʰ� ���� Ȯ���� ������ �����ϴ�.
    //spawn�� tile�� samecheck�� true��� �װ� �о�´�.
    //sameCheck�� false, �� room���� ���������̸� ���� ������ ���ִ� ���̴�.
    //���� sameCheck�� true�� �׳� �ҷ�����ϱ� save�� �� �ʿ䰡 ����.
    public void Start()
    {
        if (sameCheck == false)
        {
            saving.roomType = GameObject.FindWithTag("LevelManager").GetComponent<LevelManager>().currentScene;
            saving.prevRoomNumber = saving.curRoomNumber;
            switch (saving.roomType)
            {
                case "Title":
                case "Stage1_Start":
                case "Stage2_Start":
                case "Stage3_Start":
                    //���⼭ �� �ʱ�ȭ ���ֽø� �˴ϴ�
                    break;
                case "Shop":
                    //����
                    break;
                case "Heal":
                    //ȸ��
                    break;
                case "Stage1_Event":
                case "Stage2_Event":
                case "Stage3_Event":
                    //�̺�Ʈ
                    break;
                default:
                    saving.stageVar1 = GameObject.FindWithTag("TileManager").GetComponent<TileManager>().getTilemapVar();
                    saving.stageVar2 = GameObject.FindWithTag("SpawnManager").GetComponent<SpawnManager>().getEnemyVar();
                    saving.stageVar3 = -1;
                    break;
            }
            savePlayer(false);
        }
    }

    //2022_02_09
    //Player�� data�� �޾ƿͼ� ����.
    //�� ������ �����̳� �̷� �������� player�� �Ⱥ��̵��� �����ϴ� ���� �ʿ�.
    public void savePlayer(bool playerSave)
    {
        if(playerSave)
        {
            player = GameObject.FindWithTag("Player").GetComponent<Player>();
            saving.characterName = player.getName();
            saving.moveSpeed = player.getMoveSpd();
            saving.moveDistance = player.getMoveDist();
            saving.strength = player.getStr();
            saving.attackSpeed = player.getAttackSpd();
            saving.maxHp = player.getMaxHp();
            saving.hp = player.getHp();
            saving.gold = player.getGold();
            saving.artifact1 = player.getArtifact(0);
            saving.artifact2 = player.getArtifact(1);
            saving.artifact3 = player.getArtifact(2);

            //2022_02_11 �������ν�� player info�� �������� �ʴ� ���� title�ۿ� ���� ������
            // = title�� �����ϰ� esc�� �ȸ����� ������ ����ٰ� ����.
            //saveMap();
            //JsonFileHandler.SaveToJson<park.cell>(mapInfo, mapFilePath);
        }

        string content = JsonUtility.ToJson(this.saving, true);
        FileStream fileStream = new FileStream(this.filePath, FileMode.Create);
        using (StreamWriter writer = new StreamWriter(fileStream))
        {
            writer.Write(content);
        }
        
    }

    //2022_02_09 - �̷��� save function���� ��� SwitchScene, �� �������� �Ѿ�� ��ư�鿡�� ������ �ȴ�.
    public void saveStageClear()
    {
        //2022_02_09
        //stage Ŭ�����. Beta��.
        saving.stageNumber++;
        saving.prevRoomNumber = 0;
        //2022_02_11 ���� saveStageClear�� stage Ŭ����ÿ� �Ҹ��� �Ŷ��
        //���� ���������� �Ѿ�� �� �� cur++�� �ȵǴϱ� curRoomNumber�� 1�� ����
        saving.curRoomNumber = 1;
        //2022_02_11 stage�� start��. stageNumber�� 4�� clear�� �ؾ߰���?
        saving.roomType = "Stage" + saving.stageNumber + "_Start";
        savePlayer(true);
    }

    public void saveRoomClear()
    {
        //2022_02_09
        //������ ������� Ŭ���������� Ȯ���� ����� �־����.
        //��, ������ Ŭ�����ϰ� ����â�� �ߴ� ��Ȳ�� �ǹ��Ѵ�.
        //������ ������� tile�� spawn���� �̸� �˷������ <- ���� ���� ����
        //2022_02_11 �ӽ������� curRoomNumber++��.
        saving.curRoomNumber++;
        savePlayer(true);
    }

    public void saveRoomEnd()
    {
        //2022_02_09
        //���������� ���� ������ �Ѿ�� ����ϴ� ��.
        //curRoomNumber�� ++�ؼ� ���� �濡�� random�� ������ �����.
        saving.curRoomNumber++;
        savePlayer(false);
    }

    public void loadPlayer()
    {
        string content;
        if (!(File.Exists(filePath)))
        {
            killPlayer();
        }

        using (StreamReader reader = new StreamReader(filePath))
        {
            content = reader.ReadToEnd();
        }

        this.saving = JsonUtility.FromJson<PlayerSaveBase>(content);
        //this.mapInfo = JsonFileHandler.ReadFromJson<park.cell>(mapFilePath);
    }

    //2022_02_11
    /*
    public void saveMap(List<List<park.cell>> map)
    {
        int p = 0;
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 13; j++)
            {
                park.cell c;
                c = map[i][j];

                this.mapInfo[p] = c;
                p++;
            }
        }
    }*/


    //2022_02_09
    //�׾��� ��, ���� �ʱ�� playerSave�� ����������.
    public void killPlayer()
    {
        saving.characterName = "������ �Ƶ�";
        saving.moveSpeed = 20;
        saving.moveDistance = 1;
        saving.strength = 4;
        saving.attackSpeed = 10;
        saving.maxHp = 20;
        saving.hp = 20;
        saving.gold = 100;
        saving.artifact1 = "�Ǽ�";
        saving.artifact2 = "�Ǽ�";
        saving.artifact3 = "�Ǽ�";
        ///////////////////////////////
        saving.stageNumber = 1;
        saving.prevRoomNumber = 0;
        saving.curRoomNumber = 0;
        saving.roomType = "Stage1_Start";
        saving.stageVar1 = -1;
        saving.stageVar2 = -1;
        saving.stageVar3 = -1;

        /*
        //2022_02_11 �ʱ�ȭ�۾�
        for (int i = 0; i < 52; i++)
        {
            park.cell c;
            c = park.cell.Null;
            mapInfo.Add(c);
        }
        */

        string content = JsonUtility.ToJson(this.saving);
        FileStream fileStream = new FileStream(this.filePath, FileMode.Create);
        using (StreamWriter writer = new StreamWriter(fileStream))
        {
            writer.Write(content);
        }
        //2022_02_11
        //JsonFileHandler.SaveToJson<park.cell>(mapInfo, mapFilePath);
    }
}
