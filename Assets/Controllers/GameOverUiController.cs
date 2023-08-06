using Assets.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUiController : MonoBehaviour
{
	private string currentScoreText
	{
		set { SetTextValue(scoreTextObject, value); }
	}
	private string currentHighScoreText
	{
		set { SetTextValue(highScoreTextObject, value); }
	}
	private string currentDifficultyLevelText
	{
		set { SetTextValue(difficultyLevelTextObject, value); }
	}

	public TextMeshProUGUI difficultyLevelTextObject;
	public TextMeshProUGUI highScoreTextObject;
	public TextMeshProUGUI scoreTextObject;
	public TextMeshProUGUI newHighScoreTextObject;

	private void SetTextValue(TextMeshProUGUI textObject, string textValue)
	{
		textObject.text = textValue;
	}

	public void SetDifficultyLevel(int difficultyLevel)
	{
		currentDifficultyLevelText = UiStrings.difficultyLevelTitle + difficultyLevel.ToString();
	}
	public void SetScoreText(int score)
	{
		currentScoreText = UiStrings.scoreTitle + score.ToString();
	}
	public void SetHighScoreText(int highScore, bool newHighScore)
	{
		currentHighScoreText = UiStrings.highScoreTitle + highScore.ToString();
		if (newHighScore)
		{
			newHighScoreTextObject.enabled = true;
		}
	}
}
