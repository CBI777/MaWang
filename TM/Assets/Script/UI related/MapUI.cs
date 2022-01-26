using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace park
{
    public struct mapInformation
    {
        public bool upper;
        public bool lower;
        public bool straight;
        public short prior;
    };

    public class MapUI : MonoBehaviour
    {

        private ScrollRect scrollRect;

        [SerializeField] private float margin;
        [SerializeField] private int row, col;
        [SerializeField] private GameObject uiPrefab_box, uiPrefab_cell, uiPrefab_line;
        [SerializeField] private List<RectTransform> uiObjects = new List<RectTransform>();
        [SerializeField] private float wayChangeProbability, wayAddProbability;

        private List<List<mapInformation>> mapInfos;

        // Start is called before the first frame update
        void Start()
        {
            scrollRect = GetComponent<ScrollRect>();
            ArgumentsRandomize();
            initializing();
            MapGeneration();
            MapDraw();
        }

        public void ArgumentsRandomize()
        {

            this.row = (int)(Random.Range(3, 7));
            this.col = (int)(Random.Range(10, 50));
            this.margin = Random.Range(10,50);
        }
        public void initializing()
        {
            mapInfos = new List<List<mapInformation>>(row);
            for (int i = 0; i < this.row; i++)
            {
                mapInfos.Add(new List<mapInformation>(col));
                for (int j =0; j < this.col; j++)
                {
                    mapInformation mi;
                    mi.upper = false;
                    mi.lower = false;
                    mi.straight = false;
                    mi.prior = 0;
                    
                    mapInfos[i].Add(mi);
                }
            }

        }
        public void clearing()
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
            for (int i= 0; i < this.col; i++)
            {
                for (int j= 0; j < this.row; j++)
                {
                    mapInformation mi;
                    mi.upper = false;
                    mi.lower = false;
                    mi.straight = true;
                    mi.prior = 1;

                    if (i == 0)
                    {
                        mi.prior = 2;
                    }
                    else
                    {
                        if (mapInfos[j][i - 1].straight)
                        {
                            mi.prior = 1;
                        }
                        else if (j == 0)
                        {
                            if (mapInfos[j + 1][i - 1].upper) mi.prior = 1;
                            else
                            {
                                mi.prior = 0;
                                mi.straight = false;
                                mapInfos[j][i] = mi;
                                continue;
                            }
                        }
                        else if (j == row - 1)
                        {
                            if (mapInfos[j - 1][i - 1].lower) mi.prior = 1;
                            else
                            {
                                mi.prior = 0;
                                mi.straight = false;
                                mapInfos[j][i] = mi;
                                continue;
                            }
                        }
                        else
                        {
                            if (mapInfos[j - 1][i - 1].lower || mapInfos[j + 1][i - 1].upper) mi.prior = 1;
                            else
                            {
                                mi.prior = 0;
                                mi.straight = false;
                                mapInfos[j][i] = mi;
                                continue;
                            }
                        }

                        if (i == this.col - 1)
                        {
                            mi.straight = false;
                            mi.prior = 3;
                        }
                        else if (i == this.col - 2)
                        {
                            mi.straight = false;
                        }
                        else
                        {
                            if (Random.Range(0f, 1f) < wayChangeProbability)
                            {

                                if (j != 0 && Random.Range(0f, 2f) > 1)
                                {
                                    mi.upper = true;
                                    mi.straight = false;
                                }
                                else if (j != row - 1)
                                {
                                    mi.lower = true;
                                    mi.straight = false;
                                }
                            }
                            else if (Random.Range(0f, 1f) < wayAddProbability)
                            {
                                if (j != 0 && Random.Range(0f, 2f) > 1) { mi.upper = true; }
                                else if (j != row - 1) { mi.lower = true; }
                                if (Random.Range(0f, 1f) < wayAddProbability)
                                {
                                    if (j != 0) mi.upper = true;
                                    if (j != row - 1)  mi.lower = true;
                                    mi.straight = true;
                                }
                            }
                        }
                    }
                    

                    
                    mapInfos[j][i] = mi;
                }
            }
        }
        public void MapDraw()
        {
            float grid;

            scrollRect.content.sizeDelta = new Vector2(scrollRect.content.rect.height / row * col, scrollRect.GetComponent<RectTransform>().sizeDelta.y-margin);
            grid = scrollRect.content.rect.height / row;
            scrollRect.content.anchoredPosition = new Vector2(0, 0);
            for (int i=0;i<this.col;i++)
            {
                for (int j = 0; j < this.row; j++)
                {
                    
                    if (mapInfos[j][i].straight)
                    {
                        var newLineUI = Instantiate(uiPrefab_line, scrollRect.content).GetComponent<RectTransform>();
                        newLineUI.sizeDelta = new Vector2(grid, margin*0.5f);
                        newLineUI.rotation = Quaternion.Euler(0f, 0f, 0f);
                        newLineUI.anchoredPosition = new Vector2((i + 1) * grid, -(j + 0.5f) * grid);
                        uiObjects.Add(newLineUI);
                    }
                    if (mapInfos[j][i].upper)
                    {
                        var newLineUI = Instantiate(uiPrefab_line, scrollRect.content).GetComponent<RectTransform>();
                        newLineUI.sizeDelta = new Vector2(grid*1.4f, margin*0.5f );
                        newLineUI.rotation = Quaternion.Euler(0f, 0f, 45f);
                        newLineUI.anchoredPosition = new Vector2((i + 1) * grid, -j * grid);
                        uiObjects.Add(newLineUI);
                    }
                    if (mapInfos[j][i].lower)
                    {
                        var newLineUI = Instantiate(uiPrefab_line, scrollRect.content).GetComponent<RectTransform>();
                        newLineUI.sizeDelta = new Vector2(grid*1.4f, margin*0.5f);
                        newLineUI.rotation = Quaternion.Euler(0f, 0f, -45f);
                        newLineUI.anchoredPosition = new Vector2((i + 1) * grid, -(j+1) * grid);
                        uiObjects.Add(newLineUI);
                    }
                    if (mapInfos[j][i].prior!=0)
                    {
                        var newBoxUI = Instantiate(uiPrefab_box, scrollRect.content).GetComponent<RectTransform>();
                        newBoxUI.sizeDelta = new Vector2(grid, grid);
                        newBoxUI.anchoredPosition = new Vector2(i * grid, -j * grid);
                        var newCellUI = Instantiate(uiPrefab_cell, newBoxUI).GetComponent<RectTransform>();
                        newCellUI.sizeDelta = new Vector2(grid - margin, grid - margin);
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