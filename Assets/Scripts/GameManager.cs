using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Events;
using TMPro;

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
     * The text diplaying the timer
     * */
    public TMP_Text timerText;

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

    private bool end = false;

    public GameObject endPopUp;
    public GameObject playerScoreText;
    public GameObject corruptionScoreText;

    /*
     * Initialize the end game event and pass values to the corruption manager
     * */
    void Start()
    {
        EndGameEvent = new UnityEvent();
        EndGameEvent.AddListener(End);

        corruptionManager.EndGameEvent = EndGameEvent;
        corruptionManager.maxTimer = timerTime;

        timerText.text = timerTime+":00";
        timerTime *= 60;
    }

    // Update is called once per frame
    void Update()
    {
        if (timerTime <= 0 && !end)
        {
            EndGameEvent.Invoke();
            end = true;
        }
        else
        {
            int minutes = Mathf.FloorToInt(timerTime / 60);
            int secondes = Mathf.FloorToInt(timerTime % 60);
            timerTime -= Time.deltaTime;

            timerText.text = secondes < 10 ? minutes + ":0" + secondes : minutes + ":" + secondes;
        }
        
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
        Time.timeScale = 0;

        endPopUp.SetActive(true);

        playerScoreText.GetComponent<TextMeshProUGUI>().text += "\n" + scores.Item1;
        corruptionScoreText.GetComponent<TextMeshProUGUI>().text += "\n"+scores.Item2;

    }

    private void OnValidate()
    {
        Time.timeScale = timeScale;
    }
}
