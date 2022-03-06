using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ShopUI : MonoBehaviour
{

    [SerializeField]
    ScriptableArtifact artifactDictionary;
    [SerializeField]
    Image[] img_artifact = new Image[3];
    [SerializeField]
    Button[] btn_buy = new Button[3];
    [SerializeField]
    Text[] description = new Text[3];
    [SerializeField]
    Text[] text_artifact_name = new Text[3];
    [SerializeField]
    RectTransform slotConfirmPanel;
    [SerializeField]
    Sprite  sold_out;

    [SerializeField]
    Player player;
    [SerializeField]
    SaveManager saveManager;
    SaveBase saving;

    public readonly int artifactCount = 5;
    private int[] slotIndex = new int[3];

    public int getSlotIndex(int n)
    {
        return slotIndex[n];
    }

    private void Awake()
    {
        saving = saveManager.saving;
        if (saveManager.getSameCheck())
        {
            slotIndex[0] = saving.stageVar1;
            slotIndex[1] = saving.stageVar2;
            slotIndex[2] = saving.stageVar3;
        }
        else
        {
            for (int i = 0; i < 3; i++) slotIndex[i] = (int)Random.Range(1f, artifactCount) % artifactCount + 1;
        }

        for (int i = 0; i < 3; i++)
        {
            img_artifact[i].sprite = Resources.Load<Sprite>("Artifacts/ArtifactImage/" + artifactDictionary.getArtifact(slotIndex[i]));
            description[i].text = artifactDictionary.getArtifact(slotIndex[i]).Substring(artifactDictionary.getArtifact(slotIndex[i]).LastIndexOf('_')) + "\n" + artifactDictionary.getArtifactDescription(slotIndex[i]) + "\n가격 : " + artifactDictionary.getArtifactPrice(slotIndex[i]) +"Gold"; 
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="index">상점의 몇 번 패널의 버튼을 눌렀나 0,1,2</param>
    public void BuyButtonClicked(int index){
        Text text = btn_buy[index].GetComponentInChildren<Text>();
        if (text.text.Equals("Buy"))
        {
            if (artifactDictionary.getArtifactPrice(slotIndex[index]) > player.getGold())
            {
                text.text = "Can't Afford";
            }
            else
            {
                text.text = "Sure?";
            }
        } else if (text.text.Equals("Sure?"))
        {
            int i;
            for (i = 0; i < 3; i++)
            {
                if (player.getArtifact(i).name.Equals(artifactDictionary.getArtifact(1)))
                {
                    player.deleteArtifact(artifactDictionary.getArtifact(slotIndex[index]), i+1);
                    img_artifact[index].sprite = sold_out;
                    btn_buy[index].interactable = false;
                    btn_buy[index].image.color = new Color(1f, 1f, 1f, 0.5f);
                    player.changeGold(-artifactDictionary.getArtifactPrice(slotIndex[index]));
                    slotConfirmPanel.gameObject.SetActive(false);
                    return;
                }
            }
            slotConfirmPanel.gameObject.SetActive(true);
            slotConfirmPanel.GetChild(0).Find("Btn1").GetComponent<Button>().onClick.AddListener(()=>
            {
                player.deleteArtifact(artifactDictionary.getArtifact(slotIndex[index]), 1);
                img_artifact[index].sprite = sold_out;
                btn_buy[index].interactable = false;
                btn_buy[index].image.color = new Color(1f, 1f, 1f, 0.5f);
                player.changeGold(-artifactDictionary.getArtifactPrice(slotIndex[index]));
                slotConfirmPanel.gameObject.SetActive(false);
                return;
            });
            slotConfirmPanel.GetChild(0).Find("Btn2").GetComponent<Button>().onClick.AddListener(() =>
            { 
                player.deleteArtifact(artifactDictionary.getArtifact(slotIndex[index]), 2);
                img_artifact[index].sprite = sold_out;
                btn_buy[index].interactable = false;
                btn_buy[index].image.color = new Color(1f, 1f, 1f, 0.5f);
                player.changeGold(-artifactDictionary.getArtifactPrice(slotIndex[index]));
                slotConfirmPanel.gameObject.SetActive(false);
                return;
            });
            slotConfirmPanel.GetChild(0).Find("Btn3").GetComponent<Button>().onClick.AddListener(() =>
            { 
                player.deleteArtifact(artifactDictionary.getArtifact(slotIndex[index]), 3);
                img_artifact[index].sprite = sold_out;
                btn_buy[index].interactable = false;
                btn_buy[index].image.color = new Color(1f, 1f, 1f, 0.5f);
                player.changeGold(-artifactDictionary.getArtifactPrice(slotIndex[index]));
                slotConfirmPanel.gameObject.SetActive(false);
                return;
            });
        }
        else
        {
            Debug.Log("????????????");
        }
    }

}
