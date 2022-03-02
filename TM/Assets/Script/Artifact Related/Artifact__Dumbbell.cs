using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Artifact__Dumbbell : Artifact
{
    public Artifact__Dumbbell()
    {
        this.artifactName = "¾Æ·É";
        this.realArtifactName = "Artifact__Dumbbell";
    }

    public override void atEarn(Player player)
    {
        player.changeStr(2);
    }

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

    public override void atDestroy(Player player)
    {
        player.changeStr(-2);
    }

    //public override void atClear(Player player) { }
}
