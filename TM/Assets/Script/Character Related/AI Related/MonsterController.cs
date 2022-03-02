using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterController : MonoBehaviour
{
    //22_02_25
    [SerializeField]
    protected Vector3Int loc;

    //22_02_25
    public Vector3Int getLoc()
    {
        return this.loc;
    }
    //22_02_25
    public void setLocation(int x, int y)
    {
        this.loc.x = x;
        this.loc.y = y;
    }

    [SerializeField]
    protected ScriptableLocation playerLoc;

    [SerializeField]
    protected int alertRange;

    [SerializeField]
    protected float beforeCast;

    [SerializeField]
    protected bool AttackFlag = true; //공격 쿨타임용
    [SerializeField]
    protected bool ActFlag = true; //하나를 하면 하나를 못하게 하는 flag
    [SerializeField]
    protected bool MoveFlag = true;

    protected float attspd;
    protected float movspd;

    private void Awake()
    {
        this.attspd = gameObject.GetComponent<CharacterBase>().getAttackSpd();
        this.movspd = gameObject.GetComponent<CharacterBase>().getMoveSpd();
    }

    protected int calcDist()
    {
        return (Mathf.Abs(playerLoc.getX() - this.loc.x) + Mathf.Abs(playerLoc.getY() - this.loc.y));
    }

    protected void moveRandom()
    {
        if(MoveFlag)
        {
            Directions where = (Directions)Random.Range(0, 5);
            Directions dire = GameObject.FindWithTag("TileManager").GetComponent<TileManager>().moveObject(this.loc, where);
            if (dire != Directions.O)
            {
                transform.position = transform.position + DirectionChange.dirToNormalVec(dire);
                transform.GetComponent<CharacterBase>().setDirection(dire, transform.position);
            }

            StartCoroutine(moveCoolTime());
        }

    }

    protected void moveToPlayer()
    {
        if(MoveFlag)
        {
            int tx = playerLoc.getX() - this.loc.x;
            int ty = playerLoc.getY() - this.loc.y;
            List<Directions> moveDir = new List<Directions>();
            Directions dire;

            if (Mathf.Abs(tx) >= Mathf.Abs(ty))
            {
                if (tx >= 0 && ty >= 0) { moveDir.Add(Directions.E); moveDir.Add(Directions.N); moveDir.Add(Directions.S); moveDir.Add(Directions.W); }
                else if (tx >= 0 && ty < 0) { moveDir.Add(Directions.E); moveDir.Add(Directions.S); moveDir.Add(Directions.N); moveDir.Add(Directions.W); }
                else if (tx < 0 && ty >= 0) { moveDir.Add(Directions.W); moveDir.Add(Directions.N); moveDir.Add(Directions.S); moveDir.Add(Directions.E); }
                else { moveDir.Add(Directions.W); moveDir.Add(Directions.N); moveDir.Add(Directions.S); moveDir.Add(Directions.E); }
            }
            else
            {
                if (tx >= 0 && ty >= 0) { moveDir.Add(Directions.N); moveDir.Add(Directions.E); moveDir.Add(Directions.W); moveDir.Add(Directions.S); }
                else if (tx >= 0 && ty < 0) { moveDir.Add(Directions.S); moveDir.Add(Directions.E); moveDir.Add(Directions.W); moveDir.Add(Directions.N); }
                else if (tx < 0 && ty >= 0) { moveDir.Add(Directions.N); moveDir.Add(Directions.W); moveDir.Add(Directions.E); moveDir.Add(Directions.S); }
                else { moveDir.Add(Directions.S); moveDir.Add(Directions.W); moveDir.Add(Directions.E); moveDir.Add(Directions.N); }
            }

            dire = GameObject.FindWithTag("TileManager").GetComponent<TileManager>().moveSequence(this.loc, moveDir);
            if (dire != Directions.O)
            {
                transform.position += DirectionChange.dirToNormalVec(dire);
                transform.GetComponent<CharacterBase>().setDirection(dire, transform.position);
            }
            StartCoroutine(moveCoolTime());
        }

        
    }

    protected void awayFromPlayer()
    {
        if (MoveFlag)
        {
            int tx = playerLoc.getX() - this.loc.x;
            int ty = playerLoc.getY() - this.loc.y;
            List<Directions> moveDir = new List<Directions>();
            Directions dire;

            if (Mathf.Abs(tx) >= Mathf.Abs(ty))
            {
                if (tx >= 0 && ty >= 0) { moveDir.Add(Directions.W); moveDir.Add(Directions.S); moveDir.Add(Directions.N); moveDir.Add(Directions.E); }
                else if (tx >= 0 && ty < 0) { moveDir.Add(Directions.W); moveDir.Add(Directions.N); moveDir.Add(Directions.S); moveDir.Add(Directions.E); }
                else if (tx < 0 && ty >= 0) { moveDir.Add(Directions.E); moveDir.Add(Directions.S); moveDir.Add(Directions.N); moveDir.Add(Directions.W); }
                else { moveDir.Add(Directions.E); moveDir.Add(Directions.N); moveDir.Add(Directions.S); moveDir.Add(Directions.W); }
            }
            else
            {
                if (tx >= 0 && ty >= 0) { moveDir.Add(Directions.S); moveDir.Add(Directions.W); moveDir.Add(Directions.E); moveDir.Add(Directions.N); }
                else if (tx >= 0 && ty < 0) { moveDir.Add(Directions.N); moveDir.Add(Directions.W); moveDir.Add(Directions.E); moveDir.Add(Directions.S); }
                else if (tx < 0 && ty >= 0) { moveDir.Add(Directions.S); moveDir.Add(Directions.E); moveDir.Add(Directions.W); moveDir.Add(Directions.N); }
                else { moveDir.Add(Directions.N); moveDir.Add(Directions.E); moveDir.Add(Directions.W); moveDir.Add(Directions.S); }
            }

            dire = GameObject.FindWithTag("TileManager").GetComponent<TileManager>().moveSequence(this.loc, moveDir);
            if (dire != Directions.O)
            {
                transform.position = transform.position + DirectionChange.dirToNormalVec(dire);

                transform.GetComponent<CharacterBase>().setDirection(dire, transform.position);
            }

            StartCoroutine(moveCoolTime());
        }

    }

    protected IEnumerator moveCoolTime()
    {
        this.MoveFlag = false;
        float sec = (4.0f / movspd);

        while (sec >= 0)
        {
            sec -= Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }
        this.MoveFlag = true;
    }

    protected IEnumerator attackCoolTime()
    {
        this.AttackFlag = false;
        float sec = 5.0f / attspd;

        while (sec >= 0)
        {
            sec -= Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }
        this.AttackFlag = true;
    }

    protected IEnumerator warnArea(Vector3 loc, List<Vector3Int> range, float beforeCast)
    {
        foreach(Vector3Int p in range)
        {
            Destroy(GameObject.Instantiate(
                Resources.Load("Effect/Warning", typeof(GameObject)) as GameObject,
                (loc + new Vector3(0, 0, -0.1f) + p), Quaternion.Euler(new Vector3(0, 0, 0)), transform), beforeCast);
        }
        yield return null;
    }
}
