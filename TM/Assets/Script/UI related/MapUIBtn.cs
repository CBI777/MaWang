using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapUIBtn : MonoBehaviour
{
    /*
     * 2022_02_12_08:04
     * 임시로 만들었습니다.
     * MAP 관련 UI에서 다음 레벨을 고를 때, 그 버튼의 이벤트 처리를 위해 만들었습니다.
     * 후에 아래 코드를 MapUI스크립트에 합쳐놓고 버튼마다 MapUI를 넣을 수도 있습니다 (용량은 신경 안써도 되지 않을까)
     * (그렇게 된다면 아마 MapUI는 다른 스크립트에서 호출할 메소드로만 이루어져 있을 예정)
     */
    
    public void Cell_Clicked()
    {
        //참조 등의 문제로 UIManager를 Static으로 만들거나 하는 짓은 안하려고 했기에 findtag로 접근
        GameObject uiManager = GameObject.FindWithTag("UIManager");
        if (uiManager != null)
        {
            uiManager.GetComponent<UIManager>().panel_AccessCheck_Active(transform.parent.gameObject.GetComponent<RectTransform>());
        }
    }
}
