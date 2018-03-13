using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour {

	public void Restart()
	{
		Game.curr_score = 0;
		SceneManager.LoadScene ("Main");
	}
}
