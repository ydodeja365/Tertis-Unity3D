using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour {
	public bool enableRotation = true;
	public bool partialRotation = false;
	public float fallTime = 1f;
	public float timeCount=0;
	public int blockScore=100;
	private float blockTime = 0;

	public AudioClip move;
	public AudioClip rotate;
	public AudioClip land;
	private AudioSource src;

	private float moveInterval=0.1f;
	private float downMoveInterval=0.05f;
	private float moveDelayInterval=0.1f;
	private float downmoveDelayInterval=0.1f;
	private float delayTimer = 0;
	private float downdelayTimer = 0;
	private float moveTimer=0;
	private float downMoveTimer=0;
	private bool moveImmediate=false;
	private bool moveImmediateDown = false;


	void Start () {
		src = GetComponent<AudioSource> ();
		fallTime=GameObject.Find ("GameManager").GetComponent<Game>().fallSpeed;
	}
	void PlaySound(int sound)
	{
		switch (sound) {
		case 1:
			src.PlayOneShot (move);
			break;
		case 2:
			src.PlayOneShot (rotate);
			break;
		case 3:
			src.PlayOneShot (land);
			break;
		}
	}
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyUp(KeyCode.LeftArrow)||Input.GetKeyUp(KeyCode.RightArrow))
		{
			delayTimer = 0;
			moveTimer = 0;
			 moveImmediate=false;
		
		}
		if (Input.GetKeyUp (KeyCode.DownArrow)) {
			downdelayTimer = 0;
			downMoveTimer = 0;
			moveImmediateDown = false;
		}
		
		if (Input.GetKey (KeyCode.RightArrow)) {
			moveRight ();
			}
		if (Input.GetKey (KeyCode.LeftArrow)) {
			moveLeft ();
		}
		if (Input.GetKey (KeyCode.DownArrow)||Time.time-timeCount>=fallTime) {
			moveDown ();
		}
		if (Input.GetKeyDown (KeyCode.UpArrow)) {
			RotateBlock ();
		}
		if (blockTime < 1) {
			blockTime += Time.deltaTime;
		} else {
			blockTime = 0;
			blockScore = Mathf.Max (0, blockScore - 10);
		}
	}
	void moveLeft()
	{
		if (moveImmediate) {
			if (delayTimer < moveDelayInterval) {
				delayTimer += Time.deltaTime;
				return;
			}
			if (moveTimer < moveInterval) {
				moveTimer += Time.deltaTime;
				return;
			}
		} else {
			moveImmediate = true;
		}
		moveTimer = 0;
		transform.position += new Vector3 (-1, 0, 0);
		if (inGrid ()) {
			PlaySound (1);
			FindObjectOfType<Game> ().UpdateGrid (this);
		} else {
			transform.position += new Vector3 (1, 0, 0);
		}
	}
	void moveRight()
	{
		if(moveImmediate){
			if (delayTimer < moveDelayInterval) {
				delayTimer += Time.deltaTime;
				return;
			}

			if (moveTimer < moveInterval) {
				moveTimer += Time.deltaTime;
				return;
			}
		}
		else{
			moveImmediate=true;
		}
		moveTimer = 0;
		transform.position += new Vector3 (1, 0, 0);
		if (inGrid ()) {
			PlaySound (1);
			FindObjectOfType<Game> ().UpdateGrid (this);
		} else {
			transform.position += new Vector3 (-1, 0, 0);
		}

	}
	void moveDown()
	{
		if (moveImmediateDown) {
			if (downdelayTimer < downmoveDelayInterval) {
				downdelayTimer += Time.deltaTime;
				return;
			}

			if (downMoveTimer < downMoveInterval) {
				downMoveTimer += Time.deltaTime;
				return;
			}
		} else
			moveImmediateDown = true;
		downMoveTimer = 0;
		transform.position += new Vector3 (0, -1, 0);
		if (inGrid ()) {
			FindObjectOfType<Game> ().UpdateGrid (this);
			if (Input.GetKey (KeyCode.DownArrow)) {
				PlaySound (1);
			}
		} else {
			transform.position += new Vector3 (0, 1, 0);
			FindObjectOfType<Game> ().DeleteRow ();
			if (FindObjectOfType<Game> ().IsGameOver(this))
				FindObjectOfType<Game> ().GameOver ();
			Game.curr_score += blockScore;
			PlaySound (3);
			enabled = false;
			FindObjectOfType<Game> ().getRandomBlock ();
		}
		timeCount = Time.time;
	}
	void RotateBlock()
	{
		if (enableRotation) 
		{
			if (partialRotation) {
				if (transform.eulerAngles.z >= 90)
					transform.Rotate (0, 0, -90);
				else
					transform.Rotate (0, 0, 90);
			} 
			else 
			{
				transform.Rotate (0, 0, 90);
			}
			if (inGrid ()) {
				PlaySound (2);
				FindObjectOfType<Game> ().UpdateGrid (this);
			} 
			else {
				if (partialRotation) {
					if (transform.eulerAngles.z >= 90)
						transform.Rotate (0, 0, -90);
					else
						transform.Rotate (0, 0, 90);
				} else {
					transform.Rotate (0, 0, -90);
				}
			}
		}
	}

	bool inGrid()
	{
		foreach (Transform mino in transform) {
			Vector2 pos = FindObjectOfType<Game> ().Round (mino.position);
			if (FindObjectOfType<Game> ().InsideGrid (pos) == false)
				return false;
			if (FindObjectOfType<Game> ().GetVectorAtPos (pos) != null && FindObjectOfType<Game> ().GetVectorAtPos (pos).parent != transform) {
				return false;
			}
		}
		return true;
	}
}
