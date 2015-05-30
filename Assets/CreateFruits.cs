using UnityEngine;
using UnityEngine.Sprites;
using UnityEngine.UI;

using System.Collections;
using System.Collections.Generic;

public class CreateFruits : MonoBehaviour
{
	//
	//public Camera camera;

	// ポーズ
	public bool isPaused = false;
	// 生成するプレファブのリスト
	public List<GameObject> fruitsObjectList = new List<GameObject> ();
	// 生成するプレファブのリスト
	public List<GameObject> specialObjectList = new List<GameObject> ();
	// 生
	public List<GameObject> chainObjectList = new List<GameObject> ();

	// ブロック同士をつなぐ線を描画する
	public LineRenderer line;
	// 効果エフェクト
	public GameObject effect1;
	public GameObject effect2;
	// スタートボタン
	public GameObject button;

	// タイマー
	public float gameTimer;
	public Text gameTimerText;
	// スコア
	private int gameScore;
	public Text gameScoreText;

	static int fruitsListNum = 4;
	static float chainDistance = 3.0f;

	// 全てのGameObjectのリスト
	private List<GameObject> objectList = new List<GameObject> ();
	// 削除するGameObjectのリスト
	private List<GameObject> removableObjectList = new List<GameObject> ();
	// 現在選択中のGameObject
	private GameObject chainObj; // chain
	private GameObject firstObject; // first
	private GameObject lastObject; // last
	private GameObject currentObject; // current
	private float fruits_distance; //  distance

	// スキルゲージ
	public GameObject skill_gauge;

	//
	public GameObject special_effect_particle;

	//
	private int deleteObjectCounter = 0; 
	private int deleteGaugeCounter = 0;
	private int deleteObjectMax =  50; 

//	IEnumerator LateStart (float time)
//	{
//		yield return new WaitForSeconds (time);
//		//遅れて初期化処理
//	}
	
	// 開始
	void Start ()
	{
		gameTimer =  60.0f;
		gameScore = 0;

		StartCoroutine ("Wait", 40);
		isPaused = false;
	}

	//
//	public void createFruits ()
//	{
//		DropBall (45); //フルーツを指定した数上から降らせる
//	}
	
	// フルーツを生成する
	void DropBall (int count)
	{
		//
		StartCoroutine ("Wait", count);
		isPaused = false;
	}

	//
	IEnumerator Wait(int count) {
		for (var i = 0; i < count; i++) {
			// リストの中から一つをランダムで選択する
			GameObject fruits = fruitsObjectList [Random.Range (0, fruitsListNum)];
			float pos_x = Random.Range (-2.5f, 2.5f);
			float pos_y = Random.Range (8.8f, 10.2f);
			// 生成する
			GameObject obj = (GameObject)Instantiate (fruits, new Vector3 (pos_x, pos_y, 0), Quaternion.identity);
			// リストに追加する
			objectList.Add (obj);
			//
			yield return new WaitForSeconds(0.05f);
		}
		yield return null;
	}

	// フレーム毎に呼ばれる
	void Update ()
	{
		if (isPaused) {
			return; // ポーズ中
		}

		if (Input.GetMouseButton (0) && firstObject == null) {
			//ボールをドラッグしはじめたとき
			OnDragStart ();
		} else if (Input.GetMouseButtonUp (0)) {
			//ボールをドラッグし終わったとき
			OnDragEnd ();
		} else if (firstObject != null) {
			//ボールをドラッグしている途中
			OnDragging ();
			//calc_chain (currentObject);
		}

		if (Input.GetKeyDown ("space")) {
			addForce ();
		}

		drawLineBetweenObject (); // 繋げたブロック同士を線で結ぶ

		gameTimer -= Time.deltaTime;
		gameTimerText.text = Mathf.FloorToInt(gameTimer).ToString ();

		if (0.0f >= gameTimer) {
			// タイマーが終わった時の処理
			isPaused = true;
			gameTimerText.text = "Game Over";
			// スタートボタンをアクティブに
			button.SetActive (true);
		}

	}

	void OnDragStart ()
	{
		// クリックした位置のオブジェクトを取得する
		GameObject obj = GetCurrentHitObject ();

		// タグをチェック
		if(obj == null || obj.tag != "GameFruits"){
			//firstObject = null;
			return;
		}

		if (obj != null) {

			//Debug.Log("name:" + obj.name);
			if(obj.name.Contains("candy6")){
				special1 (obj) ;
				return;
			}

			if(obj.name.Contains("candy9")){
				special1 (obj) ;
				return;
			}

			if(obj.name.Contains("candy10")){
				special1 (obj) ;
				return;
			}

			// 最初に選択したオブジェクトを保持
			firstObject = obj; 
			fruits_distance = obj.GetComponent<CircleCollider2D> ().radius  * obj.transform.localScale.x * chainDistance;
			// リストに追加
			PushToList (obj);
		}
	}

	//
	void special1 (GameObject spObj) 
	{
		// 
		float radius = spObj.GetComponent<CircleCollider2D> ().radius * spObj.transform.localScale.x * 3.0f;
		//
		removableObjectList.Add (spObj);
		//
		foreach(GameObject obj in objectList){
			// 距離
			float dist = Vector2.Distance (spObj.transform.position, obj.transform.position);
			if (dist <= radius) {
				// candyは消さない
				if(obj.name.Contains("candy")){
					continue;
				}
				//距離が一定値以下のとき
				removableObjectList.Add (obj);
			}
		}

		// 全部削除
		foreach (GameObject obj in removableObjectList) {
			objectList.Remove (obj);
			Destroy (obj);
		}

		// effect
		GameObject effect = (GameObject)Instantiate(effect1.gameObject, spObj.transform.position, Quaternion.identity);
		Destroy (effect, 1.0f);

		// 削除した分を追加する
		DropBall (removableObjectList.Count);

		// リセット
		reset ();
	}

	//
	void colorChange (GameObject obj)
	{
		obj.GetComponent<SpriteRenderer> ().color = new Color (0.3f, 0.3f, 0.3f, 1.0f);
	}

	//
	void colorReset ()
	{
		foreach (GameObject mObj in objectList) {
			mObj.GetComponent<SpriteRenderer> ().color = new Color (1.0f, 1.0f, 1.0f, 1.0f);
		}
	}

	//
	void OnDragEnd ()
	{

		if (removableObjectList.Count >= 3) {
			// 全部削除
			foreach (GameObject obj in removableObjectList) {
				// 削除
				GameObject effect = (GameObject)Instantiate (
					effect2, 
					new Vector3(obj.transform.position.x, obj.transform.position.y, -5.0f), 
					Quaternion.identity);
				Destroy (effect, 1.0f);
				objectList.Remove (obj);
				Destroy (obj);
			}

			Vector3 pos = new Vector3 (lastObject.transform.position.x, lastObject.transform.position.y, 0);
			int pattern = Random.Range (0, specialObjectList.Count - 1);

			if (removableObjectList.Count >= 6) {
				// スペシャルを追加
				GameObject spObj = (GameObject)Instantiate (specialObjectList [pattern], pos, Quaternion.identity);
				objectList.Add (spObj);

				// 削除した分を追加する
				DropBall (removableObjectList.Count - 1);
			} else {
				// 削除した分を追加する
				DropBall (removableObjectList.Count);
			}
		}

		//
		reset ();
	}

	//
	IEnumerator WaitAndPrint(float waitTime) {

		deleteGaugeCounter = 0; //reset

		List<GameObject> tmpList = new List<GameObject> ();
		int n = 0;
		isPaused = true; // 停止
		//camera.backgroundColor = new Color(1, 0 , 0, 1);

		//
		foreach(GameObject obj in objectList){
			if(obj.name.Contains("cut")){
				//
				Debug.Log("obj.name:" + obj.name);
				tmpList.Add(obj);
				n++;
			}
		}

		//
		foreach(GameObject obj in tmpList){
			objectList.Remove (obj);
			Destroy (obj);
		}

		// score
		gameScore += 100 * n;
		gameScoreText.text = gameScore.ToString ();

		yield return new WaitForSeconds(waitTime);

		DropBall (n);
		isPaused = false; // 開始
		//camera.backgroundColor = new Color(80/256f, 135/256f, 221/256f, 1.0f);
	}

	//
	void reset () 
	{
		if (removableObjectList.Count >= 3) {
			// 削除された総数
			deleteObjectCounter += removableObjectList.Count;
			deleteGaugeCounter += removableObjectList.Count;

			// ゲージがマックスになった
			if(deleteGaugeCounter >= deleteObjectMax){
				deleteGaugeCounter = 0;
				GameObject effect = (GameObject)Instantiate(special_effect_particle.gameObject, 
				                                            special_effect_particle.transform.position, 
				                                            Quaternion.identity);
				Destroy (effect, 2.0f);
				//StartCoroutine(WaitAndPrint(2.0F));
			}

			// ゲージを伸ばす
			skill_gauge.transform.localScale = new Vector3 (0.02f * deleteGaugeCounter, 
		                                               skill_gauge.transform.localScale.y, 
		                                               skill_gauge.transform.localScale.z);
		}

		// score
		gameScore += 100 * fib (removableObjectList.Count);
		gameScoreText.text = gameScore.ToString ();
		
		// nullをセット
		firstObject = null;
		lastObject = null;
		currentObject = null;
		// 色をリセット
		colorReset ();
		line.SetVertexCount (0);
		removableObjectList = new List<GameObject> ();
		
		// 下方向へ力を加える
		addDownForce ();
	}

	//
	int fib (int n)
	{
		int p;
		int f0;
		int f1;
		int f2;

		p = 0; 
		f0 = 0;
		f1 = 1;
		f2 = 0;

		while (p<n) {
			// フィボナッチ数の出力(n>0)
			 //Debug.Log ("fib:" + f1);
			// フィボナッチ数の計算
			f2 = f1 + f0;
			// 変数の代入
			f0 = f1;
			f1 = f2;

			p++;
		}
				 
		return f2;
	}

	void OnDragging ()
	{
		// 現在選択中のオブジェクト
		currentObject = GetCurrentHitObject ();

		if (currentObject != null) {
			// 最初のオブジェクトと名前が一致するか
			if (currentObject.name != firstObject.name){
				return; // 一致しない
			}

			// 一つ前に追加したものと一致するか
			if (removableObjectList.Count > 1 && currentObject == removableObjectList[removableObjectList.Count  -2]) {
				//
				PopToList();
				return; // 直前のものと一致
			}

			// 直前に追加したものと一致するか
			if (currentObject == lastObject) {
				return; // 直前のものと一致
			}

			// すでに追加済みのものは無視
			if (removableObjectList.Contains (currentObject))
				return; // 追加済み

			// 距離
			float dist = Vector2.Distance (currentObject.transform.position, lastObject.transform.position);
			//直前のボールと現在のボールの距離を計算
			if (dist <= fruits_distance) {
				//ボール間の距離が一定値以下のとき
				PushToList (currentObject); //消去するリストにボールを追加
				chainObjectList = new List<GameObject> ();
			}
		}
	}

	//
	void calc_chain(GameObject chainBlock){

		if (chainObjectList.Contains (chainBlock)){
			return; // 追加済み
		}

		//
		foreach(GameObject obj in objectList){

			//
			if (chainObjectList.Contains (obj)){
				//continue; // 追加済み
			}

			//
			if (firstObject.name != obj.name){
				Debug.Log("一致しない chain:" + chainBlock.name + " " + obj.name);
				continue; // 一致しない
			}

			// 距離
			float dist = Vector2.Distance (chainBlock.transform.position, obj.transform.position);
			if (dist <= fruits_distance) {
				//ボール間の距離が一定値以下のとき
				obj.GetComponent<SpriteRenderer> ().color = new Color (0.9f, 1.0f, 0, 1.0f);
				// 追加
				chainObjectList.Add(obj);
				//
				calc_chain(obj);
			}
		}
	}

	// 現在選択中のオブジェクトを取得する
	GameObject GetCurrentHitObject ()
	{
		//
		var mousePos = Input.mousePosition;
		mousePos.z = 10; // select distance = 10 units from the camera
		
		Vector2 tapPoint = Camera.main.ScreenToWorldPoint (mousePos);
		Collider2D collider = Physics2D.OverlapPoint (tapPoint);

		if (collider) {
			return collider.gameObject;
		}

		return null;
	}

	// リストに追加
	void PushToList (GameObject obj)
	{
		// color change
		obj.GetComponent<SpriteRenderer> ().color = new Color (0.6f, 0.6f, 0.6f, 0.5f);
			
		lastObject = obj;
		removableObjectList.Add (obj);
	}

	// リストから削除
	void PopToList ()
	{
		// color change
		removableObjectList[removableObjectList.Count - 1].GetComponent<SpriteRenderer> ().color = new Color (1.0f, 1.0f, 1.0f, 1.0f);
		removableObjectList.Remove (removableObjectList [removableObjectList.Count - 1]);
		//
		lastObject = removableObjectList [removableObjectList.Count - 1];
	}

	// 線を描画する
	void drawLineBetweenObject ()
	{
		//
		if (removableObjectList.Count > 1) {
			line.SetVertexCount (removableObjectList.Count);
			for (int i=0; i<removableObjectList.Count; i++) {
				line.SetPosition (i, removableObjectList [i].transform.position);
			}
		}
	}

	// 
	void addForce ()
	{
		foreach (GameObject obj in objectList) {
			Rigidbody2D rb = obj.GetComponent<Rigidbody2D> ();
			float power_x = Random.Range (-15.0f, 15.0f);
			float power_y = Random.Range (3.0f, 10.0f);
			rb.AddForce (new Vector2 (power_x, power_y) * 800);
		}
	}

	// 
	void addDownForce ()
	{
		foreach (GameObject obj in objectList) {
			Rigidbody2D rb = obj.GetComponent<Rigidbody2D> ();
			float power_y = Random.Range (-3.0f, -10.0f);
			rb.AddForce (new Vector2 (0, power_y) * 100);
		}
	}
}
