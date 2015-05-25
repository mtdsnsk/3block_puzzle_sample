using UnityEngine;
using System.Collections;

public class clickButton : MonoBehaviour {

	public GameObject button;
	public GameObject obj;

	public void ClickTest () {

		Debug.Log ("Clicked.");
		Instantiate (obj, new Vector3 (0, 0, 0), Quaternion.identity);
		Destroy(button);
	}
}
