using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    /*
     * The tilemap which contains the ground tiles
     * */
    [SerializeField]
    private Tilemap map;

    /*
     * The duration time of the game in minute
     * */
    public float timerTime = 15;

    /*
     * The time scaling factor
     * */
    [Range(1f, 20f)]
    public float timeScale = 1f;

    /*
     * The instance of corruption manager
     * */
    public CorruptionManager corruptionManager;

    /*
     * The event called when the game has ended
     * */
    private UnityEvent EndGameEvent;

    /*
     * Initialize the end game event and pass values to the corruption manager
     * */
    void Start()
    {
        EndGameEvent = new UnityEvent();
        EndGameEvent.AddListener(End);

        corruptionManager.EndGameEvent = EndGameEvent;
        corruptionManager.maxTimer = timerTime;
    }

    // Update is called once per frame
    void Update()
    {
    }

    /*
     * Get the score of the player and the corruption (The number of tiles of both)
     * */
    public (int,int) GetScore()
    {
        int scorePlayer = 0;
        int scoreCorruption = 0;
        foreach (Transform child in map.transform)
        {
            switch (child.gameObject.GetComponent<TileDataContainer>().type)
            {
                case 0:
                    scorePlayer++;
                    break;
                case 1:
                    scoreCorruption++;
                    break;
                default:
                    continue;
            }
        }

        return (scorePlayer, scoreCorruption);
    }

    /*
     * Callback method when the end game event is triggered
     * Print the scores and pause the game
     * */
    public void End()
    {
        (int, int) scores = GetScore();
        Debug.Log(scores.Item1);
        Debug.Log(scores.Item2);
        Debug.Log("END !!!");
        Time.timeScale = timeScale;
    }

    private void OnValidate()
    {
        Time.timeScale = timeScale;
    }
}
