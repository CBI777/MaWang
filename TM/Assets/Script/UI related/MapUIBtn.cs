using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapUIBtn : MonoBehaviour
{
    /*
     * 2022_02_12_08:04
     * �ӽ÷� ��������ϴ�.
     * MAP ���� UI���� ���� ������ �� ��, �� ��ư�� �̺�Ʈ ó���� ���� ��������ϴ�.
     * �Ŀ� �Ʒ� �ڵ带 MapUI��ũ��Ʈ�� ���ĳ��� ��ư���� MapUI�� ���� ���� �ֽ��ϴ� (�뷮�� �Ű� �Ƚᵵ ���� ������)
     * (�׷��� �ȴٸ� �Ƹ� MapUI�� �ٸ� ��ũ��Ʈ���� ȣ���� �޼ҵ�θ� �̷���� ���� ����)
     */
    
    public void Cell_Clicked()
    {
        //���� ���� ������ UIManager�� Static���� ����ų� �ϴ� ���� ���Ϸ��� �߱⿡ findtag�� ����
        GameObject uiManager = GameObject.FindWithTag("UIManager");
        if (uiManager != null)
        {
            uiManager.GetComponent<UIManager>().panel_AccessCheck_Active(transform.parent.gameObject.GetComponent<RectTransform>());
        }
    }
}
