using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Artifact : MonoBehaviour
{
    [SerializeField] protected string artifactName;

    public string getArtifactName()
    {
        return this.artifactName;
    }

    public void use()
    {
        print("used " + artifactName);
    }
}
//이것도 사실 prefab으로 빼두고 이름으로 찾는게 맞을 듯.

public class Artifact_Nothing : Artifact
{
    public Artifact_Nothing()
    {
        this.artifactName = "맨손";
    }
}