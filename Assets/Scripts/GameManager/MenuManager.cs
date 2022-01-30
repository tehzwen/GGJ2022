using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{

	private void Awake()
	{
        GameManager.OnGameStateChanged += GameManagerOnOnGameStateChanged;

	}

	private void OnDestroy()
	{
        GameManager.OnGameStateChanged -= GameManagerOnOnGameStateChanged;
	}

    private void GameManagerOnOnGameStateChanged(GameState state)
	{
        //throw new System.NotImplementedException();
        //_colorSelectPanel.SetActive(state == GameState.SelectColor);
	}

	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
