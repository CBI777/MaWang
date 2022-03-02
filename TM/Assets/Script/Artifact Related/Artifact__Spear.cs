using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Artifact__Spear : Artifact
{
    public Artifact__Spear()
    {
        this.artifactName = "¿Â√¢";
        this.realArtifactName = "Artifact__Spear";
    }
    //public override void atEarn(Player player)

    //public override void atBattleStart(Player player) { }

    //public override void atShopStart(Player player) { }

    //public override void atEventStart(Player player) { }

    public override void use(Player player)
    {
        List<Vector3Int> range = new List<Vector3Int>();
        List<Vector3Int> effRange = new List<Vector3Int>();
        range.Add(DirectionChange.dirToVector(player.getDirections()));
        range.Add(DirectionChange.dirToVector(player.getDirections()) * 2);
        effRange.Add(DirectionChange.dirToVector(player.getDirections()));
        player.attack(range, effRange, effRange, 1.2f, "SpearSlash", "Slash2");
        print("used " + artifactName);
    }

    //public override void atDestroy(Player player) { }

    //public override void atClear(Player player) { }

}
