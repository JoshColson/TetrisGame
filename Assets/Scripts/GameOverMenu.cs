using Assets.Controllers;
using Assets.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverMenu : MonoBehaviour
{
	// Start is called before the first frame update

	public Button returnButton;
	private GameOverUiController uiController;

	private int score { get; set; } = 0;

	public void Start()
	{
		uiController = GameObject.FindWithTag(Tags.SceneController.ToString()).GetComponent<GameOverUiController>();
		score = GameData.score;
		returnButton.onClick.AddListener(ReturnClick);

		SetDifficultyLevel();
		SetScore();
	}

	private void SetDifficultyLevel()
	{
		uiController.SetDifficultyLevel(GameData.difficultyLevel);
	}
	private void SetScore()
	{
		bool isNewHighScore = score > GameData.highScore;
		uiController.SetScoreText(score);
		if (isNewHighScore)
		{
			GameData.highScore = score;
			SaveHighScore(score);
		}
		uiController.SetHighScoreText(GameData.highScore, isNewHighScore);
	}
		
	private void SaveHighScore(int score)
	{
		PlayerPrefs.SetInt("HighScore", score);
	}

	private void ReturnClick()
	{
		NavigationController.SceneNavigate(SceneNames.Welcome);
	}
}
