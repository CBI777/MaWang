using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PlayerSaveManager : MonoBehaviour
{
    private string filePath;

    private PlayerBase player;
    public PlayerSaveBase saving;

    //�� room�� �̹� room�� ������ Ȯ��.
    //�� room�� �̹� room�� ���ٸ� (�ҷ�����)�� �����ѰŶ�,
    //������ stageVar�� ���߾ ���¸� �ٸ���,
    //���� �� ���������� �̹� room�� �ٸ��� (���� ���������� ���� ����)
    //random���� �������Ѵ�.
    private bool sameCheck;
    public bool getSameCheck()
    {
        return sameCheck;
    }

    //Player�� data�� �� �������� ���Խÿ� load�ȴ�.
    public void Awake()
    {
        filePath = Application.dataPath + "/PlayerSave.json";
        saving = new PlayerSaveBase();
        loadPlayer();
        if (saving.prevRoomNumber != saving.curRoomNumber) { sameCheck = false; }
        else { sameCheck = true; }
    }
    //�ߵ� Ÿ�̹��� �Ϻη� start�� �Ͽ� ����
    //sameCheck�� false, �� room���� ���������̸� ���� ������ ���ִ� ���̴�.
    //start�� �������, tile�� spawn���� variation�� ������ awake���� �ʰ� ���� Ȯ���� ������ �����ϴ�.
    //���� sameCheck�� true�� �׳� �ҷ�����ϱ� save�� �� �ʿ䰡 ����.
    public void Start()
    {
        if (sameCheck == false)
        {
            saving.roomType = GameObject.FindWithTag("LevelManager").GetComponent<LevelManager>().currentScene;
            saving.prevRoomNumber = saving.curRoomNumber;
            switch (saving.roomType)
            {
                case "Start":
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


    //Player�� data�� �޾ƿͼ� ����.
    //�� ������ �����̳� �̷� �������� player�� �Ⱥ��̵��� �����ϴ� ���� �ʿ�.
    public void savePlayer(bool playerSave)
    {
        if(playerSave)
        {
            player = GameObject.FindWithTag("Player").GetComponent<PlayerBase>();
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
        }

        string content = JsonUtility.ToJson(this.saving, true);
        FileStream fileStream = new FileStream(this.filePath, FileMode.Create);
        using (StreamWriter writer = new StreamWriter(fileStream))
        {
            writer.Write(content);
        }
    }

    public void saveStageClear()
    {
        //stage Ŭ�����. Beta��.
        saving.stageNumber++;
        saving.prevRoomNumber = 0;
        saving.curRoomNumber = 0;
        savePlayer(true);
    }

    public void saveRoomClear()
    {
        //������ ������� Ŭ���������� Ȯ���� ����� �־����.
        //��, ������ Ŭ�����ϰ� ����â�� �ߴ� ��Ȳ�� �ǹ��Ѵ�.
        //������ ������� tile�� spawn���� �̸� �˷���߰���.
        savePlayer(true);
    }

    public void saveRoomEnd()
    {
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
    }
    //�׾��� ��, ���� �ʱ�� playerSave�� ����������.
    public void killPlayer()
    {
        saving.characterName = "������ �Ƶ�";
        saving.moveSpeed = 1;
        saving.moveDistance = 1;
        saving.strength = 4;
        saving.attackSpeed = 1;
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
        saving.roomType = "Start";
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
}
