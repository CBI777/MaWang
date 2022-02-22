using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace park
{

    /*
     * 이 코드는 후에 LevelManager, UIManager 코드 등에 병합할 예정입니다.
     * 처음 작성 할 때의 프로토타입은 Canvas > Scroll View에 직접 넣었었음
     * 
     * 2022-02-07
     * cell이라는 enum을 추가하여 구분되지 않았던 Normal,Elite,Shop,Event 의 구분을 가능하게 함
     * 주석 많이 추가
     */

    // 왜 mapInformation(구조체) 이랑 cell(enum)을 굳이 따로 놓는 거야?
    // 합쳐도 되지 않나? _02_07_03:26 -> 합쳐버렸습니다 ^ㅁ^ _02_07_03:35

    // 2022_02_19
    // 뭔가 추후의 오류가 생긴다면 그것은 높은 확률로 MapDraw와 관련한 문제일 것. 참고

    public enum cell
    {
        /*
         * 기본적인 정보 저장 방법은 비트플래그
         * 세 구간으로 나눔. (가독성을 위한 언더스코어! : 0b00000000 와 같은 표현이 기본형인데 그 사이에 언더스코어는 자유롭게 배치 가능)
         * 1. 첫 두 비트 : 보스 or 보스직전 or 지나왔는가? (대충 아래 속성과 독립적인 잡다한 친구들)
         * 2. 가운데 세 비트 : 비트마다 각각 바로 옆으로 가는 길, 아래로 가는 길, 위로 가는 길에 대한 플래그
         * 3. 마지막 세 비트 : 셀의 타입을 저장(노말/엘리트/상점/이벤트...)
         * 정보 확인 방법 : 목표cell과 cell.뭐시기를 비트AND(&) 연산 후 == cell.뭐시기
         * 두 독립적인 속성 섞어서 저장할 땐 비트OR(|) 연산
         */

        Null    = 0b_00_000_000, //초기값. Cell 없고(화면 표시 X) 길도 없음

        //타입(노말/엘리트/상점/이벤트...)
        ClrType = 0b_11_111_000, //타입을 초기화할 때 &연산자와 같이 쓰이는 Flag. 
        Normal  = 0b_00_000_001, 
        Elite   = 0b_00_000_010, 
        Shop    = 0b_00_000_011,
        Event   = 0b_00_000_100,

        //    (  ↗ㅁ)
        //길:{ ㅁ→ㅁ } 에서 화살표
        //    (  ↘ㅁ)
        ClrWay = 0b_11_000_111,
        Upper   = 0b_00_001_000,
        Lower   = 0b_00_010_000,
        Straight= 0b_00_100_000,

        // 기타 잡다한 독립적인 속성 (보스방/보스직전방/지나왔는지체크)
        ClrLoc  = 0b_00_111_111,
        //Initial = 0b_01_000_000, _02_07_05:30 뭔가 데이터 낭비 같아서 뺐음 + checked 를 추가하고 싶었음
        PreBoss = 0b_01_000_000,
        Boss    = 0b_10_000_000,
        Checked = 0b_11_000_000 // 지나온 곳인가?
    }


    public class MapUI : MonoBehaviour
    {
        private SaveBase saving; //2022_02_16 추가

        [SerializeField] private ScrollRect scrollRect; // 맵UI 틀

        [SerializeField] private float margin=20;  //각 셀/라인들의 여백
        [SerializeField] private int row=4, col=13; //맵의 행, 열
        [SerializeField] private GameObject uiPrefab_box, uiPrefab_cell, uiPrefab_line; // UI에 직접 그려질 원본 프리팹.
        [SerializeField] private float wayChangeProbability, wayAddProbability, EliteProbability, ShopProbability, EventProbability; //여러 속성들의 부여될 확률
        [SerializeField] private List<RectTransform> uiObjects = new List<RectTransform>(); //프리팹들의 생성, 삭제를 보다 편리하게 해줄 게임오브젝트 리스트 : 테스트용으로 자주 썼고 실제 게임에선 별 필요 없을 수 있음

        private List<RectTransform> selectables = new List<RectTransform>(3); //2022_02_16 추가 : 라디오버튼의 효과를 원한다!

        public List<List<cell>> mapInfos; // ** 맵 정보 저장.
        /// <summary>
        /// mapinfos는 대체로 UIManager/ MapUIBtn/ SaveManager가 접근&참조합니다.
        /// 이들은 모두 mapInfos를 직접 수정하지 않습니다.
        /// 값을 수정할 때에는 아래의 SetMapData/UpdateMapData 메소드를 호출합니다.
        /// get/set은 saveManager가 호출하며, 이는 2차원 리스트를 1차원 리스트로 변환해야하기 때문
        /// 그 외의 읽기 용도의 접근은 대체로 Public이기에 직접 접근합니다. Get은 거의 쓰지 않습니다.
        /// 
        /// </summary>

        public void Start() //2022_02_18 saving의 인스펙터 참조를 지향했지만 굴복하고 Start에서 참조
        // 죄송합니다 이거 어떻게 할 수 있지 않을까 하고 scriptableobject니 뭐니 찾아보다가 늦었습니다
        {
            saving = GameObject.FindWithTag("LevelManager").GetComponent<SaveManager>().saving;
        }

        
        public void Initializing() //mapInfos 초기화
        {
            mapInfos = new List<List<cell>>(row);
            for (int i = 0; i < row; i++)
            {
                mapInfos.Add(new List<cell>(col));
                for (int j =0; j < col; j++)
                {
                    cell c;
                    c = cell.Null;
                    
                    mapInfos[i].Add(c);
                }
            }
        }
        public void Clearing() //mapInfos & uiObjects & selectables 내용 지우기
        {
            if (mapInfos.Count > 0)
            {
                mapInfos.Clear();
            }
            if (selectables.Count > 0)
            {
                selectables.Clear();
            }
            foreach(var obj in uiObjects)
            {
                Destroy(obj.gameObject);
            }
            uiObjects.Clear();
        }

        public void SelectDraw(Transform button, bool flag)//이제 어딜 갈지 선택하는 걸 표시
        {
            foreach (RectTransform rect in selectables)
            {
                rect.Find("Highlight_Selected").gameObject.SetActive(false);
            }
            button.Find("Highlight_Selected").gameObject.SetActive(flag);
        } 
        public void MapGeneration() //mapInfos의 데이터 생성 
        {
            /*
             * MapInfos의 정보만 수정하는 곳
             * 
             * 기본 알고리즘
             * 당연히 이중 for문인데 열 단위로 먼저 접근해서
             * n열 전부 처리 후 (n+1)열 처리함
             * n열의 길이 이어지는 곳에 (n+1)열의 셀을 만듬
             * (n+1)열의 셀은 알아서 타입 지정 후 다음 길 만들기
             * 셀마다 다음 열로 이어지는 길은 1~3개임
             * 
             * 맵이 중간에 끊길 걱정이요? 안하셔도 됩니다 ^^
             * 셀 하나당 다음 열로 가는 길이 무조건 하나 이상이라서 보스까지 끊길일은 없답니다!
             * 
             * 길이 X자로 크로스 되는건 방지하기 조금 귀찮아보임 ㅎㅎㅎㅎㅎㅎ 그런거 바라지 마세요 XD
             */

            for (int i= 0; i < col; i++)
            {
                for (int j= 0; j < row; j++)
                {
                    cell c;
                    c = cell.Null;

                    if (i == 0) // 첫번째 열이면 그냥 Normal & Straight : 뭐 얘도 무리없이 길이나 타입 바꿀 수 있는데 단지 귀찮아서 대충 해놨음
                    {
                        c = cell.Normal | cell.Straight;
                    }
                    else if (i == col - 1)  // 마지막 열이면(==보스) Boss 설정
                    {
                        if (j == 0)
                            c = cell.Boss;
                        else
                            c = cell.Null;
                    }
                    else
                    {
                        if ((mapInfos[j][i - 1]&cell.Straight)==cell.Straight) // 왼칸의 앞길 있으면 : 셀 생성
                        {
                            c = cell.Normal | cell.Straight;
                        }
                        else if (j == 0) // 맨 윗줄이면 => 왼아래칸의 위쪽길 있음 ? 셀 생성 : NULL
                        {
                            if ((mapInfos[j + 1][i - 1] & cell.Upper) == cell.Upper)
                            {
                                c = cell.Normal | cell.Straight;
                            }
                            else
                            {
                                c = cell.Null;
                                mapInfos[j][i] = c;
                                continue;
                            }
                        }
                        else if (j == row - 1) // 맨 아랫줄이면 => 왼윗칸의 아래쪽길 있음 ? 셀 생성 : NULL
                        {
                            if ((mapInfos[j - 1][i - 1] & cell.Lower) == cell.Lower)
                            {
                                c = cell.Normal | cell.Straight;
                            }
                            else
                            {
                                c = cell.Null;
                                mapInfos[j][i] = c;
                                continue;
                            }
                        }
                        else // 맨 윗줄도 아니고 맨 아랫줄도 아니면 => 왼윗칸의 아래쪽길 || 왼아래칸의 위쪽길 ? 셀 생성 : NULL
                        {
                            if ((mapInfos[j + 1][i - 1] & cell.Upper) == cell.Upper || (mapInfos[j - 1][i - 1] & cell.Lower) == cell.Lower)
                            {
                                c = cell.Normal | cell.Straight;
                            }
                            else
                            {
                                c = cell.Null;
                                mapInfos[j][i] = c;
                                continue;
                            }
                        }
                        // ▲ 이전 열의 정보를 통해 Null or 셀 생성을 결정.

                        if (i == col - 1)  // 보스는 이미 설정함
                        {
                            continue;
                        }
                        else if (i == col - 2) // 마지막 전 열이면(==보스 직전 단계면) PreBoss 설정
                        {
                            c = cell.PreBoss;
                        }
                        else
                        {
                            // ▼ 길의 방향에 무작위성 부여
                            if (Random.Range(0f, 1f) < wayChangeProbability) // 길의 기본값 (직진) 을 다른 방향으로 바꾸는 경우
                            {

                                if (j != 0 && Random.Range(0f, 2f) > 1) // 맨 윗줄 아니고 50% => 위쪽 길로 바꾸기
                                {
                                    c &= cell.ClrWay;
                                    c |= cell.Upper;
                                }
                                else if (j != row - 1) // 맨 아랫줄 아니고 50% => 아래쪽 길로 바꾸기
                                {
                                    c &= cell.ClrWay;
                                    c |= cell.Lower;
                                }
                            }
                            else if (Random.Range(0f, 1f) < wayAddProbability) // 그 외 다른 방향의 길을 추가하는 경우 1
                            {
                                if (j != 0 && Random.Range(0f, 2f) > 1) //맨 윗줄 아니고 50% => 위쪽 길 추가
                                {
                                    c |= cell.Upper;
                                }
                                else if (j != row - 1) // 맨 아랫줄 아니고 50% => 아래쪽 길 추가
                                {
                                    c |= cell.Lower;
                                }

                                if (Random.Range(0f, 1f) < wayAddProbability) // 그 외 다른 방향의 길을 추가하는 경우 2 
                                {
                                    // 전방향 길 추가
                                    if (j != 0) c |= cell.Upper;
                                    if (j != row - 1) c |= cell.Lower;
                                    c |= cell.Straight;
                                }
                            }
                        }

                        // ▼ 셀의 타입(노말/엘리트/이벤트..) 지정
                        if ((c & cell.Boss) != cell.Boss)
                        {
                            if (Random.Range(0f,1f) < EliteProbability)
                            {
                                c &= cell.ClrType;
                                c |= cell.Elite;
                            } else if (Random.Range(0f, 1f - EliteProbability) < EventProbability)
                            {
                                c &= cell.ClrType;
                                c |= cell.Event;
                            } else if (Random.Range(0f, 1f - EliteProbability - EventProbability) < ShopProbability)
                            {
                                c &= cell.ClrType;
                                c |= cell.Shop;
                            }
                            else
                            {
                                c &= cell.ClrType;
                                c |= cell.Normal;
                            }
                        }
                    }

                    mapInfos[j][i] = c;
                }
            }
        }
        public void MapDraw() //** UI에 직접 그리기 (Instantiate) 및 속성 변경
        {
            selectables.Clear();
            float grid; // 행, 열의 개수에 따른 한 칸의 크기를 유동적으로 사용. 

            Debug.Log(scrollRect);
            Debug.Log(scrollRect.content);
            scrollRect.content.sizeDelta = new Vector2(scrollRect.content.rect.height / row * col, scrollRect.GetComponent<RectTransform>().sizeDelta.y-margin);
            grid = scrollRect.content.rect.height / row;
            scrollRect.content.anchoredPosition = new Vector2(0, 0);
            //맵UI의 틀의 크기를 설정! (행, 열이 자유롭게 바뀔 때 필요했음)
            //가끔 위치가 지멋대로 엇나가서 위치 고정하는 코드도 넣었음
            
            for (int i=0;i<col;i++)
            {
                for (int j = 0; j < row; j++)
                {
                    if ((mapInfos[j][i] & cell.Straight) == cell.Straight)
                    {
                        var newLineUI = Instantiate(uiPrefab_line, scrollRect.content).GetComponent<RectTransform>();
                        newLineUI.sizeDelta = new Vector2(grid, margin*0.5f);
                        newLineUI.rotation = Quaternion.Euler(0f, 0f, 0f);
                        newLineUI.anchoredPosition = new Vector2((i + 1) * grid, -(j + 0.5f) * grid);
                        if ((mapInfos[j][i] & cell.Checked) == cell.Checked && ((mapInfos[j][i+1] & cell.Checked)==cell.Checked || (saving.curRoomNumber == i+1 && saving.curRoomRow == j)))
                        {
                            newLineUI.Find("Orange").gameObject.SetActive(true);
                        }
                        else
                        {
                            newLineUI.Find("Black").gameObject.SetActive(true);
                        }
                        uiObjects.Add(newLineUI);
                    }
                    if ((mapInfos[j][i] & cell.Upper) == cell.Upper)
                    {
                        var newLineUI = Instantiate(uiPrefab_line, scrollRect.content).GetComponent<RectTransform>();
                        newLineUI.sizeDelta = new Vector2(grid*1.4f, margin*0.5f );
                        newLineUI.rotation = Quaternion.Euler(0f, 0f, 45f);
                        newLineUI.anchoredPosition = new Vector2((i + 1) * grid, -j * grid);
                        if ((mapInfos[j][i] & cell.Checked) == cell.Checked && ((j>0 && (mapInfos[j - 1][i + 1] & cell.Checked) == cell.Checked) || (saving.curRoomNumber == i + 1 && saving.curRoomRow == j-1)))
                        {
                            newLineUI.Find("Orange").gameObject.SetActive(true);
                        }
                        else
                        {
                            newLineUI.Find("Black").gameObject.SetActive(true);
                        }
                        uiObjects.Add(newLineUI);
                    }
                    if ((mapInfos[j][i] & cell.Lower) == cell.Lower)
                    {
                        var newLineUI = Instantiate(uiPrefab_line, scrollRect.content).GetComponent<RectTransform>();
                        newLineUI.sizeDelta = new Vector2(grid*1.4f, margin*0.5f);
                        newLineUI.rotation = Quaternion.Euler(0f, 0f, -45f);
                        newLineUI.anchoredPosition = new Vector2((i + 1) * grid, -(j+1) * grid);
                        if ((mapInfos[j][i] & cell.Checked) == cell.Checked && ((j<row-1 &&(mapInfos[j + 1][i + 1] & cell.Checked) == cell.Checked )|| (saving.curRoomNumber == i + 1 && saving.curRoomRow == j+1)))
                        {
                            newLineUI.Find("Orange").gameObject.SetActive(true);
                        }
                        else
                        {
                            newLineUI.Find("Black").gameObject.SetActive(true);
                        }
                        uiObjects.Add(newLineUI);
                    }
                    // ▲ 길 그리기
                    // ▼ 셀 그리기
                    if (mapInfos[j][i] != cell.Null)
                    {
                        var newBoxUI = Instantiate(uiPrefab_box, scrollRect.content).GetComponent<RectTransform>();
                        newBoxUI.sizeDelta = new Vector2(grid, grid);
                        newBoxUI.anchoredPosition = new Vector2(i * grid, -j * grid);
                        var newCellUI = Instantiate(uiPrefab_cell, newBoxUI).GetComponent<RectTransform>();
                        newCellUI.sizeDelta = new Vector2(grid - margin, grid - margin);
                        newCellUI.gameObject.GetComponent<MapUIBtn>().xIndex = i;
                        newCellUI.gameObject.GetComponent<MapUIBtn>().yIndex = j;

                        //▼ 보스방 그리기 : 특별취급(규격이 다름..?)
                        if (mapInfos[j][i]== cell.Boss)
                        {
                            newBoxUI.anchoredPosition = new Vector2(i * grid, (grid - scrollRect.content.rect.height) / 2);
                            newCellUI.Find("Boss").gameObject.SetActive(true);
                            if ((mapInfos[saving.curRoomRow][saving.curRoomNumber] & ~cell.ClrLoc) == cell.PreBoss)
                            {
                                selectables.Add(newCellUI);
                            }
                            uiObjects.Add(newBoxUI);
                            uiObjects.Add(newCellUI);
                            continue;
                        }

                        //▼ 현재 방과 연결된 다음 방들의 구별. -> 현재 방이 끝난 후 버튼이 활성화될 방들.
                        if (saving.curRoomNumber == -1 && i==0)
                        {
                            selectables.Add(newCellUI);
                        }
                        else if (j > 0 && i > 0 && saving.curRoomNumber + 1 == i && saving.curRoomRow + 1 == j && (mapInfos[j - 1][i - 1] & cell.Lower) == cell.Lower)
                        {
                            selectables.Add(newCellUI);
                        }
                        else if (j < row - 1 && i > 0 && saving.curRoomNumber + 1 == i && saving.curRoomRow - 1 == j && (mapInfos[j + 1][i - 1] & cell.Upper) == cell.Upper )
                        {
                            selectables.Add(newCellUI);
                        }
                        else if (i > 0 && saving.curRoomNumber + 1 == i && saving.curRoomRow == j && (mapInfos[j][i - 1] & cell.Straight) == cell.Straight)
                        {
                            selectables.Add(newCellUI);
                        }

                        //▼ 이미 지나온 루트와 현재 방 표시
                        if (saving.curRoomNumber == i && saving.curRoomRow == j)
                        {
                            newCellUI.Find("Highlight_Current").gameObject.SetActive(true);
                            newCellUI.Find("Disabled").gameObject.SetActive(false);
                        }
                        else if ((mapInfos[j][i] & cell.Checked) == cell.Checked)
                        {
                            newCellUI.Find("Highlight_Passed").gameObject.SetActive(true);
                            newCellUI.Find("Disabled").gameObject.SetActive(false);
                        }

                        //▼ 어떤 방인지 표시
                        if ((mapInfos[j][i] & ~cell.ClrType) == cell.Normal)
                        {
                            newCellUI.Find("Normal").gameObject.SetActive(true);
                        } 
                        else if ((mapInfos[j][i] & ~cell.ClrType) == cell.Elite)
                        {
                            newCellUI.Find("Elite").gameObject.SetActive(true);
                        } 
                        else if ((mapInfos[j][i] & ~cell.ClrType) == cell.Shop)
                        {
                            newCellUI.Find("Shop").gameObject.SetActive(true);
                        }
                        else if ((mapInfos[j][i] & ~cell.ClrType) == cell.Event)
                        {
                            newCellUI.Find("Event").gameObject.SetActive(true);
                        }
                        else
                        {
                            newCellUI.Find("Default").gameObject.SetActive(true);
                        }

                        uiObjects.Add(newBoxUI);
                        uiObjects.Add(newCellUI);
                    }
                }
            }
        }
        public List<park.cell> GetMapData()//2차원리스트->1차원리스트 getter
        {
            List<park.cell> mapSave = new List<park.cell>();
            for (int i = 0; i < mapInfos.Count; i++)
            {
                for (int j = 0; j < mapInfos[i].Count; j++)
                {
                    mapSave.Add(mapInfos[i][j]);
                }
            }
            return mapSave;
        }
        public void SetMapData(List<park.cell> mapSave)//1차원리스트->2차원리스트 setter
        {
            mapInfos = new List<List<cell>>(row);
            for (int i = 0; i < row; i++)
            {
                List<park.cell> temp = new List<park.cell>(col);
                for (int j = 0; j < col; j++)
                {
                    temp.Add(mapSave[i * col + j]);
                }
                mapInfos.Add(temp);
            }
        }
        public void SelectableButtonsActive()//씬 내의 활동이 끝난 후 호출될 메소드. 다음 방을 고를 때 비활성화되어있던 버튼들이 활성화.
        {
            foreach (RectTransform rect in selectables)
            {
                rect.GetComponent<Button>().interactable = true;
                rect.Find("Disabled").gameObject.SetActive(false);
            }
        }
        public void UpdateMapData(int r, int c) //다음 Room을 선택했을 때 이 메소드로 MapInfos를 수정 후 SwitchScene이 변경사항을 저장.
        {
            if (saving.curRoomNumber != c - 1)
            {
                Debug.Log("뭔가 입력이 잘못됐다!");
            }
            else
            {
                Debug.Log("r:" + r + "c:" + c);
                if (saving.curRoomNumber>-1)mapInfos[saving.curRoomRow][saving.curRoomNumber] |= cell.Checked;
                saving.curRoomRow = r;
                saving.curRoomNumber = c;
            }
        }

        public void ArgumentsRandomize() //디버그용 : 안씀
        {
            // 행 ,열 ,여백 랜덤값지정
            row = (int)(Random.Range(3, 7));
            col = (int)(Random.Range(10, 50));
            margin = Random.Range(10,50);
        }
        public void DebugButtonclick() //디버그용 : 안씀
        {
            //초기에 맵 생성의 오류를 확인하기 위해 임시로 냅둔 버튼이 호출하던 메소드.
            //그냥 버튼 누르면 새로운 조합의 맵이 생성되는 것.
            //계속 눌러보면서 가지치는 꼴을 보면 재미있다.
            Clearing();
            Initializing();
            MapGeneration();
            MapDraw();
        }
        public void debugMapInfos() //디버그용 : 안씀
        {
            //각종 오류의 원인을 알게 해준 일등공신.
            //4~5개의 오타를 성공적으로 고칠 수 있었다.
            //디버그로그는 신이다
            Debug.Log(mapInfos != null);
            for (int j =0; j<row; j++)
            {
                for (int i = 0; i < col; i++)
                {
                    Debug.Log("방 좌표 :" +j.ToString() + i.ToString() + mapInfos[j][i]);
                }
            }
        }
    }
}