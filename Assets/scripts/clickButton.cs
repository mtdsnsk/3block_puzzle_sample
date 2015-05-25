using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class clickButton : MonoBehaviour {

	public GameObject button;
	public GameObject obj;

	public void ClickTest () {

		obj.SetActive (true);
		Destroy (button);

	}
}
