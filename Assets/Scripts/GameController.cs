using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameController : MonoBehaviour {
	[SerializeField]
	private GameObject flagSprite;
	[SerializeField]
	private GameObject tileCell;
	[SerializeField]
	private GameObject mine;
	[SerializeField]
	private GameObject squareCell;
	[SerializeField]
	private GameObject difficultyButtons;
	[SerializeField]
	private GameObject smileyObject;
	[SerializeField]
	private GameObject NumMinesToFlag;
	[SerializeField]
	private GameObject gameResultText1;
	[SerializeField]
	private GameObject gameResultText2;
	[SerializeField]
	private Sprite loseSmiley;
	[SerializeField]
	private Sprite blockedMine;
	[SerializeField]
	private GameObject tile_textMesh;
	private GameObject[][] cell2D_Matrix;
	private GameObject[] mine_Array;
	private int[][] mineMatrix;
	private float startPosX = -5.0f;
	private float startPosY = 3.25f;
	private int rows;
	private int columns;
	private int num_mines;
	private bool gameOver = false;
	private int numFlags = 0;

	public GameObject NumMinesToFlagText {
		get{ return NumMinesToFlag; }
	}

	public GameObject[][] getCell2D_Matrix {
		get{ return cell2D_Matrix; }
	}

	public GameObject[] MineObjectArray {
		get{ return mine_Array; }
	}

	public int[][] MineMatrix {
		get{ return mineMatrix; }
	}

	public bool GameOver {
		get{ return this.gameOver; }
		set{ this.gameOver = value; }
	}

	public GameObject FlagSprite {
		get{ return this.flagSprite; }
	}

	public Sprite BlockedMineSprite {
		get{ return this.blockedMine; }
	}

	public int NumberMines {
		get{return this.num_mines;}
		set{this.num_mines = value;}
	}

	public int NumberFlags {
		get{return this.numFlags;}
		set{this.numFlags = value;}
	}

	private void BuildTileCellBoard(){
		float tempX = startPosX;
		float tempY = startPosY;
		GameObject[][] temp2D_array = new GameObject[rows][];

		for (int i = 0; i < rows; i++){
			temp2D_array [i] = new GameObject[columns];
			for (int j = 0; j < columns; j++) {
				temp2D_array[i][j] = (GameObject) Instantiate (tileCell, new Vector3 (tempX, tempY, 0.0f), Quaternion.identity);
				temp2D_array [i] [j].GetComponent<Cell> ().MatrixNumberX = i;
				temp2D_array [i] [j].GetComponent<Cell> ().MatrixNumberY = j;
				temp2D_array [i] [j].GetComponent<Cell> ().setGameController = this;
				Instantiate (squareCell, new Vector3 (tempX, tempY, 1.0f), Quaternion.identity);
				tempX += 0.6f;
			}
			tempX = startPosX;
			tempY -= 0.6f;
		}

		cell2D_Matrix = temp2D_array;
	}

	//places random mines into the cell2D_Matrix
	private void PlaceRandomMines(){
		int[][] mine_matrix = new int[num_mines][];
		for(int i = 0; i < num_mines; i++){
			bool isDuplicate = false;
			int rand_x = 0, rand_y = 0;
			while (!isDuplicate) {
				rand_x = Random.Range (0, this.rows);
				rand_y = Random.Range (0, this.columns);
				isDuplicate = check_Duplicate_Mines (mine_matrix, rand_x, rand_y, i);
			}
			mine_matrix [i] = new int[2];
			mine_matrix [i][0] = rand_x;
			mine_matrix [i][1] = rand_y;
		}
		GameObject[] temp_mineArray = new GameObject[num_mines];
		int k = 0;
		foreach(int[] i in mine_matrix){
			cell2D_Matrix [i [0]] [i [1]].GetComponent<Cell> ().NumberAdjacentMines = 13;
			float x_pos = cell2D_Matrix [i [0]] [i [1]].gameObject.transform.position.x;
			float y_pos = cell2D_Matrix [i [0]] [i [1]].gameObject.transform.position.y;
			temp_mineArray[k++] = (GameObject) Instantiate (mine, new Vector3(x_pos, y_pos, 1.0f), Quaternion.identity);
		}
		this.mine_Array = temp_mineArray;
		this.mineMatrix = mine_matrix;
	}

	//checks for duplicate mines at same location
	private bool check_Duplicate_Mines(int[][] arr, int x, int y, int range){
		for(int i = 0; i < range; i++){
			if (arr[i] [0] == x && arr[i] [1] == y) {
				return false;
			}
		}
		return true;
	}

	private void PlaceNumMinesText(){
		for (int i = 0; i < rows; i++) {
			for (int j = 0; j < columns; j++) {
				int num_adj_mines = FindAllAdjacentMines (i, j);
				if (num_adj_mines != 0) {
					cell2D_Matrix [i] [j].GetComponent<Cell> ().NumberAdjacentMines = num_adj_mines;
					float x_pos = cell2D_Matrix [i] [j].gameObject.transform.position.x;
					float y_pos = cell2D_Matrix [i] [j].gameObject.transform.position.y;
					GameObject tile_text = (GameObject) Instantiate (tile_textMesh, new Vector3 (x_pos, y_pos, 1.0f), Quaternion.identity);
					tile_text.GetComponent<TextMesh>().characterSize = 0.5f;
					tile_text.GetComponent<TextMesh>().alignment = TextAlignment.Center;
					tile_text.GetComponent<TextMesh>().anchor = TextAnchor.MiddleCenter;
					tile_text.GetComponent<TextMesh>().transform.position = new Vector3 (x_pos, y_pos, 1.0f);
					tile_text.GetComponent<TextMesh>().text = num_adj_mines.ToString ();
					tile_text.GetComponent<TextMesh>().color = Color.blue;
					/*
					GameObject holderObject = new GameObject();
					TextMesh text = holderObject.AddComponent<TextMesh> ();
					text.characterSize = 0.5f;
					text.alignment = TextAlignment.Center;
					text.anchor = TextAnchor.MiddleCenter;
					text.transform.position = new Vector3 (x_pos, y_pos, 1.0f);
					text.text = num_adj_mines.ToString ();
					text.color = Color.blue;
					*/
				}
			}
		}
	}

	private int FindAllAdjacentMines(int i, int j){
		int sum = 0;
		if (cell2D_Matrix [i] [j].GetComponent<Cell> ().NumberAdjacentMines != 13) {
			if (i - 1 >= 0 && cell2D_Matrix [i-1] [j].GetComponent<Cell> ().NumberAdjacentMines == 13) {
				sum++;
			}
			if (j - 1 >= 0 && cell2D_Matrix [i] [j-1].GetComponent<Cell> ().NumberAdjacentMines == 13) {
				sum++;
			}
			if (i+1 < rows && cell2D_Matrix [i+1] [j].GetComponent<Cell> ().NumberAdjacentMines == 13) {
				sum++;
			}
			if (j+1 < columns && cell2D_Matrix [i] [j+1].GetComponent<Cell> ().NumberAdjacentMines == 13) {
				sum++;
			}
			if (i+1 < rows && j+1 < columns && cell2D_Matrix [i+1] [j+1].GetComponent<Cell> ().NumberAdjacentMines == 13) {
				sum++;
			}
			if (i-1 >= 0 && j-1 >= 0 && cell2D_Matrix [i-1] [j-1].GetComponent<Cell> ().NumberAdjacentMines == 13) {
				sum++;
			}
			if (i+1 < rows && j-1 >= 0 && cell2D_Matrix [i+1] [j-1].GetComponent<Cell> ().NumberAdjacentMines == 13) {
				sum++;
			}
			if (i-1 >= 0 && j+1 < columns && cell2D_Matrix [i-1] [j+1].GetComponent<Cell> ().NumberAdjacentMines == 13) {
				sum++;
			}
		}
		return sum;
	}

	private void StartGame(){
		this.difficultyButtons.SetActive(false);
		startPosX = ((columns / 2) * -0.6f) + (0.3f * Mathf.Pow(0, (columns%2))); //centers TileCells
		this.smileyObject.SetActive(true);
		this.NumMinesToFlagText.SetActive (true);
		this.NumMinesToFlagText.GetComponent<Text>().text = "Flag Mines: " + num_mines;
		BuildTileCellBoard ();
		PlaceRandomMines ();
		PlaceNumMinesText ();
	}

	public void beginnerDifficulty(){
		this.rows = 9;
		this.columns = 9;
		this.num_mines = 9;
		StartGame ();
	}

	public void intermediateDifficulty(){
		this.rows = 14;
		this.columns = 16;
		this.num_mines = 40;
		StartGame ();
	}

	public void expertDifficulty(){
		this.rows = 14;
		this.columns = 24;
		this.num_mines = 70;
		StartGame ();
	}

	public void Lose(){
		this.GameOver = true;
		gameResultText1.GetComponent<Text> ().text = "GAME OVER :(";
		gameResultText2.GetComponent<Text> ().text = "GAME OVER :(";
		gameResultText1.SetActive (true);
		gameResultText2.SetActive (true);
		smileyObject.GetComponent<SpriteRenderer> ().sprite = loseSmiley;
	}

	public void Win(){
		this.GameOver = true;
		gameResultText1.GetComponent<Text> ().text = "YOU WIN! :)";
		gameResultText2.GetComponent<Text> ().text = "YOU WIN! :)";
		gameResultText1.SetActive (true);
		gameResultText2.SetActive (true);
	}

}
