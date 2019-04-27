using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NexusController : ShootingSystem
{
    // Public game objects
    public GameObject minion;
    public GameObject minionsParent;
    public GameObject champion;
    public GameObject championsParent;
    public GameObject mainCamera;
    public Transform spawnPoint;
    public Transform[] whiteTargets;
    public Transform[] blackTargets;

    public GameObject minionHealthBar;
    public GameObject championHealthBar;

    public float minionSpawnRate = 20f;

    private float minionSpawnTimer = 0f;

    protected override void Start()
    {
        base.Start();

        if (isWhite)
        {
            ChampionSpawn();
        }
    }

    protected override void Update()
    {
        base.Update();

        minionSpawnTimer += Time.deltaTime;
        if (minionSpawnTimer >= minionSpawnRate)
        {
            for (int i = 0; i < 3; i++)
            {
                MinionsSpawn();
            }
            minionSpawnTimer = 0f;
        }
    }

    private void MinionsSpawn()
    {
        GameObject newMinion = Instantiate(minion, spawnPoint.position,
                    Quaternion.Euler(spawnPoint.transform.forward)) as GameObject;
        newMinion.transform.SetParent(minionsParent.transform);

        if (isWhite) // for white minions
        {
            newMinion.GetComponent<SpriteRenderer>().color = Color.white;
            newMinion.GetComponent<CharactersController>().isWhite = true;
            MinionController setNew = newMinion.GetComponent<MinionController>();
            setNew.iconMap = whiteTargets;
            setNew.healthBar = Instantiate(minionHealthBar, newMinion.transform);
        }
        else // for black minions
        {
            Vector3 tempScale = new Vector3(-newMinion.transform.localScale.x,
                newMinion.transform.localScale.y, newMinion.transform.localScale.z);
            newMinion.transform.localScale = tempScale;
            newMinion.GetComponent<SpriteRenderer>().color = Color.black;
            newMinion.GetComponent<CharactersController>().isWhite = false;
            MinionController setNew = newMinion.GetComponent<MinionController>();
            setNew.iconMap = blackTargets;
            setNew.healthBar = Instantiate(minionHealthBar, newMinion.transform);
        }
    }

    public void ChampionSpawn()
    {
        GameObject newChampion = Instantiate(champion, spawnPoint.position,
                   Quaternion.Euler(spawnPoint.transform.forward)) as GameObject;
        //Debug.Log(Quaternion.Euler(spawnPoint.transform.forward));
        newChampion.transform.SetParent(championsParent.transform);

        if (isWhite) // for white champions
        {
            newChampion.GetComponent<SpriteRenderer>().color = Color.white;
            newChampion.GetComponent<CharactersController>().isWhite = true;
            ChampionController setNew = newChampion.GetComponent<ChampionController>();
            setNew.healthBar = Instantiate(championHealthBar, newChampion.transform);
            setNew.home = this;

            CameraController main = mainCamera.GetComponent<CameraController>();
            main.champion = newChampion;
        }
        else // for black champions
        {
            Vector3 tempScale = new Vector3(-newChampion.transform.localScale.x,
                newChampion.transform.localScale.y, newChampion.transform.localScale.z);
            newChampion.transform.localScale = tempScale;
            newChampion.GetComponent<SpriteRenderer>().color = Color.black;
            newChampion.GetComponent<CharactersController>().isWhite = false;
            ChampionController setNew = newChampion.GetComponent<ChampionController>();
            setNew.healthBar = Instantiate(championHealthBar, newChampion.transform);
        }
    }
}
