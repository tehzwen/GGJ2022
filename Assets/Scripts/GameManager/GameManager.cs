using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameState State;

	public static event Action<GameState> OnGameStateChanged;

	private void Awake()
	{
        Instance = this;
	}

	// Start is called before the first frame update
	void Start()
	{
		UpdateGameState(GameState.Menu);
	}

	public void UpdateGameState(GameState newState)
	{
		State = newState;
		switch (newState)
		{
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

	private void MenuHandleSelection()
	{

	}



    // Update is called once per frame
    void Update()
    {
        
    }

}

public enum GameState
{
	Menu,
	DayCycle,
	NightCycle,
	EndGame
}
