using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PlayerSaveManager : MonoBehaviour
{
    private string filePath;
    //2022_02_11 map 저장용 파일명
    //private string mapFilePath;

    private Player player;
    //2022_02_11 map 저장용
    ////어짜피 고정크기고, 이러면 list를 쓰는게 편하다.
    public List<park.cell> mapInfo;
    public PlayerSaveBase saving;

    //2022_02_09
    //전 room이 이번 room과 같은지 확인하는 변수가 sameCheck.
    //전 room이 이번 room과 같다면 (불러오기)를 진행한거라,
    //이전의 stageVar에 맞추어서 상태를 꾸리고,
    //만약 전 스테이지와 이번 room이 다르면 (다음 스테이지로 최초 진행)
    //random으로 돌려야한다.
    private bool sameCheck;
    public bool getSameCheck()
    {
        return sameCheck;
    }

    //2022_02_09
    //Player의 data는 매 스테이지 진입시에 load된다.
    public void Awake()
    {
        //2022_02_11 awake수정
        filePath = Application.dataPath + "/PlayerSave.json";
        //mapFilePath = "/MapSave.json";
        //mapInfo = new List<park.cell>(52);
        saving = new PlayerSaveBase();
        loadPlayer();
        if (saving.prevRoomNumber != saving.curRoomNumber) { sameCheck = false; }
        else { sameCheck = true; }
    }

    //2022_02_09
    //발동 타이밍을 일부러 start로 하여 늦춤
    //start로 늦춰놔서, tile과 spawn에서 variation을 생성한 awake보다 늦게 들어와 확실히 저장이 가능하다.
    //spawn과 tile은 samecheck가 true라면 그걸 읽어온다.
    //sameCheck가 false, 즉 room으로 최초진행이면 이제 저장을 해주는 것이다.
    //만약 sameCheck가 true면 그냥 불러오기니까 save도 할 필요가 없음.
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
                    //여기서 맵 초기화 해주시면 됩니다
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

    //2022_02_09
    //Player의 data를 받아와서 저장.
    //이 때문에 상점이나 이런 곳에서도 player를 안보이도록 생성하는 것이 필요.
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

            //2022_02_11 지금으로써는 player info를 저장하지 않는 곳이 title밖에 없기 때문에
            // = title은 유일하게 esc가 안먹히기 때문에 여기다가 넣음.
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

    //2022_02_09 - 이러한 save function들은 모두 SwitchScene, 즉 다음으로 넘어가는 버튼들에서 적용이 된다.
    public void saveStageClear()
    {
        //2022_02_09
        //stage 클리어시. Beta임.
        saving.stageNumber++;
        saving.prevRoomNumber = 0;
        //2022_02_11 만약 saveStageClear가 stage 클리어시에 불리는 거라면
        //다음 스테이지로 넘어가는 갈 때 cur++가 안되니까 curRoomNumber를 1로 맞춤
        saving.curRoomNumber = 1;
        //2022_02_11 stage를 start로. stageNumber가 4면 clear를 해야겠지?
        saving.roomType = "Stage" + saving.stageNumber + "_Start";
        savePlayer(true);
    }

    public void saveRoomClear()
    {
        //2022_02_09
        //모종의 방법으로 클리어했음을 확인할 방법이 있어야함.
        //즉, 전투를 클리어하고 보상창이 뜨는 상황을 의미한다.
        //모종의 방법으로 tile과 spawn에게 이를 알려줘야함 <- 아직 구현 안함
        //2022_02_11 임시적으로 curRoomNumber++임.
        saving.curRoomNumber++;
        savePlayer(true);
    }

    public void saveRoomEnd()
    {
        //2022_02_09
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
    //죽었을 때, 완전 초기로 playerSave를 돌려버린다.
    public void killPlayer()
    {
        saving.characterName = "마왕의 아들";
        saving.moveSpeed = 20;
        saving.moveDistance = 1;
        saving.strength = 4;
        saving.attackSpeed = 10;
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
        saving.roomType = "Stage1_Start";
        saving.stageVar1 = -1;
        saving.stageVar2 = -1;
        saving.stageVar3 = -1;

        /*
        //2022_02_11 초기화작업
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
