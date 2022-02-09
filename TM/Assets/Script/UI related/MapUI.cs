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

        [SerializeField] private ScrollRect scrollRect; // 맵UI 틀

        [SerializeField] private float margin;  //각 셀/라인들의 여백
        [SerializeField] private int row, col; //맵의 행, 열
        [SerializeField] private GameObject uiPrefab_box, uiPrefab_cell, uiPrefab_line; // UI에 직접 그려질 원본 프리팹.
        [SerializeField] private List<RectTransform> uiObjects = new List<RectTransform>(); //프리팹들의 생성, 삭제를 보다 편리하게 해줄 게임오브젝트 리스트 : 테스트용으로 자주 썼고 실제 게임에선 별 필요 없을 수 있음
        [SerializeField] private float wayChangeProbability, wayAddProbability, EliteProbability, ShopProbability, EventProbability; //여러 속성들의 부여될 확률

        private List<List<cell>> mapInfos; // ** 맵 정보 저장.

        // Start is called before the first frame update
        void Start()
        {
            initializing();
            MapGeneration();
            MapDraw();
        }

        public void ArgumentsRandomize() //테스트용 : 행,열 개수 바꾸고 여백 바꾸고 등등... 지금은 4*13으로 정해져서 필요없음
        {

            this.row = (int)(Random.Range(3, 7));
            this.col = (int)(Random.Range(10, 50));
            this.margin = Random.Range(10,50);
        }
        public void initializing() //mapInfos 초기화
        {
            mapInfos = new List<List<cell>>(row);
            for (int i = 0; i < this.row; i++)
            {
                mapInfos.Add(new List<cell>(col));
                for (int j =0; j < this.col; j++)
                {
                    cell c;
                    c = cell.Null;
                    
                    mapInfos[i].Add(c);
                }
            }

        }
        public void clearing() //mapInfos & uiObjects 내용 지우기
        {
            mapInfos.Clear();
            foreach(var obj in uiObjects)
            {
                Destroy(obj.gameObject);
            }
            uiObjects.Clear();
        }

        public void MapGeneration()
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

            for (int i= 0; i < this.col; i++)
            {
                for (int j= 0; j < this.row; j++)
                {
                    cell c;
                    c = cell.Null;

                    if (i == 0) // 첫번째 열이면 그냥 Normal & Straight : 뭐 얘도 무리없이 길이나 타입 바꿀 수 있는데 단지 귀찮아서 대충 해놨음
                    {
                        c = cell.Normal | cell.Straight;
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



                        if (i == this.col - 1)  // 마지막 열이면(==보스) Boss 설정
                        {
                            c = cell.Boss;
                        }
                        else if (i == this.col - 2) // 마지막 전 열이면(==보스 직전 단계면) PreBoss 설정
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
        public void MapDraw() //UI에 직접 그리기 (Instantiate)
        {
            float grid; // 행, 열의 개수에 따른 한 칸의 크기를 유동적으로 사용. 

            scrollRect.content.sizeDelta = new Vector2(scrollRect.content.rect.height / row * col, scrollRect.GetComponent<RectTransform>().sizeDelta.y-margin);
            grid = scrollRect.content.rect.height / row;
            scrollRect.content.anchoredPosition = new Vector2(0, 0);
            //맵UI의 틀의 크기를 설정! (행, 열이 자유롭게 바뀔 때 필요했음)
            //가끔 위치가 지멋대로 엇나가서 위치 고정하는 코드도 넣었음
            
            for (int i=0;i<this.col;i++)
            {
                for (int j = 0; j < this.row; j++)
                {
                    
                    if ((mapInfos[j][i] & cell.Straight) == cell.Straight)
                    {
                        var newLineUI = Instantiate(uiPrefab_line, scrollRect.content).GetComponent<RectTransform>();
                        newLineUI.sizeDelta = new Vector2(grid, margin*0.5f);
                        newLineUI.rotation = Quaternion.Euler(0f, 0f, 0f);
                        newLineUI.anchoredPosition = new Vector2((i + 1) * grid, -(j + 0.5f) * grid);
                        if ((mapInfos[j][i] & cell.Checked) == cell.Checked)
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
                        if ((mapInfos[j][i] & cell.Checked) == cell.Checked)
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
                        if ((mapInfos[j][i] & cell.Checked) == cell.Checked)
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
                    if ((mapInfos[j][i] & ~cell.ClrType) != cell.Null)
                    {
                        var newBoxUI = Instantiate(uiPrefab_box, scrollRect.content).GetComponent<RectTransform>();
                        newBoxUI.sizeDelta = new Vector2(grid, grid);
                        newBoxUI.anchoredPosition = new Vector2(i * grid, -j * grid);
                        var newCellUI = Instantiate(uiPrefab_cell, newBoxUI).GetComponent<RectTransform>();
                        newCellUI.sizeDelta = new Vector2(grid - margin, grid - margin);
                        /* //아직 스프라이트가 제대로 마련되지 않았음 구현해야될 부분 _02_07_06:13
                        if ((mapInfos[j][i] & cell.Checked) == cell.Checked)
                        {
                            newCellUI.Find("").gameObject.SetActive(true);
                        }
                        else
                        {
                            newCellUI.Find("Black").gameObject.SetActive(true);
                        }
                        */
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
        
        public void Buttonclick()
        {
            clearing();
            initializing();
            MapGeneration();
            MapDraw();
        }

    }
}