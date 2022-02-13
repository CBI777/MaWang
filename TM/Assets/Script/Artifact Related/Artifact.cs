using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class Artifact : MonoBehaviour
{
    /* 2022_02_13 - ������ ȹ��ÿ� �۵��ϴ� �Լ�
     * ��� ������ �̸��� Artifact__????���� ������ �ϸ� ���ڴٰ� �����մϴ�.
     * artifactName�� ������ �÷��̸� �ϴ� ������� ���̴� �̸�.
     * realArtifactName�� �ڵ����� �κп����� ������ �̸�.
     * 
     * ���� ���, �Ǽ��� artifactName�� '�Ǽ�'������,
     * realArtifactName�� �׿� �ش��ϴ� Artifact__Hand��.
     */
    [SerializeField] protected string artifactName;

    [SerializeField] protected string realArtifactName;

    public string getArtifactName() { return this.artifactName; }
    public string getRealArtifactName() { return this.realArtifactName; }

    //2022_02_13 - ������ ȹ��ÿ� �۵��ϴ� �Լ�
    /* 1. atEarn - ȹ��ÿ� �۵� : passive ������ ���, ���� ��ġ��ŭ ���⼭ �÷��̾��� ������ �����ϴ� ������ ���
     * 2. at????Start �Լ��� - �� ���������� ���ۿ� �۵� : ��Ʋ�� ���� ��� �� ��ü ���ݷ� 1����...��.
     *                                                     �������� ���� ��� ���� ���� 75%�� ����... ��.
     * 3. use - ������ �������� ���ÿ� �۵� : 123���� ������ ���� ����� �� �Լ�.
     * 4. atDestroy - ������ �ı��� �� �۵��ϴ� �Լ� : Destroy()�Լ��ʹ� �ٸ���, 1ȸ���� ���, ������ ��ü���� �̷���� �� �θ���.
     * ���� ��, passive �������� �����Ǿ��� ������ �ٽñ� ���󺹱� �ϴµ��� ����ϴ� ���� ��ǥ���̰ڴ�.
     * ���Ŵ� player�ʿ��� atDestroy�� �θ��� �Ǽ����� ��ü�ϰų� �ϱ� ������, �� �ȿ����� destroy�� ���� �ʴ´�.
     */

    //2022_02_13 - ������ ȹ��ÿ� �۵��ϴ� �Լ�
    public virtual void atEarn(Player player) { }

    //2022_02_13 - ���������� ���ۿ� �۵��ϴ� �Լ�
    public virtual void atBattleStart(Player player) { }

    public virtual void atShopStart(Player player) { }

    public virtual void atEventStart(Player player) { }

    //2022_02_13 - �������� ������ ������ ���������� ������� �� �۵��ϴ� �Լ�
    public virtual void use(Player player)
    {
        print("used " + artifactName);
    }

    //2022_02_13 - ������ �ı�(1ȸ���� ���, ��ü...)�ÿ� �۵��ϴ� �Լ�
    public virtual void atDestroy(Player player) { }

    //2022_02_13 - �������� �Ϸ�ÿ� �۵��ϴ� �Լ�
    public virtual void atClear(Player player) { }
}