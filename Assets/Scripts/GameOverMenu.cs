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
		score = GameOverData.score;
		returnButton.onClick.AddListener(ReturnClick);

		SetDifficultyLevel();
		SetScore();
	}

	private void SetDifficultyLevel()
	{
		uiController.SetDifficultyLevel(GameOverData.difficultyLevel);
	}
	private void SetScore()
	{
		bool isNewHighScore = score > GameOverData.highScore;
		uiController.SetScoreText(score);
		if (isNewHighScore)
		{
			GameOverData.highScore = score;
		}
		uiController.SetHighScoreText(GameOverData.highScore, isNewHighScore);
	}

	private void ReturnClick()
	{
		NavigationController.SceneNavigate(SceneNames.Welcome);
	}
}
