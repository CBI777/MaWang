using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Artifact__FourBladeSword : Artifact
{
    public Artifact__FourBladeSword()
    {
        this.artifactName = "»ç¸é°Ë";
        this.realArtifactName = "Artifact__FourBladeSword";
    }

    //public override void atEarn(Player player)

    //public override void atBattleStart(Player player) { }

    //public override void atShopStart(Player player) { }

    //public override void atEventStart(Player player) { }

    public override void use(Player player)
    {
        List<Vector3Int> range = new List<Vector3Int>();
        range.Add(new Vector3Int(0, 1, 0));
        range.Add(new Vector3Int(0, -1, 0));
        range.Add(new Vector3Int(1, 0, 0));
        range.Add(new Vector3Int(-1, 0, 0));
        player.attack(range, 1f, "SwordSlash", "Slash1");
    }

    //public override void atDestroy(Player player) { }

    //public override void atClear(Player player) { }
}
