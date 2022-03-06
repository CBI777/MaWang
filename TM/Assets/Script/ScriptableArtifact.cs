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
    /// 1번부터 시작합니다. 해당되는 artifact의 진짜이름(Artifact__...)를 return
    /// </summary>
    /// <param name="n"></param>
    /// <returns></returns>
    public string getArtifact(int n)
    {
        return artifactDictionary[n];
    }
    /// <summary>
    /// 1번부터 시작합니다. 해당되는 artifact의 이름을 return
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

    //여기다가 int list를 하나 더 만들어서 artifact의 가격으로 사용하거나,
    //string list를 하나 더 만들어서 무슨 역할을 하는 유물인지를 알려주는 용도로도 쓸 수 있습니다.
}
