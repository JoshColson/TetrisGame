using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Controllers;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverMenu : MonoBehaviour
{
	// Start is called before the first frame update
	const string scoreWriting = "Score: ";
	const string difficultyWriting = "Difficulty: ";
	const string highScoreWriting = "High Score: ";
	public Button returnButton;

	public TextMeshProUGUI difficultyText;
	public TextMeshProUGUI scoreText;
	public TextMeshProUGUI newText;
	public TextMeshProUGUI highScore;

	private int score = 0;

	public void Start()
	{
		score = GameOverData.score;
		difficultyText.text = difficultyWriting+GameOverData.difficultyLevel.ToString();
		scoreText.text = scoreWriting+score.ToString();
		returnButton.onClick.AddListener(ReturnClick);

		SetHighScore();
	}

	private void SetHighScore()
	{
		if (score > GameOverData.highScore)
		{
			GameOverData.highScore = score;
			newText.enabled = true;
		}
		highScore.text = highScoreWriting + GameOverData.highScore.ToString();
	}

	private void ReturnClick()
	{
		SceneController.SceneNavigate(SceneNames.Welcome);
	}
}
