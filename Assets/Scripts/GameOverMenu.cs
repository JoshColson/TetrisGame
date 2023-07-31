using Assets.Controllers;
using TMPro;
using UnityEngine;
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
		score = GameData.score;
		difficultyText.text = difficultyWriting+GameData.difficultyLevel.ToString();
		scoreText.text = scoreWriting+score.ToString();
		returnButton.onClick.AddListener(ReturnClick);

		SetHighScore();
	}

	private void SetHighScore()
	{
		if (score > GameData.highScore)
		{
			GameData.highScore = score;
			newText.enabled = true;
		}
		highScore.text = highScoreWriting + GameData.highScore.ToString();
		SaveHighScore(score);
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
