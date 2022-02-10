using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBase : CharacterBase
{
    [SerializeField] private int gold;
    [SerializeField]
    private string[] artifacts = new string[3];
    //private List<Artifact> artifact;

    public int getGold() { return this.gold; }
    public string getArtifact(int num)
    {
        if (num >= 0 && num <= 2)
        {
            return artifacts[num];
        }
        else
        {
            return "Invalid";
        }
    }
    public void updatePlayer(PlayerSaveBase player)
    {
        this.characterName = player.characterName;
        this.moveSpeed = player.moveSpeed;
        this.moveDistance = player.moveDistance;
        this.strength = player.strength;
        this.attackSpeed = player.attackSpeed;
        this.maxHp = player.maxHp;
        this.hp = player.hp;
        this.gold = player.gold;
        this.artifacts[0] = player.artifact1;
        this.artifacts[1] = player.artifact2;
        this.artifacts[2] = player.artifact3;
    }
}
