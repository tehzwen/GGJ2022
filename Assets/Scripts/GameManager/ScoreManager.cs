using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public Text scoreText;
	private int score = 0;

	public static ScoreManager Instance;

	private void Awake()
	{
		Instance = this;
	}

	private void Update()
	{
        scoreText.text = "Score: " + score;
	}

	void IncrementScore(int amount)
	{
        score += amount;
	}
}
