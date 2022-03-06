using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ClearPanel : MonoBehaviour
{
    [SerializeField]
    ScriptableArtifact artifactDictionary;

    [SerializeField]
    UnityEngine.UI.Button gold;
    [SerializeField]
    UnityEngine.UI.Button artifact0;
    [SerializeField]
    UnityEngine.UI.Button artifact1;
    [SerializeField]
    UnityEngine.UI.Button artifact2;
    [SerializeField]
    UnityEngine.UI.Button hp;
    [SerializeField]
    UnityEngine.UI.Button atkspd;
    [SerializeField]
    UnityEngine.UI.Button movspd;
    [SerializeField]
    UnityEngine.UI.Button str;


    [SerializeField]
    UnityEngine.UI.Text goldText;
    [SerializeField]
    UnityEngine.UI.Image goldImage;
    [SerializeField]
    UnityEngine.UI.Text artifactText0;
    [SerializeField]
    UnityEngine.UI.Text artifactText1;
    [SerializeField]
    UnityEngine.UI.Text artifactText2;
    [SerializeField]
    UnityEngine.UI.Image artifactImage;
    [SerializeField]
    UnityEngine.UI.Image statusBackground;

    [SerializeField]
    UnityEngine.UI.Text hpText;
    [SerializeField]
    UnityEngine.UI.Text atkspdText;
    [SerializeField]
    UnityEngine.UI.Text movspdText;
    [SerializeField]
    UnityEngine.UI.Text strText;

    [SerializeField]
    public int goldAward;
    [SerializeField]
    public int artiAward;
    [SerializeField]
    public int statChangeAward;

    [SerializeField]
    UnityEngine.UI.Text description;

    public void setAwards(int gld, int arti, int statChange)
    {
        this.goldAward = gld;
        this.artiAward = arti;
        this.statChangeAward = statChange;

        artifactImage.GetComponent<EventTrigger>().enabled = false;
        gold.interactable = true;
        goldText.text = goldAward.ToString() + "골드";
        goldImage.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
        goldImage.GetComponent<ImageAnimation>().enabled = true;
        gold.onClick.AddListener( () =>
        { this.changeGold(); gold.interactable = false; goldImage.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.5f); goldImage.GetComponent<ImageAnimation>().enabled = false; SoundEffecter.playSFX("EarnIt"); }
        );

        if (artiAward != 0)
        {
            artifact0.interactable = true;
            artifact1.interactable = true;
            artifact2.interactable = true;
            artifactText0.text = "1번 유물과 교체";
            artifactText1.text = "2번 유물과 교체";
            artifactText2.text = "3번 유물과 교체";
            artifactImage.sprite = Resources.Load<Sprite>("Artifacts/ArtifactImage/" + artifactDictionary.getArtifact(arti));
            artifactImage.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
            //22_03_02
            artifactImage.GetComponent<EventTrigger>().enabled = true;
            description.text = artifactDictionary.getArtifactDescription(artiAward);
            artifact0.onClick.AddListener(() =>
            { this.changeArtifact(1); artifact0.interactable = false; artifact1.interactable = false; artifact2.interactable = false; artifactImage.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.3f); SoundEffecter.playSFX("EarnIt"); });
            artifact1.onClick.AddListener(() =>
            { this.changeArtifact(2); artifact0.interactable = false; artifact1.interactable = false; artifact2.interactable = false; artifactImage.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.3f); SoundEffecter.playSFX("EarnIt"); });
            artifact2.onClick.AddListener(() =>
            { this.changeArtifact(3); artifact0.interactable = false; artifact1.interactable = false; artifact2.interactable = false; artifactImage.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.3f); SoundEffecter.playSFX("EarnIt"); });
        }

        if(statChangeAward != 0)
        {
            hp.interactable = true;
            hpText.text = "체력 " + (this.statChangeAward * 2) + "증가";
            atkspd.interactable = true;
            atkspdText.text = "공격속도 " + (this.statChangeAward) + "증가";
            movspd.interactable = true;
            movspdText.text = "이동속도 " + (this.statChangeAward * 2) + "증가";
            str.interactable = true;
            strText.text = "힘 " + (this.statChangeAward) + "증가";
            statusBackground.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);

            hp.onClick.AddListener(() =>
            { this.changeHp(); hp.interactable = false; atkspd.interactable = false; movspd.interactable = false; str.interactable = false;
                statusBackground.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.5f); SoundEffecter.playSFX("EarnIt");
            });
            atkspd.onClick.AddListener(() =>
            {
                this.changeAgl(); hp.interactable = false; atkspd.interactable = false; movspd.interactable = false; str.interactable = false;
                statusBackground.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.5f); SoundEffecter.playSFX("EarnIt");
            });
            movspd.onClick.AddListener(() =>
            {
                this.changeSpd(); hp.interactable = false; atkspd.interactable = false; movspd.interactable = false; str.interactable = false;
                statusBackground.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.5f); SoundEffecter.playSFX("EarnIt");
            });
            str.onClick.AddListener(() =>
            {
                this.changeStr(); hp.interactable = false; atkspd.interactable = false; movspd.interactable = false; str.interactable = false;
                statusBackground.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.5f); SoundEffecter.playSFX("EarnIt");
            });
        }
    }

    private void changeGold()
    {
        GameObject.FindWithTag("Player").GetComponent<Player>().changeGold(this.goldAward);
    }

    private void changeArtifact(int num)
    {
        GameObject.FindWithTag("Player").GetComponent<Player>().deleteArtifact(artifactDictionary.getArtifact(artiAward), num);
    }

    private void changeHp()
    {
        GameObject.FindWithTag("Player").GetComponent<Player>().changeMaxHP(this.statChangeAward*2);
    }

    private void changeStr()
    {
        GameObject.FindWithTag("Player").GetComponent<Player>().changeStr(this.statChangeAward);
    }

    private void changeSpd()
    {
        GameObject.FindWithTag("Player").GetComponent<Player>().changeMovSpd(this.statChangeAward*2);
    }

    private void changeAgl()
    {
        GameObject.FindWithTag("Player").GetComponent<Player>().changeAtkSpd(this.statChangeAward);
    }
}
