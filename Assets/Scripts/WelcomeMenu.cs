using Assets.Controllers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WelcomeMenu : MonoBehaviour
{
    const string difficultyWriting = "Difficulty Level: ";
    const string highScoreWriting = "High Score: ";

    public Button newGameButton;
    public Button resetHighScoreButton;
    public Slider difficultySlider;
    public TextMeshProUGUI difficultyText;
    public TextMeshProUGUI highScoreText;
    private int difficultyLevel;
    private int maxDifficulty = 5;
    private int minDifficulty = 1;
    private int highScore { get; set; } = 0;

    // Start is called before the first frame update
    private void Start()
	{
        LoadSavedData();
		highScoreText.text = highScoreWriting + highScore.ToString();
        difficultySlider.maxValue = maxDifficulty;
        difficultySlider.minValue = minDifficulty;
		difficultySlider.wholeNumbers = true;
		difficultySlider.onValueChanged.AddListener(delegate { SliderChangeCheck(); });
        newGameButton.onClick.AddListener(NewGameClick);
        resetHighScoreButton.onClick.AddListener(ResetHighScoreClick);
	}

    private void LoadSavedData()
    {
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        GameData.highScore = highScore;
	}

    private void NewGameClick()
    {
		NavigationController.SceneNavigate(SceneNames.Tetris);
    }

    private void ResetHighScoreClick()
    {
        highScore = 0;
        GameData.highScore = highScore;
		PlayerPrefs.SetInt("HighScore", 0);
		highScoreText.text = highScoreWriting + highScore.ToString();
        resetHighScoreButton.interactable = false;
	}

    private void SliderChangeCheck()
    {
        difficultyLevel = (int)difficultySlider.value;
        StartingData.difficultyLevel = difficultyLevel;
        GameData.difficultyLevel = difficultyLevel;

		difficultyText.text = difficultyWriting+difficultyLevel.ToString();
    }
}
