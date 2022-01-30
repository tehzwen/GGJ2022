using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerController : MonoBehaviour
{
    public static TimerController instance;

    public TimeSpan timePlaying;
    public Time timeofDay;

    private bool timerGoing;
    private float elapsedTime;

    [Tooltip("Reference to Text object on UI for displaying of time")]
    [SerializeField] public Text timeCounter;

    [Tooltip("Magnitude of time sped up (ie. 60 implies 1 second equates to 1 hour in game)")]
    [SerializeField] public float daySpeed = 60f; //60f means 1 second == 1 hour in game

    private void Awake()
	{
        instance = this;
	}
	void Start()
    {
        timeCounter.text = "Time: 00:00.00";
        timerGoing = false;
    }

    public void BeginTimer() //call via TimerController.instance.BeginTimer() in gameController
	{
        timerGoing = true;
        elapsedTime = 0f;
        StartCoroutine(UpdateTimer());
	}

    public void EndTimer()
	{
        timerGoing = false;
	}

    private IEnumerator UpdateTimer()
	{
		while (timerGoing)
		{
            elapsedTime += Time.deltaTime * daySpeed;

            timePlaying = TimeSpan.FromSeconds(elapsedTime);
            string timePlayingStr = "Time: " + timePlaying.ToString(@"dd\.hh\:mm\:ss");
            timeCounter.text = timePlayingStr;
            yield return null;
		}
	}

    public TimeSpan GetTimePlaying()
	{
        return this.timePlaying;
	}

    public float GetHour()
	{
        return elapsedTime / 3600;
	}

    public float GetMinute()
	{
        return elapsedTime / 60;
	}

}
