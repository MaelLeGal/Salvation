using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{

    [SerializeField]
    private Tilemap map;

    public float timerTime = 15;

    [Range(1f, 20f)]
    public float timeScale = 1f;

    private UnityEvent EndGameEvent;

    // Start is called before the first frame update
    void Start()
    {
        EndGameEvent = new UnityEvent();
        EndGameEvent.AddListener(End);

        GameObject.Find("CorruptionManager").GetComponent<CorruptionManager>().EndGameEvent = EndGameEvent;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public (int,int) GetScore()
    {
        int scorePlayer = 0;
        int scoreCorruption = 0;
        foreach (Transform child in map.transform)
        {
            switch (child.gameObject.name)
            {
                case "Grass":
                    scorePlayer++;
                    break;
                case "Dry_Ground":
                    scoreCorruption++;
                    break;
                default:
                    continue;
            }
        }

        return (scorePlayer, scoreCorruption);
    }

    public void End()
    {
        (int, int) scores = GetScore();
        Debug.Log(scores.Item1);
        Debug.Log(scores.Item2);
        Debug.Log("END !!!");
        timeScale = 0;
        Time.timeScale = timeScale;
    }

    private void OnValidate()
    {
        Time.timeScale = timeScale;
    }
}
