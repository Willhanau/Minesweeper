using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Cell : MonoBehaviour {
	private GameController gameController;
	private int[] matrix_num = new int[2];
	private int num_adjacent_mines = 0; //13 indicates the cell is a mine
	private bool isFlagged = false;
	private GameObject[][] arr2D;
	private GameObject[] mineObjectArray;
	private int[][] mineMatrix;
	private GameObject flagObject;

	void Start(){
		arr2D = gameController.GetComponent<GameController> ().getCell2D_Matrix;
		mineObjectArray = gameController.GetComponent<GameController> ().MineObjectArray;
		mineMatrix = gameController.GetComponent<GameController> ().MineMatrix;
		float x_pos = this.transform.position.x;
		float y_pos = this.transform.position.y;
		this.flagObject = (GameObject) Instantiate (gameController.FlagSprite, new Vector3(x_pos, y_pos, -1.0f), Quaternion.identity);
		this.flagObject.SetActive (false);
	}

	public GameController setGameController {
		set{ this.gameController = value; }
	}

	public int MatrixNumberX {
		get{ return this.matrix_num[0]; }
		set{ this.matrix_num[0] = value; }
	}

	public int MatrixNumberY {
		get{ return this.matrix_num[1]; }
		set{ this.matrix_num[1] = value; }
	}

	public int NumberAdjacentMines {
		get{ return this.num_adjacent_mines; }
		set{ this.num_adjacent_mines = value; }
	}

	public bool Flagged {
		get{ return this.isFlagged; }
		set{ this.isFlagged = value; }
	}

	public GameObject FlagObject {
		get{ return flagObject; }
	}

	private void OnMouseOver(){
		if (!gameController.GameOver) {
			if (Input.GetMouseButtonDown (0)) {
				if (!Flagged && !gameController.GameOver) {
					ExposeCell ();
				}
			} else if (Input.GetMouseButtonDown (1) && !Flagged) {
				this.Flagged = true;
				this.flagObject.SetActive (true);
				gameController.NumberFlags += 1;
				gameController.NumMinesToFlagText.GetComponent<Text> ().text = "Flag Mines: " + (gameController.NumberMines - gameController.NumberFlags);
				if (gameController.NumberFlags == gameController.NumberMines && checkWinCondition()) {
					gameController.Win ();
				}
			} else if (Input.GetMouseButtonDown (1) && Flagged) {
				this.Flagged = false;
				this.flagObject.SetActive (false);
				gameController.NumberFlags -= 1;
				gameController.NumMinesToFlagText.GetComponent<Text> ().text = "Flag Mines: " + (gameController.NumberMines - gameController.NumberFlags);
				if (gameController.NumberFlags == gameController.NumberMines && checkWinCondition()) {
					gameController.Win ();
				}
			}
		}
	}

	private void ExposeCell(){
		if (this.num_adjacent_mines == 13) {
			ExposeAllMines ();
			gameController.Lose ();
		} else {
			ExposeSurroundingCells (this.matrix_num);
		}
	}

	private void ExposeAllMines(){
		for (int i = 0; i < arr2D.Length; i++) {
			for (int j = 0; j < arr2D [i].Length; j++) {
				if (arr2D [i] [j].GetComponent<Cell> ().num_adjacent_mines == 13) {
					arr2D [i] [j].SetActive (false);
				}
			}
		}
		int k = 0;
		foreach(int[] i in mineMatrix){
			if (arr2D [i [0]] [i [1]].GetComponent<Cell> ().Flagged) {
				mineObjectArray [k].GetComponent<SpriteRenderer> ().sprite = gameController.BlockedMineSprite;
				arr2D [i [0]] [i [1]].GetComponent<Cell> ().FlagObject.SetActive(false);
			}
			k++;
		}
	}

	private void ExposeSurroundingCells(int[] idx){
		if(idx[0] >= arr2D.Length || idx[1] >= arr2D[0].Length || idx[0] < 0 || idx[1] < 0){
			return;
		}
		if(arr2D[idx[0]][idx[1]].GetComponent<Cell>().Flagged || arr2D[idx[0]][idx[1]].GetComponent<Cell>().NumberAdjacentMines == 13 || arr2D [idx [0]] [idx [1]].GetComponent<Cell> ().NumberAdjacentMines == -1){
			return;
		}
		int[] temp = { 0, 0 };
		arr2D [idx [0]] [idx [1]].SetActive(false);
		if (arr2D [idx [0]] [idx [1]].GetComponent<Cell> ().NumberAdjacentMines > 0) {
			arr2D [idx [0]] [idx [1]].GetComponent<Cell> ().NumberAdjacentMines = -1;
			return;
		} else {
			arr2D [idx [0]] [idx [1]].GetComponent<Cell> ().NumberAdjacentMines = -1;
			temp [0] = idx [0];
			temp [1] = idx [1] + 1;
			ExposeSurroundingCells(temp);

			temp [0] = idx [0] + 1;
			temp [1] = idx [1];
			ExposeSurroundingCells (temp);

			temp [0] = idx [0] + 1;
			temp [1] = idx [1] + 1;
			ExposeSurroundingCells (temp);

			temp [0] = idx [0];
			temp [1] = idx [1] - 1;
			ExposeSurroundingCells (temp);

			temp [0] = idx [0] - 1;
			temp [1] = idx [1];
			ExposeSurroundingCells (temp);

			temp [0] = idx [0] - 1;
			temp [1] = idx [1] - 1;
			ExposeSurroundingCells (temp);

			temp [0] = idx [0] + 1;
			temp [1] = idx [1] - 1;
			ExposeSurroundingCells (temp);

			temp [0] = idx [0] - 1;
			temp [1] = idx [1] + 1;
			ExposeSurroundingCells (temp);
		}
		return;
	}

	private bool checkWinCondition(){
		int k = 0;
		foreach(int[] i in mineMatrix){
			if (!arr2D [i [0]] [i [1]].GetComponent<Cell> ().Flagged) {
				return false;
			}
			k++;
		}
		return true;
	}

}
