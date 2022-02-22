using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace park
{

    /*
     * �� �ڵ�� �Ŀ� LevelManager, UIManager �ڵ� � ������ �����Դϴ�.
     * ó�� �ۼ� �� ���� ������Ÿ���� Canvas > Scroll View�� ���� �־�����
     * 
     * 2022-02-07
     * cell�̶�� enum�� �߰��Ͽ� ���е��� �ʾҴ� Normal,Elite,Shop,Event �� ������ �����ϰ� ��
     * �ּ� ���� �߰�
     */

    // �� mapInformation(����ü) �̶� cell(enum)�� ���� ���� ���� �ž�?
    // ���ĵ� ���� �ʳ�? _02_07_03:26 -> ���Ĺ��Ƚ��ϴ� ^��^ _02_07_03:35

    // 2022_02_19
    // ���� ������ ������ ����ٸ� �װ��� ���� Ȯ���� MapDraw�� ������ ������ ��. ����

    public enum cell
    {
        /*
         * �⺻���� ���� ���� ����� ��Ʈ�÷���
         * �� �������� ����. (�������� ���� ������ھ�! : 0b00000000 �� ���� ǥ���� �⺻���ε� �� ���̿� ������ھ�� �����Ӱ� ��ġ ����)
         * 1. ù �� ��Ʈ : ���� or �������� or �����Դ°�? (���� �Ʒ� �Ӽ��� �������� ����� ģ����)
         * 2. ��� �� ��Ʈ : ��Ʈ���� ���� �ٷ� ������ ���� ��, �Ʒ��� ���� ��, ���� ���� �濡 ���� �÷���
         * 3. ������ �� ��Ʈ : ���� Ÿ���� ����(�븻/����Ʈ/����/�̺�Ʈ...)
         * ���� Ȯ�� ��� : ��ǥcell�� cell.���ñ⸦ ��ƮAND(&) ���� �� == cell.���ñ�
         * �� �������� �Ӽ� ��� ������ �� ��ƮOR(|) ����
         */

        Null    = 0b_00_000_000, //�ʱⰪ. Cell ����(ȭ�� ǥ�� X) �浵 ����

        //Ÿ��(�븻/����Ʈ/����/�̺�Ʈ...)
        ClrType = 0b_11_111_000, //Ÿ���� �ʱ�ȭ�� �� &�����ڿ� ���� ���̴� Flag. 
        Normal  = 0b_00_000_001, 
        Elite   = 0b_00_000_010, 
        Shop    = 0b_00_000_011,
        Event   = 0b_00_000_100,

        //    (  �֤�)
        //��:{ ���椱 } ���� ȭ��ǥ
        //    (  �٤�)
        ClrWay = 0b_11_000_111,
        Upper   = 0b_00_001_000,
        Lower   = 0b_00_010_000,
        Straight= 0b_00_100_000,

        // ��Ÿ ����� �������� �Ӽ� (������/����������/�����Դ���üũ)
        ClrLoc  = 0b_00_111_111,
        //Initial = 0b_01_000_000, _02_07_05:30 ���� ������ ���� ���Ƽ� ���� + checked �� �߰��ϰ� �;���
        PreBoss = 0b_01_000_000,
        Boss    = 0b_10_000_000,
        Checked = 0b_11_000_000 // ������ ���ΰ�?
    }


    public class MapUI : MonoBehaviour
    {
        private SaveBase saving; //2022_02_16 �߰�

        [SerializeField] private ScrollRect scrollRect; // ��UI Ʋ

        [SerializeField] private float margin=20;  //�� ��/���ε��� ����
        [SerializeField] private int row=4, col=13; //���� ��, ��
        [SerializeField] private GameObject uiPrefab_box, uiPrefab_cell, uiPrefab_line; // UI�� ���� �׷��� ���� ������.
        [SerializeField] private float wayChangeProbability, wayAddProbability, EliteProbability, ShopProbability, EventProbability; //���� �Ӽ����� �ο��� Ȯ��
        [SerializeField] private List<RectTransform> uiObjects = new List<RectTransform>(); //�����յ��� ����, ������ ���� ���ϰ� ���� ���ӿ�����Ʈ ����Ʈ : �׽�Ʈ������ ���� ��� ���� ���ӿ��� �� �ʿ� ���� �� ����

        private List<RectTransform> selectables = new List<RectTransform>(3); //2022_02_16 �߰� : ������ư�� ȿ���� ���Ѵ�!

        public List<List<cell>> mapInfos; // ** �� ���� ����.
        /// <summary>
        /// mapinfos�� ��ü�� UIManager/ MapUIBtn/ SaveManager�� ����&�����մϴ�.
        /// �̵��� ��� mapInfos�� ���� �������� �ʽ��ϴ�.
        /// ���� ������ ������ �Ʒ��� SetMapData/UpdateMapData �޼ҵ带 ȣ���մϴ�.
        /// get/set�� saveManager�� ȣ���ϸ�, �̴� 2���� ����Ʈ�� 1���� ����Ʈ�� ��ȯ�ؾ��ϱ� ����
        /// �� ���� �б� �뵵�� ������ ��ü�� Public�̱⿡ ���� �����մϴ�. Get�� ���� ���� �ʽ��ϴ�.
        /// 
        /// </summary>

        public void Start() //2022_02_18 saving�� �ν����� ������ ���������� �����ϰ� Start���� ����
        // �˼��մϴ� �̰� ��� �� �� ���� ������ �ϰ� scriptableobject�� ���� ã�ƺ��ٰ� �ʾ����ϴ�
        {
            saving = GameObject.FindWithTag("LevelManager").GetComponent<SaveManager>().saving;
        }

        
        public void Initializing() //mapInfos �ʱ�ȭ
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
        public void Clearing() //mapInfos & uiObjects & selectables ���� �����
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

        public void SelectDraw(Transform button, bool flag)//���� ��� ���� �����ϴ� �� ǥ��
        {
            foreach (RectTransform rect in selectables)
            {
                rect.Find("Highlight_Selected").gameObject.SetActive(false);
            }
            button.Find("Highlight_Selected").gameObject.SetActive(flag);
        } 
        public void MapGeneration() //mapInfos�� ������ ���� 
        {
            /*
             * MapInfos�� ������ �����ϴ� ��
             * 
             * �⺻ �˰���
             * �翬�� ���� for���ε� �� ������ ���� �����ؼ�
             * n�� ���� ó�� �� (n+1)�� ó����
             * n���� ���� �̾����� ���� (n+1)���� ���� ����
             * (n+1)���� ���� �˾Ƽ� Ÿ�� ���� �� ���� �� �����
             * ������ ���� ���� �̾����� ���� 1~3����
             * 
             * ���� �߰��� ���� �����̿�? ���ϼŵ� �˴ϴ� ^^
             * �� �ϳ��� ���� ���� ���� ���� ������ �ϳ� �̻��̶� �������� �������� ����ϴ�!
             * 
             * ���� X�ڷ� ũ�ν� �Ǵ°� �����ϱ� ���� �����ƺ��� ������������ �׷��� �ٶ��� ������ XD
             */

            for (int i= 0; i < col; i++)
            {
                for (int j= 0; j < row; j++)
                {
                    cell c;
                    c = cell.Null;

                    if (i == 0) // ù��° ���̸� �׳� Normal & Straight : �� �굵 �������� ���̳� Ÿ�� �ٲ� �� �ִµ� ���� �����Ƽ� ���� �س���
                    {
                        c = cell.Normal | cell.Straight;
                    }
                    else if (i == col - 1)  // ������ ���̸�(==����) Boss ����
                    {
                        if (j == 0)
                            c = cell.Boss;
                        else
                            c = cell.Null;
                    }
                    else
                    {
                        if ((mapInfos[j][i - 1]&cell.Straight)==cell.Straight) // ��ĭ�� �ձ� ������ : �� ����
                        {
                            c = cell.Normal | cell.Straight;
                        }
                        else if (j == 0) // �� �����̸� => �޾Ʒ�ĭ�� ���ʱ� ���� ? �� ���� : NULL
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
                        else if (j == row - 1) // �� �Ʒ����̸� => ����ĭ�� �Ʒ��ʱ� ���� ? �� ���� : NULL
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
                        else // �� ���ٵ� �ƴϰ� �� �Ʒ��ٵ� �ƴϸ� => ����ĭ�� �Ʒ��ʱ� || �޾Ʒ�ĭ�� ���ʱ� ? �� ���� : NULL
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
                        // �� ���� ���� ������ ���� Null or �� ������ ����.

                        if (i == col - 1)  // ������ �̹� ������
                        {
                            continue;
                        }
                        else if (i == col - 2) // ������ �� ���̸�(==���� ���� �ܰ��) PreBoss ����
                        {
                            c = cell.PreBoss;
                        }
                        else
                        {
                            // �� ���� ���⿡ �������� �ο�
                            if (Random.Range(0f, 1f) < wayChangeProbability) // ���� �⺻�� (����) �� �ٸ� �������� �ٲٴ� ���
                            {

                                if (j != 0 && Random.Range(0f, 2f) > 1) // �� ���� �ƴϰ� 50% => ���� ��� �ٲٱ�
                                {
                                    c &= cell.ClrWay;
                                    c |= cell.Upper;
                                }
                                else if (j != row - 1) // �� �Ʒ��� �ƴϰ� 50% => �Ʒ��� ��� �ٲٱ�
                                {
                                    c &= cell.ClrWay;
                                    c |= cell.Lower;
                                }
                            }
                            else if (Random.Range(0f, 1f) < wayAddProbability) // �� �� �ٸ� ������ ���� �߰��ϴ� ��� 1
                            {
                                if (j != 0 && Random.Range(0f, 2f) > 1) //�� ���� �ƴϰ� 50% => ���� �� �߰�
                                {
                                    c |= cell.Upper;
                                }
                                else if (j != row - 1) // �� �Ʒ��� �ƴϰ� 50% => �Ʒ��� �� �߰�
                                {
                                    c |= cell.Lower;
                                }

                                if (Random.Range(0f, 1f) < wayAddProbability) // �� �� �ٸ� ������ ���� �߰��ϴ� ��� 2 
                                {
                                    // ������ �� �߰�
                                    if (j != 0) c |= cell.Upper;
                                    if (j != row - 1) c |= cell.Lower;
                                    c |= cell.Straight;
                                }
                            }
                        }

                        // �� ���� Ÿ��(�븻/����Ʈ/�̺�Ʈ..) ����
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
        public void MapDraw() //** UI�� ���� �׸��� (Instantiate) �� �Ӽ� ����
        {
            selectables.Clear();
            float grid; // ��, ���� ������ ���� �� ĭ�� ũ�⸦ ���������� ���. 

            Debug.Log(scrollRect);
            Debug.Log(scrollRect.content);
            scrollRect.content.sizeDelta = new Vector2(scrollRect.content.rect.height / row * col, scrollRect.GetComponent<RectTransform>().sizeDelta.y-margin);
            grid = scrollRect.content.rect.height / row;
            scrollRect.content.anchoredPosition = new Vector2(0, 0);
            //��UI�� Ʋ�� ũ�⸦ ����! (��, ���� �����Ӱ� �ٲ� �� �ʿ�����)
            //���� ��ġ�� ���ڴ�� �������� ��ġ �����ϴ� �ڵ嵵 �־���
            
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
                    // �� �� �׸���
                    // �� �� �׸���
                    if (mapInfos[j][i] != cell.Null)
                    {
                        var newBoxUI = Instantiate(uiPrefab_box, scrollRect.content).GetComponent<RectTransform>();
                        newBoxUI.sizeDelta = new Vector2(grid, grid);
                        newBoxUI.anchoredPosition = new Vector2(i * grid, -j * grid);
                        var newCellUI = Instantiate(uiPrefab_cell, newBoxUI).GetComponent<RectTransform>();
                        newCellUI.sizeDelta = new Vector2(grid - margin, grid - margin);
                        newCellUI.gameObject.GetComponent<MapUIBtn>().xIndex = i;
                        newCellUI.gameObject.GetComponent<MapUIBtn>().yIndex = j;

                        //�� ������ �׸��� : Ư�����(�԰��� �ٸ�..?)
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

                        //�� ���� ��� ����� ���� ����� ����. -> ���� ���� ���� �� ��ư�� Ȱ��ȭ�� ���.
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

                        //�� �̹� ������ ��Ʈ�� ���� �� ǥ��
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

                        //�� � ������ ǥ��
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
        public List<park.cell> GetMapData()//2��������Ʈ->1��������Ʈ getter
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
        public void SetMapData(List<park.cell> mapSave)//1��������Ʈ->2��������Ʈ setter
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
        public void SelectableButtonsActive()//�� ���� Ȱ���� ���� �� ȣ��� �޼ҵ�. ���� ���� �� �� ��Ȱ��ȭ�Ǿ��ִ� ��ư���� Ȱ��ȭ.
        {
            foreach (RectTransform rect in selectables)
            {
                rect.GetComponent<Button>().interactable = true;
                rect.Find("Disabled").gameObject.SetActive(false);
            }
        }
        public void UpdateMapData(int r, int c) //���� Room�� �������� �� �� �޼ҵ�� MapInfos�� ���� �� SwitchScene�� ��������� ����.
        {
            if (saving.curRoomNumber != c - 1)
            {
                Debug.Log("���� �Է��� �߸��ƴ�!");
            }
            else
            {
                Debug.Log("r:" + r + "c:" + c);
                if (saving.curRoomNumber>-1)mapInfos[saving.curRoomRow][saving.curRoomNumber] |= cell.Checked;
                saving.curRoomRow = r;
                saving.curRoomNumber = c;
            }
        }

        public void ArgumentsRandomize() //����׿� : �Ⱦ�
        {
            // �� ,�� ,���� ����������
            row = (int)(Random.Range(3, 7));
            col = (int)(Random.Range(10, 50));
            margin = Random.Range(10,50);
        }
        public void DebugButtonclick() //����׿� : �Ⱦ�
        {
            //�ʱ⿡ �� ������ ������ Ȯ���ϱ� ���� �ӽ÷� ���� ��ư�� ȣ���ϴ� �޼ҵ�.
            //�׳� ��ư ������ ���ο� ������ ���� �����Ǵ� ��.
            //��� �������鼭 ����ġ�� ���� ���� ����ִ�.
            Clearing();
            Initializing();
            MapGeneration();
            MapDraw();
        }
        public void debugMapInfos() //����׿� : �Ⱦ�
        {
            //���� ������ ������ �˰� ���� �ϵ����.
            //4~5���� ��Ÿ�� ���������� ��ĥ �� �־���.
            //����׷α״� ���̴�
            Debug.Log(mapInfos != null);
            for (int j =0; j<row; j++)
            {
                for (int i = 0; i < col; i++)
                {
                    Debug.Log("�� ��ǥ :" +j.ToString() + i.ToString() + mapInfos[j][i]);
                }
            }
        }
    }
}