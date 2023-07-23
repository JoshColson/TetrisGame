using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Controllers
{
	public class SceneController : MonoBehaviour
	{
		public TetrominoData nextTetronimo { get; set; }

		public TetrominoData currentTetronimo { get; set; }
		public TetrominoData[] tetrominoes { get; set; }



		public SceneController(TetrominoData[] tetrominos)
		{
			//Initialize tetromino data
			tetrominoes = tetrominos;

			for (int i = 0; i < tetrominoes.Length; i++)
			{
				tetrominoes[i].Initialize();
			}

			//Populate nextTetronimo
			newNextTetronimo();
		}

		public void newNextTetronimo()
		{
			int random = UnityEngine.Random.Range(0, tetrominoes.Length);
			nextTetronimo = tetrominoes[random];
		}

		public void MoveUpTetronimos()
		{
			currentTetronimo = nextTetronimo;
			newNextTetronimo();
		}


	}

}
