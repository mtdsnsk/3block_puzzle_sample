using UnityEngine;
using UnityEngine.UI;
using System.Collections;
//using CreateFruits;

public class clickButton : MonoBehaviour {

	public GameObject button;
	public GameObject obj;

	private bool flg;

	void Start(){
		flg = false;
	}

	public void ClickTest () {

		if(flg){
			//obj.SetActive (true);
			//button.SetActive (false);
			Application.LoadLevel (0);
		}else{
			obj.SetActive (true);
			button.SetActive (false);
			flg = true;
		}

	}
}
