using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : CharacterBase
{
    [SerializeField] private int gold;
    //2022_02_13 artifacts가 본격적으로 artifact 클래스에 대응할 수 있도록 처리
    [SerializeField]
    private Artifact[] artifacts = new Artifact[3];
    [SerializeField] private SpawnManager spawnManager;

    //2022_02_11 - UIManager를 UIManager로 하니까 제대로 인식이 안되는 문제가 있어서, uiManager로 변경하였습니다.
    [SerializeField] private UIManager uiManager;

    //2022_02_13 현재의 artifact를 확인하기 위한 curArtifact를 추가
    [SerializeField] private int curArtifact;

    public int getGold() { return this.gold; }
    //22_03_01
    public void changeGold(int amount)
    {
        if (this.gold+amount>=0) this.gold += amount;
        if(this.gold <= 0) this.gold = 0;
        uiManager.changeGold(this.gold);
    }

    //2022_02_13 getCurArtifact 추가
    public int getCurArtifact() { return this.curArtifact; }
    public void setCurArtifact(int num) { this.curArtifact = num; }
    //2022_02_13 artifacts가 본격적으로 artifact 클래스에 대응할 수 있도록 처리
    public Artifact getArtifact(int num)
    {
        if (num >= 0 && num <= 2)
        {
            return artifacts[num];
        }
        else
        {
            return null;
        }
    }
    //22_03_01
    public void changeMaxHP(int plusHp)
    {
        this.maxHp += plusHp;
        hpHeal(plusHp);
    }
    //22_03_01
    public override void changeStr(int plusStr)
    {
        base.changeStr(plusStr);
        uiManager.changeSTR(this.strength);
    }
    //22_03_01
    public void changeMovSpd(int plusSpd)
    {
        this.moveSpeed += plusSpd;
        uiManager.changeMOVSPD(this.moveSpeed);
    }
    //22_03_01
    public void changeAtkSpd(int plusAtkspd)
    {
        this.attackSpeed += plusAtkspd;
        uiManager.changeATKSPD(this.attackSpeed);
    }

    public void updatePlayer(SaveBase player)
    {
        //2022_02_13 CurArtifact관련 초기화 처리.
        this.curArtifact = 0;
        this.characterName = player.characterName;
        this.moveSpeed = player.moveSpeed;
        this.moveDistance = player.moveDistance;
        this.strength = player.strength;
        this.attackSpeed = player.attackSpeed;
        this.maxHp = player.maxHp;
        this.hp = player.hp;
        this.gold = player.gold;
        //2022_02_13 - 유물 불러오기
        this.artifacts[0] = Resources.Load<GameObject>("Artifacts/" + player.artifact1).GetComponent<Artifact>();
        this.artifacts[1] = Resources.Load<GameObject>("Artifacts/" + player.artifact2).GetComponent<Artifact>();
        this.artifacts[2] = Resources.Load<GameObject>("Artifacts/" + player.artifact3).GetComponent<Artifact>();
    }

    //2022_02_13 유물의 사용을 담당하는 함수가 useArtifact. 이제 이 함수가 UIManager에서 불리게 됩니다.
    public void useArtifact() 
    {
        if (uiManager.getAttackFlag())
        {
            this.artifacts[curArtifact].use(this);
        }

    }

    /// <summary>
    /// 원하는 곳에다 공격을 요청하는 함수. 보통 artifact내부에서 쓰임.
    /// attackRange로 지정한 곳에 effect가 모두 나옴.
    /// 만약, effect를 특정한 곳에서만 발동시키고 싶다면 overloading된 함수를 사용할 것
    /// </summary>
    /// <param name="attackRange">어디를 때릴건지(보통 player의 방향에 따라서 달라질거고,)</param>
    /// <param name="damageMultiply">공격계수. 소수점을 이용함. [player.strength*계수] 로 데미지가 결정</param>
    /// <param name="effectName">공격에 사용되는 effect이름. Effect 폴더 내의 이름을 써주면 된다.</param>
    /// <param name="sfxName">공격의 효과음 이름. SoundEffects 폴더 내의 이름을 써주면 된다.</param>
    public void attack(List<Vector3Int> attackRange, float damageMultiply, string effectName, string sfxName)
    {
        if(spawnManager.AttackCharacter(transform.position, attackRange,
            Mathf.RoundToInt(this.strength*damageMultiply)))
        {
            foreach(Vector3Int vec in attackRange)
            {
                EffectHelper.printEffect(effectName, (transform.position + vec), DirectionChange.dirToRotation(vec));
                SoundEffecter.playSFX(sfxName);
            }
        }
        else
        {
            SoundEffecter.playSFX("Miss");
        }
    }

    /// <summary>
    /// 원하는 곳에다 공격을 요청하는 함수. 보통 artifact내부에서 쓰임.
    /// effectRange로 지정한 곳에서만 effect가 나옴.
    /// 만약, effect를 attackRange 전체에서 발동시키고 싶다면 overloading된 함수를 사용할 것
    /// 반드시 effectRange와 effectDir의 갯수와 순서를 잘 맞출 것.
    /// </summary>
    /// <param name="attackRange">어디를 때릴건지(보통 player의 방향에 따라서 달라질거고,)</param>
    /// <param name="effectRange">effect가 어디서만 재생되게 할 것인지</param>
    /// /// <param name="effectDir">effect의 방향은? (1,0) (0,1) (-1,0) (0, -1)으로</param>
    /// <param name="damageMultiply">공격계수. 소수점을 이용함. [player.strength*계수] 로 데미지가 결정</param>
    /// <param name="effectName">공격에 사용되는 effect이름. Effect 폴더 내의 이름을 써주면 된다.</param>
    /// <param name="sfxName">공격의 효과음 이름. SoundEffects 폴더 내의 이름을 써주면 된다.</param>
    public void attack(List<Vector3Int> attackRange, List<Vector3Int> effectRange, List<Vector3Int> effectDir, float damageMultiply, string effectName, string sfxName)
    {
        if (spawnManager.AttackCharacter(transform.position, attackRange,
            Mathf.RoundToInt(this.strength * damageMultiply)))
        {
            for (int i = 0; i< effectRange.Count; i++)
            {
                EffectHelper.printEffect(effectName, (transform.position + effectRange[i]), DirectionChange.dirToRotation(effectDir[i]));
            }
            SoundEffecter.playSFX(sfxName);
        }
        else
        {
            SoundEffecter.playSFX("Miss");
        }
    }


    //2022_02_13 artifact제거용도 함수 4개
    //2022_02_14 4개의 함수 수정
    /// <summary>
    /// 보통 유물들 내부에서 사용. 현재 선택된 칸의 유물을 제거하고 맨손으로 변경
    /// 이 함수는 atDestroy를 따로 불러주지 않는다.
    /// </summary>
    public void deleteArtifact()
    {
        //원래는 destroy를 했으나, dataloss때문에 destroyimmediate을 써야한다고 경고문구가 뜸.
        //Unity의 garbage collecting을 믿기로 했음. 자주 불리는 함수도 아니고.
        //Artifact temp = this.artifacts[curArtifact];
        this.artifacts[curArtifact] = Resources.Load<GameObject>("Artifacts/Artifact__Hand").GetComponent<Artifact>();
        uiManager.changeArtifact((curArtifact+1), this.artifacts[curArtifact].getRealArtifactName());
        //Destroy(temp);
    }

    /// <summary>
    /// 특정 slot의 artifact를 제거하고 맨손으로 변경
    /// 이 함수는 atDestroy를 불러주고 제거한다.
    /// </summary>
    /// <param name="slot">교체하고 싶은 slot. 1, 2, 3으로 결정</param>
    public void deleteArtifact(int slot)
    {
        //원래는 과거의 것을 destroy를 했으나, dataloss때문에 destroyimmediate을 써야한다고 경고문구가 뜸.
        //Unity의 garbage collecting을 믿기로 했음. 자주 불리는 함수도 아니고.
        //Artifact temp = this.artifacts[slot - 1];
        this.artifacts[slot - 1].atDestroy(this);
        this.artifacts[slot - 1] = Resources.Load<GameObject>("Artifacts/Artifact__Hand").GetComponent<Artifact>();
        uiManager.changeArtifact((slot), this.artifacts[slot - 1].getRealArtifactName());
        //Destroy(temp);
    }

    /// <summary>
    /// 보통 유물들 내부에서 사용. 현재 선택된 칸의 유물을 제거하고 원하는 유물로 교체
    /// 이 함수는 atDestory를 불러주지 않는다.
    /// </summary>
    /// <param name="artifactName">교체하고 싶은 artifact의 real name(Artifact__???)</param>
    public void deleteArtifact(string artifactName)
    {//원래는 과거의 것을 destroy를 했으나, dataloss때문에 destroyimmediate을 써야한다고 경고문구가 뜸.
        //Unity의 garbage collecting을 믿기로 했음. 자주 불리는 함수도 아니고.
        //Artifact temp = this.artifacts[slot - 1];
        //Artifact temp = this.artifacts[curArtifact];
        this.artifacts[curArtifact] = Resources.Load<GameObject>("Artifacts/" + artifactName).GetComponent<Artifact>();
        //22_02_25 atEarn추가
        this.artifacts[curArtifact].atEarn(this);
        uiManager.changeArtifact((curArtifact + 1), this.artifacts[curArtifact].getRealArtifactName());
        //Destroy(temp);
    }

    /// <summary>
    /// 원하는 칸에 위치한 유물을 원하는 유물로 교체
    /// 이 함수는 atDestroy를 불러주고 제거한다.
    /// </summary>
    /// <param name="artifactName">교체하고 싶은 artifact의 real name(Artifact__???)</param>
    /// <param name="slot">교체하고 싶은 slot. 1, 2, 3으로 결정</param>
    public void deleteArtifact(string artifactName, int slot)
    {
        //원래는 과거의 것을 destroy를 했으나, dataloss때문에 destroyimmediate을 써야한다고 경고문구가 뜸.
        //Unity의 garbage collecting을 믿기로 했음. 자주 불리는 함수도 아니고.
        //Artifact temp = this.artifacts[slot - 1];
        //Artifact temp = this.artifacts[slot-1];
        this.artifacts[slot - 1].atDestroy(this);
        this.artifacts[slot - 1] = Resources.Load<GameObject>("Artifacts/" + artifactName).GetComponent<Artifact>();
        //22_02_25 atEarn추가
        this.artifacts[slot - 1].atEarn(this);
        uiManager.changeArtifact((slot), this.artifacts[slot - 1].getRealArtifactName());
        //Destroy(temp);
    }


    public void move(Vector2 direction)
    {
        if (uiManager.getMoveFlag())
        {
            if (spawnManager.MoveCharacter(transform.position, direction))
            {
                Vector3 targetLoc = new Vector3(direction.x,direction.y, 0) + transform.position;
                this.transform.position = targetLoc;
                setDirection(direction, this.transform.position);
                SoundEffecter.playSFX("Move");
            }
        }
    }

    //2022_02_11 - player는 hp바가 따로 있기 때문에 override하였음.
    public override bool hpDamage(int damage)
    {
        if (this.hp <= damage)
        {
            this.hp = 0;
            uiManager.changeHpBar();
            GameObject.FindWithTag("LevelManager").GetComponent<LevelManager>().gameOver();
            Destroy(gameObject);
            return true;
        }
        this.hp -= damage;
        uiManager.changeHpBar();

        return false;
    }
    //2022_02_11 - 순수 테스트용.
    public void testhpDamage()
    {
        if (hp < 0) { hp = 1; }
        else { this.hp--; }
        uiManager.changeHpBar();
    }
    public override void hpHeal(int heal)
    {
        //2022_02_13 놀랍게도 오타가 있어서 제대로 적용되지 않았었습니다. 수정 완료
        if (this.maxHp <= (this.hp + heal))
        {
            this.hp = maxHp;
        }
        else { this.hp += heal; }
        //2022_02_11 - hp바 용
        uiManager.changeHpBar();
    }

}
