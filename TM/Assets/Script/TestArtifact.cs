using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestArtifact : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private int art;
    [SerializeField] private int cur;

    [SerializeField] private List<string> artName = new List<string>();

    private void Start()
    {
        art = 0;
        cur = 1;
        artName.Add("Artifact__BasicPotion");
        artName.Add("Artifact__Spear");
        artName.Add("Artifact__FourBladeSword");
        artName.Add("Artifact__Hand");
        artName.Add("Artifact__Dumbbell");
    }

    public void changeArtifact()
    {
        player.deleteArtifact(artName[art], cur);
        art++; cur++;
        if (art == 5) { art = 0; }
        if (cur == 4) { cur = 1; }
    }



}
