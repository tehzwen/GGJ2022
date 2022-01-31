using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameState State;
    public static event Action<GameState> OnGameStateChanged;
    public TimerController timerController;
    public Light2D GlobalLight;
    public GameObject player;
    public float dayCycleStart = 5f;
    public float nightCycleStart = 19f;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        UpdateGameState(GameState.Start);
    }

    public void UpdateGameState(GameState newState)
    {
        State = newState;
        switch (newState)
        {
            case GameState.Start:
                InitializeGame();
                break;
            case GameState.Menu:
                MenuHandleSelection();
                break;
            case GameState.DayCycle:
                _OnNightEnd();
                break;
            case GameState.NightCycle:
                _OnNightFall();
                break;
            case GameState.EndGame:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }

        OnGameStateChanged?.Invoke(newState);
    }

    private void _OnNightFall()
    {
        Player.Controller playerScript = player.GetComponent<Player.Controller>();
        playerScript.OnNightFall();

        Extensions.Finder.NightAffectedObject[] nightObjects = Extensions.Finder.GetNightAffectedObjects();

        for (int i = 0; i < nightObjects.Length; i++)
        {
            nightObjects[i].NightScript.OnNightFall();
        }
    }

    private void _OnNightEnd()
    {
        Player.Controller playerScript = player.GetComponent<Player.Controller>();
        playerScript.OnNightEnd();

        Extensions.Finder.NightAffectedObject[] nightObjects = Extensions.Finder.GetNightAffectedObjects();

        for (int i = 0; i < nightObjects.Length; i++)
        {
            nightObjects[i].NightScript.OnNightEnd();
        }
    }

    private void InitializeGame()
    {
        timerController.BeginTimer();
        UpdateGameState(GameState.DayCycle);
    }

    private void MenuHandleSelection()
    {

    }

    void Update()
    {
        float currHour = timerController.GetHour();
        // Debug.Log(string.Format("Hour: {0}", currHour));
        // Debug.Log("Current day: " + timerController.GetDay());
        // Debug.Log("Cycle: " + State);
        if (State == GameState.DayCycle && currHour > nightCycleStart)
            UpdateGameState(GameState.NightCycle);

        if (currHour > dayCycleStart && currHour < nightCycleStart)
        {
            if (State == GameState.NightCycle)
            {
                UpdateGameState(GameState.DayCycle);
            }
            else
            {
                float darknessFactor = Mathf.Abs(12.0f - currHour) / 12.0f;

                if (currHour > 12.0f)
                {
                    GlobalLight.intensity = ((12.0f + (12 - (currHour - 12.0f))) / 24.0f) - darknessFactor;
                }
                else
                {
                    GlobalLight.intensity = ((currHour + 12.0f) / 24.0f) - darknessFactor;
                }
            }
        }
    }

    GameState GetGameState()
    {
        return this.State;
    }

    public struct TimeRepr
    {
        public float Minute;
        public float Hour;

        public TimeRepr(TimerController timeController)
        {
            Hour = timeController.GetHour() % 24;
            Minute = timeController.GetMinute() % 60;
        }
    }

    public TimeRepr GetGameTimeValue()
    {
        return new TimeRepr(timerController);
    }

    public String GetGameTime()
    {
        TimeRepr time = GetGameTimeValue();
        float hour = time.Hour;
        float minute = time.Minute;

        return hour.ToString() + ":" + minute.ToString();
    }
}

public enum GameState
{
    Start,
    Menu,
    DayCycle,
    NightCycle,
    EndGame
}
