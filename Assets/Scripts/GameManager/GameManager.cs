using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameState State;

    public static event Action<GameState> OnGameStateChanged;

    public TimerController timerController;

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

                break;
            case GameState.NightCycle:
                break;
            case GameState.EndGame:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }

        OnGameStateChanged?.Invoke(newState);
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
        Debug.Log("Curr Hour: " + currHour);
        Debug.Log("Cycle: " + State);
        if (State == GameState.DayCycle && currHour > nightCycleStart)
            UpdateGameState(GameState.NightCycle);


        if (State == GameState.NightCycle && (currHour > dayCycleStart && currHour < nightCycleStart))
            UpdateGameState(GameState.DayCycle);


    }

    GameState GetGameState()
    {
        return this.State;
    }

    String GetGameTime()
    {
        float hour = timerController.GetHour() % 24;
        float minute = timerController.GetMinute() % 60;

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
