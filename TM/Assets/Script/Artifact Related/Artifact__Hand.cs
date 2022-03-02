using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Artifact__Hand : Artifact
{
    public Artifact__Hand()
    {
        this.artifactName = "¸Ç¼Õ";
        this.realArtifactName = "Artifact__Hand";
    }

    //public override void atEarn(Player player)

    //public override void atBattleStart(Player player) { }

    //public override void atShopStart(Player player) { }

    //public override void atEventStart(Player player) { }

    public override void use(Player player)
    {
        List<Vector3Int> range = new List<Vector3Int>();
        range.Add(DirectionChange.dirToVector(player.getDirections()));
        player.attack(range, 0.7f, "FistSlash", "Punch");
        print("used " + artifactName);
    }

    //public override void atDestroy(Player player) { }

    //public override void atClear(Player player) { }
}
