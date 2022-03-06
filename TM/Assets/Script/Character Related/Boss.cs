using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Boss : CharacterBase
{
    // Start is called before the first frame update
    void Start()
    {
        this.hpBackground = GameObject.FindWithTag("BossHpBackground");
        this.hpForeground = GameObject.FindWithTag("BossHpForeground").GetComponent<Image>();
        hpBackground.SetActive(true);
        hpForeground.fillAmount = 1f;

        directionArrow.SetActive(true);
        rotateArrow();
        directionArrow.transform.position = this.transform.position + (new Vector3(0, -0.4f));

    }

    public override void setDirection(Directions dir, Vector3 loc)
    {
        this.direction = dir;

        if (this.direction == Directions.S || this.direction == Directions.W)
        {
            gameObject.GetComponent<SpriteRenderer>().flipX = false;
        }
        else { gameObject.GetComponent<SpriteRenderer>().flipX = true; }

        if (dirHud) { directionArrow.transform.position = loc + (new Vector3(0, -0.4f)); rotateArrow(); }
    }

    public override bool hpDamage(int damage)
    {
        if (this.hp <= damage)
        {
            GameObject.FindWithTag("AudioManager").GetComponent<AudioSource>().volume = 0.3f;
            SoundEffecter.playSFX("AAA");
            Time.timeScale = 0.3f;
            StartCoroutine(WaitForScene());
            //Destroy(gameObject);
            //return true;
        }
        this.hp -= damage;
        if (hpHud) { hpForeground.fillAmount = (float)hp / (float)maxHp; }
        return false;
    }

    private IEnumerator WaitForScene()
    {
        float sec = 1f;
        while (sec >= 0f)
        {
            sec -= Time.deltaTime;

            yield return new WaitForFixedUpdate();
        }
        GameObject.FindWithTag("LevelManager").GetComponent<SaveManager>().saving.stageFlag = false;
        GameObject.FindWithTag("LevelManager").GetComponent<SaveManager>().savePlayer(true, true);
        GameObject.FindWithTag("LevelManager").GetComponent<LevelManager>().LoadScene("Stage1_Clear");

    }

}
