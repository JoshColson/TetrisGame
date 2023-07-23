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

		public TetrominoData? heldTetromino { get; set; }

		private HoldTetromino holdTetrominoScript;
		private PreviewTetromino previewTetrominoScript;

		private GameObject holdTetrominoObject;

		private GameObject previewTetrominoObject;

		public SceneController(TetrominoData[] tetrominos)
		{
			holdTetrominoObject = GameObject.Find("HoldTetromino");
			previewTetrominoObject = GameObject.Find("PreviewTetromino");

			holdTetrominoScript = holdTetrominoObject.GetComponent<HoldTetromino>();
			previewTetrominoScript = previewTetrominoObject.GetComponent<PreviewTetromino>();

			//Initialize tetromino data
			tetrominoes = tetrominos;

			for (int i = 0; i < tetrominoes.Length; i++)
			{
				tetrominoes[i].Initialize();
			}

			//Populate nextTetronimo
			NewNextTetronimo();
		}

		public void NewNextTetronimo()
		{
			int random = Random.Range(0, tetrominoes.Length);
			nextTetronimo = tetrominoes[random];
			previewTetrominoScript.UpdateNextTetrominoVisuals(nextTetronimo);
		}

		public void MoveUpTetronimos()
		{
			NewTetronimo(nextTetronimo);
			NewNextTetronimo();
		}

		public void NewTetronimo(TetrominoData tetromino)
		{
			currentTetronimo = tetromino;
		}

		public void StoreHeldTetromino()
		{
			if (heldTetromino is null)
			{
				heldTetromino = currentTetronimo;
				MoveUpTetronimos();
			}
			else
			{
				heldTetromino = currentTetronimo;
			}

			holdTetrominoScript.UpdateHeldTetrominoVisuals(heldTetromino);
		}


	}

}
