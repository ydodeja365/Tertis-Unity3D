using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Game : MonoBehaviour {
	public Text Score;
	public Text Level;
	public static int w=10;
	public static int h=20;
	public static Transform[,] grid=new Transform[w,h];
	public int OneClear=50;
	public int TwoClear=200;
	public int ThreeClear=500;
	public int Tetris=1000;
	private int linesCleared=0;
	public static int curr_score=0;
	public AudioClip lineClearSound;
	private GameObject preview;
	private GameObject next;
	private bool gameStarted=false;
	private Vector2 previewpos=new Vector2(-6.5f,15f);
	public float fallSpeed=1.0f;
	public int currentLevel=0;
	private int numLinesClear=0;
	// Use this for initialization
	void Start () {
		getRandomBlock ();
	}
	void Update()
	{
		UpdateScore ();
		UpdateLevel ();
		UpdateSpeed ();
		//Debug.Log (currentLevel + " " + fallSpeed);
	}
	public void UpdateScore()
	{
		if (linesCleared > 0) {
			numLinesClear += linesCleared;
			GetComponent<AudioSource> ().PlayOneShot (lineClearSound);
			switch (linesCleared) {
			case 1:
				curr_score += OneClear+(currentLevel*25);
				break;
			case 2:
				curr_score += TwoClear+(currentLevel*50);
				break;
			case 3:
				curr_score += ThreeClear+(currentLevel*75);
				break;
			case 4:
				curr_score += Tetris+(currentLevel*100);
				break;
			}
			linesCleared = 0;
		}
		Score.text = curr_score.ToString ();
	}
	public void GameOver()
	{
		SceneManager.LoadScene("GameOver");
	}
	public bool IsGameOver(Block b)
	{
		for (int x = 0; x < w; x++) {
			foreach (Transform mino in b.transform)
			{
				Vector2 pos=Round(mino.position);
				if (pos.y > h)
					return true;
			}
		}
		return false;
	}
	public bool isRowFilled(int y)
	{
		for (int x = 0; x < w; x++) {
			if (grid [x, y] == null)
				return false;
		}
		return true;
	}
	public void DeleteMinos(int y)
	{
		for (int x = 0; x < w; x++) {
			Destroy (grid [x, y].gameObject);
			grid [x, y] = null;
		}
	}
	public void ShiftBoardRow(int y)
	{
		for(int x=0;x<w;x++)
		{
			if (grid [x, y] != null) {
				grid [x, y - 1] = grid [x, y];
				grid [x, y] = null;
				grid [x, y - 1].position += new Vector3 (0, -1, 0);

			}
		}
	}
	public void ShiftBoard(int y)
	{
		for (int i = y; i < h; i++) {
			ShiftBoardRow (i);
		}
	}
	public void DeleteRow()
	{
		for (int y = 0; y < h; y++) {
			if (isRowFilled (y)) {
				linesCleared++;
				DeleteMinos (y);
				ShiftBoard (y+1);
				--y;
			}
		}
	}

	public void UpdateGrid(Block block)
	{
		for (int y = 0; y < h; y++) {
			for (int x = 0; x < w; x++) {
				if (grid [x, y] != null) {
					if (grid [x, y].parent == block.transform) {
						grid [x, y] = null;
					}
				}
			}
		}
		foreach (Transform mino in block.transform) {
			Vector2 pos = Round (mino.position);
			if (pos.y < h) {
				grid [((int)pos.x)-1, (int)pos.y] = mino.transform;
			}
		}
	}
	public bool InsideGrid(Vector2 pos)
	{
		return (int)pos.x > 0 && (int)pos.x <= w && (int)pos.y >= 0;
	}
	public Vector2 Round(Vector2 pos)
	{
		return new Vector2 (Mathf.Round (pos.x), Mathf.Round (pos.y));
	}
	public void getRandomBlock()
	{
		
		if (!gameStarted) {
			gameStarted = true;
			next = (GameObject)Instantiate (Resources.Load ("Prefabs/" + getRandomName (), typeof(GameObject)), new Vector2 (5.0f, 20.0f), Quaternion.identity);
			preview = (GameObject)Instantiate (Resources.Load ("Prefabs/" + getRandomName (), typeof(GameObject)), previewpos, Quaternion.identity);
			preview.GetComponent<Block> ().enabled = false;
		} else {
			preview.transform.localPosition = new Vector2 (5.0f, 20.0f);
			next = preview;
			next.GetComponent<Block> ().enabled = true;
			preview = (GameObject)Instantiate (Resources.Load ("Prefabs/" + getRandomName (), typeof(GameObject)), previewpos, Quaternion.identity);
			preview.GetComponent<Block> ().enabled = false;
		}
	}
	private string getRandomName()
	{
		string name="_BLOCK";
		int n = Random.Range (1, 8);
		switch (n) {
		case 1:
			name = "SQ" + name;
			break;
		case 2:
			name = "LONG" + name;
			break;
		case 3:
			name = "S" + name;
			break;
		case 4:
			name = "Z" + name;
			break;
		case 5:
			name = "J" + name;
			break;
		case 6:
			name = "L" + name;
			break;
		case 7:
			name = "E" + name;
			break;
		}
		return name;
	}
	public Transform GetVectorAtPos(Vector2 pos)
	{
		if (pos.y >= h)
			return null;
		else
			return grid [((int)pos.x)-1, (int)pos.y];
	}
void UpdateLevel()
{
		int x = numLinesClear / 10;
		if (x!=0) {
			currentLevel += x;
			numLinesClear = 0;
		}
		Level.text = currentLevel.ToString ();
}
void UpdateSpeed()
{
		fallSpeed = 1.0f - ((float)currentLevel * 0.1f);
}
}