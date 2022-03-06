using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : CharacterBase
{
    [SerializeField] private int gold;
    //2022_02_13 artifacts�� ���������� artifact Ŭ������ ������ �� �ֵ��� ó��
    [SerializeField]
    private Artifact[] artifacts = new Artifact[3];
    [SerializeField] private SpawnManager spawnManager;

    //2022_02_11 - UIManager�� UIManager�� �ϴϱ� ����� �ν��� �ȵǴ� ������ �־, uiManager�� �����Ͽ����ϴ�.
    [SerializeField] private UIManager uiManager;

    //2022_02_13 ������ artifact�� Ȯ���ϱ� ���� curArtifact�� �߰�
    [SerializeField] private int curArtifact;

    public int getGold() { return this.gold; }
    //22_03_01
    public void changeGold(int amount)
    {
        if (this.gold+amount>=0) this.gold += amount;
        if(this.gold <= 0) this.gold = 0;
        uiManager.changeGold(this.gold);
    }

    //2022_02_13 getCurArtifact �߰�
    public int getCurArtifact() { return this.curArtifact; }
    public void setCurArtifact(int num) { this.curArtifact = num; }
    //2022_02_13 artifacts�� ���������� artifact Ŭ������ ������ �� �ֵ��� ó��
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
        //2022_02_13 CurArtifact���� �ʱ�ȭ ó��.
        this.curArtifact = 0;
        this.characterName = player.characterName;
        this.moveSpeed = player.moveSpeed;
        this.moveDistance = player.moveDistance;
        this.strength = player.strength;
        this.attackSpeed = player.attackSpeed;
        this.maxHp = player.maxHp;
        this.hp = player.hp;
        this.gold = player.gold;
        //2022_02_13 - ���� �ҷ�����
        this.artifacts[0] = Resources.Load<GameObject>("Artifacts/" + player.artifact1).GetComponent<Artifact>();
        this.artifacts[1] = Resources.Load<GameObject>("Artifacts/" + player.artifact2).GetComponent<Artifact>();
        this.artifacts[2] = Resources.Load<GameObject>("Artifacts/" + player.artifact3).GetComponent<Artifact>();
    }

    //2022_02_13 ������ ����� ����ϴ� �Լ��� useArtifact. ���� �� �Լ��� UIManager���� �Ҹ��� �˴ϴ�.
    public void useArtifact() 
    {
        if (uiManager.getAttackFlag())
        {
            this.artifacts[curArtifact].use(this);
        }

    }

    /// <summary>
    /// ���ϴ� ������ ������ ��û�ϴ� �Լ�. ���� artifact���ο��� ����.
    /// attackRange�� ������ ���� effect�� ��� ����.
    /// ����, effect�� Ư���� �������� �ߵ���Ű�� �ʹٸ� overloading�� �Լ��� ����� ��
    /// </summary>
    /// <param name="attackRange">��� ��������(���� player�� ���⿡ ���� �޶����Ű�,)</param>
    /// <param name="damageMultiply">���ݰ��. �Ҽ����� �̿���. [player.strength*���] �� �������� ����</param>
    /// <param name="effectName">���ݿ� ���Ǵ� effect�̸�. Effect ���� ���� �̸��� ���ָ� �ȴ�.</param>
    /// <param name="sfxName">������ ȿ���� �̸�. SoundEffects ���� ���� �̸��� ���ָ� �ȴ�.</param>
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
    /// ���ϴ� ������ ������ ��û�ϴ� �Լ�. ���� artifact���ο��� ����.
    /// effectRange�� ������ �������� effect�� ����.
    /// ����, effect�� attackRange ��ü���� �ߵ���Ű�� �ʹٸ� overloading�� �Լ��� ����� ��
    /// �ݵ�� effectRange�� effectDir�� ������ ������ �� ���� ��.
    /// </summary>
    /// <param name="attackRange">��� ��������(���� player�� ���⿡ ���� �޶����Ű�,)</param>
    /// <param name="effectRange">effect�� ��𼭸� ����ǰ� �� ������</param>
    /// /// <param name="effectDir">effect�� ������? (1,0) (0,1) (-1,0) (0, -1)����</param>
    /// <param name="damageMultiply">���ݰ��. �Ҽ����� �̿���. [player.strength*���] �� �������� ����</param>
    /// <param name="effectName">���ݿ� ���Ǵ� effect�̸�. Effect ���� ���� �̸��� ���ָ� �ȴ�.</param>
    /// <param name="sfxName">������ ȿ���� �̸�. SoundEffects ���� ���� �̸��� ���ָ� �ȴ�.</param>
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


    //2022_02_13 artifact���ſ뵵 �Լ� 4��
    //2022_02_14 4���� �Լ� ����
    /// <summary>
    /// ���� ������ ���ο��� ���. ���� ���õ� ĭ�� ������ �����ϰ� �Ǽ����� ����
    /// �� �Լ��� atDestroy�� ���� �ҷ����� �ʴ´�.
    /// </summary>
    public void deleteArtifact()
    {
        //������ destroy�� ������, dataloss������ destroyimmediate�� ����Ѵٰ� ������� ��.
        //Unity�� garbage collecting�� �ϱ�� ����. ���� �Ҹ��� �Լ��� �ƴϰ�.
        //Artifact temp = this.artifacts[curArtifact];
        this.artifacts[curArtifact] = Resources.Load<GameObject>("Artifacts/Artifact__Hand").GetComponent<Artifact>();
        uiManager.changeArtifact((curArtifact+1), this.artifacts[curArtifact].getRealArtifactName());
        //Destroy(temp);
    }

    /// <summary>
    /// Ư�� slot�� artifact�� �����ϰ� �Ǽ����� ����
    /// �� �Լ��� atDestroy�� �ҷ��ְ� �����Ѵ�.
    /// </summary>
    /// <param name="slot">��ü�ϰ� ���� slot. 1, 2, 3���� ����</param>
    public void deleteArtifact(int slot)
    {
        //������ ������ ���� destroy�� ������, dataloss������ destroyimmediate�� ����Ѵٰ� ������� ��.
        //Unity�� garbage collecting�� �ϱ�� ����. ���� �Ҹ��� �Լ��� �ƴϰ�.
        //Artifact temp = this.artifacts[slot - 1];
        this.artifacts[slot - 1].atDestroy(this);
        this.artifacts[slot - 1] = Resources.Load<GameObject>("Artifacts/Artifact__Hand").GetComponent<Artifact>();
        uiManager.changeArtifact((slot), this.artifacts[slot - 1].getRealArtifactName());
        //Destroy(temp);
    }

    /// <summary>
    /// ���� ������ ���ο��� ���. ���� ���õ� ĭ�� ������ �����ϰ� ���ϴ� ������ ��ü
    /// �� �Լ��� atDestory�� �ҷ����� �ʴ´�.
    /// </summary>
    /// <param name="artifactName">��ü�ϰ� ���� artifact�� real name(Artifact__???)</param>
    public void deleteArtifact(string artifactName)
    {//������ ������ ���� destroy�� ������, dataloss������ destroyimmediate�� ����Ѵٰ� ������� ��.
        //Unity�� garbage collecting�� �ϱ�� ����. ���� �Ҹ��� �Լ��� �ƴϰ�.
        //Artifact temp = this.artifacts[slot - 1];
        //Artifact temp = this.artifacts[curArtifact];
        this.artifacts[curArtifact] = Resources.Load<GameObject>("Artifacts/" + artifactName).GetComponent<Artifact>();
        //22_02_25 atEarn�߰�
        this.artifacts[curArtifact].atEarn(this);
        uiManager.changeArtifact((curArtifact + 1), this.artifacts[curArtifact].getRealArtifactName());
        //Destroy(temp);
    }

    /// <summary>
    /// ���ϴ� ĭ�� ��ġ�� ������ ���ϴ� ������ ��ü
    /// �� �Լ��� atDestroy�� �ҷ��ְ� �����Ѵ�.
    /// </summary>
    /// <param name="artifactName">��ü�ϰ� ���� artifact�� real name(Artifact__???)</param>
    /// <param name="slot">��ü�ϰ� ���� slot. 1, 2, 3���� ����</param>
    public void deleteArtifact(string artifactName, int slot)
    {
        //������ ������ ���� destroy�� ������, dataloss������ destroyimmediate�� ����Ѵٰ� ������� ��.
        //Unity�� garbage collecting�� �ϱ�� ����. ���� �Ҹ��� �Լ��� �ƴϰ�.
        //Artifact temp = this.artifacts[slot - 1];
        //Artifact temp = this.artifacts[slot-1];
        this.artifacts[slot - 1].atDestroy(this);
        this.artifacts[slot - 1] = Resources.Load<GameObject>("Artifacts/" + artifactName).GetComponent<Artifact>();
        //22_02_25 atEarn�߰�
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

    //2022_02_11 - player�� hp�ٰ� ���� �ֱ� ������ override�Ͽ���.
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
    //2022_02_11 - ���� �׽�Ʈ��.
    public void testhpDamage()
    {
        if (hp < 0) { hp = 1; }
        else { this.hp--; }
        uiManager.changeHpBar();
    }
    public override void hpHeal(int heal)
    {
        //2022_02_13 ����Ե� ��Ÿ�� �־ ����� ������� �ʾҾ����ϴ�. ���� �Ϸ�
        if (this.maxHp <= (this.hp + heal))
        {
            this.hp = maxHp;
        }
        else { this.hp += heal; }
        //2022_02_11 - hp�� ��
        uiManager.changeHpBar();
    }

}
