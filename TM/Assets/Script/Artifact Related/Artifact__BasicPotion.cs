using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Artifact__BasicPotion : Artifact
{
    public Artifact__BasicPotion()
    {
        this.artifactName = "그냥 포션";
        this.realArtifactName = "Artifact__BasicPotion";
    }

    //public override void atEarn(Player player)

    //public override void atBattleStart(Player player) { }

    //public override void atShopStart(Player player) { }

    //public override void atEventStart(Player player) { }

    public override void use(Player player)
    {
        player.hpHeal(8);
        print("used " + artifactName);
        player.deleteArtifact();
    }

    //public override void atDestroy(Player player) { }

    //public override void atClear(Player player) { }
}
