using Assets.Controllers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Assets.Data;

public class WelcomeMenu : MonoBehaviour
{


    public Button newGameButton;
    public Button resetHighScoreButton;
    public Slider difficultySlider;

    private int difficultyLevel;
    private int maxDifficulty = 5;
    private int minDifficulty = 1;
    public int highScore { get; private set; } = 0;
    private WelcomeMenuUiController uiController { get; set; }

	private void Awake()
	{
		uiController = GameObject.FindWithTag(Tags.SceneController.ToString()).GetComponent<WelcomeMenuUiController>();
		difficultySlider.onValueChanged.AddListener(delegate { SliderChangeCheck(); });
		newGameButton.onClick.AddListener(NewGameClick);
		resetHighScoreButton.onClick.AddListener(ResetHighScoreClick);
	}

	// Start is called before the first frame update
	private void Start()
	{
        LoadSavedData();
        difficultySlider.maxValue = maxDifficulty;
        difficultySlider.minValue = minDifficulty;
		difficultySlider.wholeNumbers = true;
        SetHighScore();
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

    private void SetHighScore()
    {
		uiController.SetHighScoreText(highScore);
    }
    private void ResetHighScoreClick()
    {
        highScore = 0;
        GameData.highScore = highScore;
		PlayerPrefs.SetInt("HighScore", 0);
        SetHighScore();
        resetHighScoreButton.interactable = false;
	}

    private void SliderChangeCheck()
    {
        difficultyLevel = (int)difficultySlider.value;
        StartingData.difficultyLevel = difficultyLevel;
        GameData.difficultyLevel = difficultyLevel;
		uiController.SetDifficultySliderText(difficultyLevel);
    }
}
