using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PlayerSaveManager : MonoBehaviour
{
    private string filePath;

    private PlayerBase player;
    public PlayerSaveBase saving;

    //전 room이 이번 room과 같은지 확인.
    //전 room이 이번 room과 같다면 (불러오기)를 진행한거라,
    //이전의 stageVar에 맞추어서 상태를 꾸리고,
    //만약 전 스테이지와 이번 room이 다르면 (다음 스테이지로 최초 진행)
    //random으로 돌려야한다.
    private bool sameCheck;
    public bool getSameCheck()
    {
        return sameCheck;
    }

    //Player의 data는 매 스테이지 진입시에 load된다.
    public void Awake()
    {
        filePath = Application.dataPath + "/PlayerSave.json";
        saving = new PlayerSaveBase();
        loadPlayer();
        if (saving.prevRoomNumber != saving.curRoomNumber) { sameCheck = false; }
        else { sameCheck = true; }
    }
    //발동 타이밍을 일부러 start로 하여 늦춤
    //sameCheck가 false, 즉 room으로 최초진행이면 이제 저장을 해주는 것이다.
    //start로 늦춰놔서, tile과 spawn에서 variation을 생성한 awake보다 늦게 들어와 확실히 저장이 가능하다.
    //만약 sameCheck가 true면 그냥 불러오기니까 save도 할 필요가 없음.
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
                    //상점
                    break;
                case "Heal":
                    //회복
                    break;
                case "Stage1_Event":
                case "Stage2_Event":
                case "Stage3_Event":
                    //이벤트
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


    //Player의 data를 받아와서 저장.
    //이 때문에 상점이나 이런 곳에서도 player를 안보이도록 생성하는 것이 필요.
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
        //stage 클리어시. Beta임.
        saving.stageNumber++;
        saving.prevRoomNumber = 0;
        saving.curRoomNumber = 0;
        savePlayer(true);
    }

    public void saveRoomClear()
    {
        //모종의 방법으로 클리어했음을 확인할 방법이 있어야함.
        //즉, 전투를 클리어하고 보상창이 뜨는 상황을 의미한다.
        //모종의 방법으로 tile과 spawn에게 이를 알려줘야겠지.
        savePlayer(true);
    }

    public void saveRoomEnd()
    {
        //실질적으로 다음 방으로 넘어갈때 사용하는 것.
        //curRoomNumber를 ++해서 다음 방에서 random이 돌도록 만든다.
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
    //죽었을 때, 완전 초기로 playerSave를 돌려버린다.
    public void killPlayer()
    {
        saving.characterName = "마왕의 아들";
        saving.moveSpeed = 1;
        saving.moveDistance = 1;
        saving.strength = 4;
        saving.attackSpeed = 1;
        saving.maxHp = 20;
        saving.hp = 20;
        saving.gold = 100;
        saving.artifact1 = "맨손";
        saving.artifact2 = "맨손";
        saving.artifact3 = "맨손";
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
