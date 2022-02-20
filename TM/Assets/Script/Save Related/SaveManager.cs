using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    private string filePath;

    private Player player;
    public SaveBase saving;

    //2022_02_09
    //�� room�� �̹� room�� ������ Ȯ���ϴ� ������ sameCheck.
    //�� room�� �̹� room�� ���ٸ� (�ҷ�����)�� �����ѰŶ�,
    //������ stageVar�� ���߾ ���¸� �ٸ���,
    //���� �� ���������� �̹� room�� �ٸ��� (���� ���������� ���� ����)
    //random���� �������Ѵ�.
    private bool sameCheck;
    public bool getSameCheck(){return sameCheck;}

    //2022_02_09
    //Player�� data�� �� �������� ���Խÿ� load�ȴ�.
    public void Awake()
    {
        //2022_02_11 awake����
        filePath = Application.dataPath + "/PlayerSave.json";
        saving = new SaveBase();
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
        Debug.Log(saving.prevRoomNumber +" " +saving.curRoomNumber + " " + saving.curRoomRow);
        if (sameCheck == false)
        {
            saving.roomType = GameObject.FindWithTag("LevelManager").GetComponent<LevelManager>().currentScene;
            park.MapUI mapui = GameObject.FindWithTag("UIManager").GetComponent<park.MapUI>();
            saving.prevRoomNumber = saving.curRoomNumber;
            switch (saving.roomType)
            {
                case "Title":
                    break;
                case "Stage1_Start":
                case "Stage2_Start":
                case "Stage3_Start":
                    //2022_02_13 ���� ����� �����ϴ� �κ��� ��� start������ save�� �ȵǾ��� ���� ����
                    
                    //2022_02_18 �� �ʱ�ȭ
                    
                    if (mapui.mapInfos!=null)
                    {
                        mapui.mapInfos.Clear();
                        Debug.Log("mapinfoscleared");
                    }
                    mapui.Initializing();
                    mapui.MapGeneration();
                    saving.mapData = mapui.GetMapData();

                    saving.stageVar1 = GameObject.FindWithTag("TileManager").GetComponent<TileManager>().getTilemapVar();
                    saving.stageVar2 = GameObject.FindWithTag("SpawnManager").GetComponent<SpawnManager>().getEnemyVar();
                    saving.stageVar3 = -1;
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
                default: //���Ⱑ ������ ó���ϴ� ���ΰ���? _Normal & _Elite
                    if (mapui != null)
                    {
                        Debug.Log(saving.mapData);
                        mapui.SetMapData(saving.mapData);
                    }
                    Debug.Log(saving.mapData);

                    saving.stageVar1 = GameObject.FindWithTag("TileManager").GetComponent<TileManager>().getTilemapVar();
                    saving.stageVar2 = GameObject.FindWithTag("SpawnManager").GetComponent<SpawnManager>().getEnemyVar();
                    saving.stageVar3 = -1;
                    break;
            }
            savePlayer(false,false);
            Debug.Log(saving.prevRoomNumber + " " + saving.curRoomNumber + " " + saving.curRoomRow);
        }
    }

    //2022_02_09
    //Player�� data�� �޾ƿͼ� ����.
    //�� ������ �����̳� �̷� �������� player�� �Ⱥ��̵��� �����ϴ� ���� �ʿ�.
    public void savePlayer(bool playerSave, bool mapSave)
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
            //2022_02_13 - ����� player�� ������ ����
            saving.artifact1 = player.getArtifact(0).getRealArtifactName();
            saving.artifact2 = player.getArtifact(1).getRealArtifactName();
            saving.artifact3 = player.getArtifact(2).getRealArtifactName();

            //2022_02_18 ��
            saving.mapData = GameObject.FindWithTag("UIManager").GetComponent<park.MapUI>().GetMapData();
        }
        if (mapSave)
        {
            park.MapUI mapui = GameObject.FindWithTag("UIManager").GetComponent<park.MapUI>();
            if (mapui.mapInfos != null)
            {
                saving.mapData = mapui.GetMapData();
            }
            else
            {
                Debug.Log("mapsavefailed");
            }
        }

        string content = JsonUtility.ToJson(this.saving, true);
        FileStream fileStream = new FileStream(this.filePath, FileMode.Create);
        using (StreamWriter writer = new StreamWriter(fileStream))
        {
            writer.Write(content);
        }
    }

    public void loadPlayer()//2022_02_09 �� SaveManager��ü�� saving�� ������ JSon���� �ҷ��� �����.
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

        this.saving = JsonUtility.FromJson<SaveBase>(content);
        //this.mapInfo = JsonFileHandler.ReadFromJson<park.cell>(mapFilePath);
    }

    public void killPlayer()//2022_02_09 �׾��� ��, ���� �ʱ�� playerSave�� ����������.
    {
        if (saving.mapData!=null) saving.mapData.Clear();
        saving.characterName = "������ �Ƶ�";
        saving.moveSpeed = 20;
        saving.moveDistance = 1;
        saving.strength = 4;
        saving.attackSpeed = 10;
        saving.maxHp = 20;
        saving.hp = 20;
        saving.gold = 100;
        //2022_02_13 - Artifact�� ���濡 �����Ͽ� ����
        saving.artifact1 = "Artifact__Hand";
        saving.artifact2 = "Artifact__Hand";
        saving.artifact3 = "Artifact__Hand";
        ///////////////////////////////
        saving.stageNumber = 1;
        saving.prevRoomNumber = -2;
        saving.curRoomNumber = -1;
        saving.curRoomRow = 0;
        saving.roomType = "Title";
        saving.stageVar1 = -1;
        saving.stageVar2 = -1;
        saving.stageVar3 = -1;


        string content = JsonUtility.ToJson(this.saving);
        FileStream fileStream = new FileStream(this.filePath, FileMode.Create);
        using (StreamWriter writer = new StreamWriter(fileStream))
        {
            writer.Write(content);
        }
    }



    /// <summary>
    /// 2022_02_19
    /// save~���ñ� �޼ҵ��� ó���� ���� �ּ�
    /// 
    /// saveStageClear�� �� ������ ����ε��Ͽ� �ϴ� �ּ����� �ݸ��س��ҽ��ϴ�.
    /// ���Ҿ�,
    /// saving.curRoomNumber�� ++�ϴ� �ܰ�� MapUI���� ���� room�� �� �� ó���ϴ� �ɷ� �����߽��ϴ�
    /// �����ϰ� ������ saveRoomClear()�� saveRoomEnd�� ���� SavePlayer�� ȣ���ϴ� �� �� �ܿ� ���� �޼ҵ忩�� ���� �ּ�ó���߽��ϴ�.
    /// �翬�� SceneSwitch�� �ڵ�� �����߽��ϴ�.
    /// ���� �ٽ� �� ���� ���� �� �����ϴٸ� �ϴ��� ���������� �ּ����� ��������ϴ�.
    /// </summary>
    /// 
    /*
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

        //saving.curRoomNumber++;
        savePlayer(true);
    }
    public void saveRoomEnd()
    {
        //2022_02_09
        //���������� ���� ������ �Ѿ�� ����ϴ� ��.
        //curRoomNumber�� ++�ؼ� ���� �濡�� random�� ������ �����.

        //saving.curRoomNumber++;
        savePlayer(false);
    }

    */
   

    
}
