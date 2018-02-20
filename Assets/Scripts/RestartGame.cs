using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class RestartGame : MonoBehaviour {

	private void OnMouseDown(){
		SceneManager.LoadScene (0);
	}

	private void OnMouseOver(){
		gameObject.transform.localScale = new Vector3 (3.0f, 3.0f, 1.0f);
	}

	private void OnMouseExit(){
		gameObject.transform.localScale = new Vector3 (2.75f, 2.75f, 1.0f);
	}
}
