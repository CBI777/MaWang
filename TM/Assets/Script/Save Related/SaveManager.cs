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
    //전 room이 이번 room과 같은지 확인하는 변수가 sameCheck.
    //전 room이 이번 room과 같다면 (불러오기)를 진행한거라,
    //이전의 stageVar에 맞추어서 상태를 꾸리고,
    //만약 전 스테이지와 이번 room이 다르면 (다음 스테이지로 최초 진행)
    //random으로 돌려야한다.
    private bool sameCheck;
    public bool getSameCheck(){return sameCheck;}

    //2022_02_09
    //Player의 data는 매 스테이지 진입시에 load된다.
    public void Awake()
    {
        //2022_02_11 awake수정
        filePath = Application.dataPath + "/PlayerSave.json";
        saving = new SaveBase();
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
                    //2022_02_13 여기 제대로 저장하는 부분이 없어서 start에서는 save가 안되었던 문제 수정
                    
                    //2022_02_18 맵 초기화
                    
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
                default: //여기가 나머지 처리하는 곳인가봄? _Normal & _Elite
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
    //Player의 data를 받아와서 저장.
    //이 때문에 상점이나 이런 곳에서도 player를 안보이도록 생성하는 것이 필요.
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
            //2022_02_13 - 변경된 player의 구조에 대응
            saving.artifact1 = player.getArtifact(0).getRealArtifactName();
            saving.artifact2 = player.getArtifact(1).getRealArtifactName();
            saving.artifact3 = player.getArtifact(2).getRealArtifactName();

            //2022_02_18 맵
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

    public void loadPlayer()//2022_02_09 이 SaveManager객체의 saving의 내용을 JSon에서 불러와 덮어쓴다.
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

    public void killPlayer()//2022_02_09 죽었을 때, 완전 초기로 playerSave를 돌려버린다.
    {
        if (saving.mapData!=null) saving.mapData.Clear();
        saving.characterName = "마왕의 아들";
        saving.moveSpeed = 20;
        saving.moveDistance = 1;
        saving.strength = 4;
        saving.attackSpeed = 10;
        saving.maxHp = 20;
        saving.hp = 20;
        saving.gold = 100;
        //2022_02_13 - Artifact의 변경에 대응하여 변경
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
    /// save~뭐시기 메소드의 처리에 대한 주석
    /// 
    /// saveStageClear는 먼 후일의 얘기인듯하여 일단 주석으로 격리해놓았습니다.
    /// 더불어,
    /// saving.curRoomNumber를 ++하는 단계는 MapUI에서 다음 room을 고를 때 처리하는 걸로 변경했습니다
    /// 변경하고 났더니 saveRoomClear()와 saveRoomEnd는 그저 SavePlayer를 호출하는 한 줄 외에 없는 메소드여서 전부 주석처리했습니다.
    /// 당연히 SceneSwitch의 코드는 수정했습니다.
    /// 후일 다시 쓸 일이 생길 것 같습니다만 일단은 어지러워서 주석으로 접어놨습니다.
    /// </summary>
    /// 
    /*
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

        //saving.curRoomNumber++;
        savePlayer(true);
    }
    public void saveRoomEnd()
    {
        //2022_02_09
        //실질적으로 다음 방으로 넘어갈때 사용하는 것.
        //curRoomNumber를 ++해서 다음 방에서 random이 돌도록 만든다.

        //saving.curRoomNumber++;
        savePlayer(false);
    }

    */
   

    
}
