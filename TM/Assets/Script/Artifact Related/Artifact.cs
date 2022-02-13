using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class Artifact : MonoBehaviour
{
    /* 2022_02_13 - 유물의 획득시에 작동하는 함수
     * 모든 유물의 이름은 Artifact__????으로 통일을 하면 좋겠다고 생각합니다.
     * artifactName은 실제로 플레이를 하는 사람에게 보이는 이름.
     * realArtifactName은 코드적인 부분에서의 유물의 이름.
     * 
     * 예를 들어, 맨손은 artifactName은 '맨손'이지만,
     * realArtifactName은 그에 해당하는 Artifact__Hand다.
     */
    [SerializeField] protected string artifactName;

    [SerializeField] protected string realArtifactName;

    public string getArtifactName() { return this.artifactName; }
    public string getRealArtifactName() { return this.realArtifactName; }

    //2022_02_13 - 유물의 획득시에 작동하는 함수
    /* 1. atEarn - 획득시에 작동 : passive 유물의 경우, 적용 수치만큼 여기서 플레이어의 스탯을 변동하는 등으로 사용
     * 2. at????Start 함수들 - 각 스테이지의 시작에 작동 : 배틀로 예를 들면 적 전체 공격력 1감소...등.
     *                                                     상점으로 예를 들면 상점 가격 75%로 조정... 등.
     * 3. use - 유물의 직접적인 사용시에 작동 : 123으로 유물을 고르고 사용할 때 함수.
     * 4. atDestroy - 유물을 파괴할 때 작동하는 함수 : Destroy()함수와는 다르게, 1회용의 사용, 유물의 교체등이 이루어질 때 부른다.
     * 예를 들어서, passive 유물에서 변동되었던 스탯을 다시금 원상복구 하는데에 사용하는 것이 대표적이겠다.
     * 제거는 player쪽에서 atDestroy를 부르고 맨손으로 교체하거나 하기 때문에, 이 안에서는 destroy를 하지 않는다.
     */

    //2022_02_13 - 유물의 획득시에 작동하는 함수
    public virtual void atEarn(Player player) { }

    //2022_02_13 - 스테이지의 시작에 작동하는 함수
    public virtual void atBattleStart(Player player) { }

    public virtual void atShopStart(Player player) { }

    public virtual void atEventStart(Player player) { }

    //2022_02_13 - 스테이지 내에서 유물을 직접적으로 사용했을 때 작동하는 함수
    public virtual void use(Player player)
    {
        print("used " + artifactName);
    }

    //2022_02_13 - 유물의 파괴(1회용은 사용, 교체...)시에 작동하는 함수
    public virtual void atDestroy(Player player) { }

    //2022_02_13 - 스테이지 완료시에 작동하는 함수
    public virtual void atClear(Player player) { }
}