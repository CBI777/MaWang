using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ScriptableArtifact : ScriptableObject
{
    [SerializeField]
    private List<string> artifactDictionary = new List<string>();

    [SerializeField]
    private List<string> artifactRealName = new List<string>();

    [SerializeField]
    private List<string> artifactDescription = new List<string>();

    [SerializeField]
    private List<int> artifactPrice = new List<int>();

    /// <summary>
    /// 1������ �����մϴ�. �ش�Ǵ� artifact�� ��¥�̸�(Artifact__...)�� return
    /// </summary>
    /// <param name="n"></param>
    /// <returns></returns>
    public string getArtifact(int n)
    {
        return artifactDictionary[n];
    }
    /// <summary>
    /// 1������ �����մϴ�. �ش�Ǵ� artifact�� �̸��� return
    /// </summary>
    /// <param name="n"></param>
    /// <returns></returns>
    public string getArtifactRealName(int n)
    {
        return artifactRealName[n];
    }
    public string getArtifactDescription(int n)
    {
        return artifactDescription[n];
    }
    public int getArtifactPrice(int n)
    {
        return artifactPrice[n];
    }

    //����ٰ� int list�� �ϳ� �� ���� artifact�� �������� ����ϰų�,
    //string list�� �ϳ� �� ���� ���� ������ �ϴ� ���������� �˷��ִ� �뵵�ε� �� �� �ֽ��ϴ�.
}
