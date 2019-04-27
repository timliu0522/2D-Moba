using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    public GameObject champion;
    private List<ChampionController> whiteChampions = new List<ChampionController>();
    private List<ChampionController> blackChampions = new List<ChampionController>();

    private GameObject timer;
    public GameObject scoreBoard;

    void Start()
    {
        GetComponent<Camera>().orthographicSize = 3f;

        timer = GameObject.Find("Timer");

        ChampionController[] champions = FindObjectsOfType<ChampionController>();
        for (int i = 0; i < champions.Length; i++)
        {
            if (champions[i].isWhite)
            {
                whiteChampions.Add(champions[i]);
            }
            else
            {
                blackChampions.Add(champions[i]);
            }
        }
    }

    void Update()
    {
        if (champion != null)
        {
            transform.position = new Vector3(champion.transform.position.x, champion.transform.position.y, -10);
        }

        timer.GetComponent<Text>().text = "Time: " + (int)(Time.time / 60f) + ":" + (int)(Time.time % 60f);

        int whiteTotalKills = 0;
        int blackTotalKills = 0;
        for (int i = 0; i < whiteChampions.Count; i++)
        {
            whiteTotalKills += whiteChampions[i].killCount;
        }
        for (int i = 0; i < blackChampions.Count; i++)
        {
            blackTotalKills += blackChampions[i].killCount;
        }

        scoreBoard.GetComponent<Text>().text = whiteTotalKills + " / " + blackTotalKills;
    }
}
