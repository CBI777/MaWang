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
//�̰͵� ��� prefab���� ���ΰ� �̸����� ã�°� ���� ��.

public class Artifact_Nothing : Artifact
{
    public Artifact_Nothing()
    {
        this.artifactName = "�Ǽ�";
    }
}