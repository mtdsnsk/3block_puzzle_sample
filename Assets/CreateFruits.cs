using UnityEngine;
using UnityEngine.Sprites;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CreateFruits : MonoBehaviour
{
	// ポーズ
	public bool isPaused = false;
	// 生成するプレファブのリスト
	public List<GameObject> fruitsObjectList = new List<GameObject> ();
	// 生成するプレファブのリスト
	public List<GameObject> specialObjectList = new List<GameObject> ();
	// ブロック同士をつなぐ線を描画する
	public LineRenderer line;
	//
	private float gameTimer;
	public Text gameTimerText;
	private int gameScore;
	public Text gameScoreText;

	// 全てのGameObjectのリスト
	private List<GameObject> objectList = new List<GameObject> ();
	// 削除するGameObjectのリスト
	private List<GameObject> removableObjectList = new List<GameObject> ();
	// 現在選択中のGameObject
	private string currentObjectName;  // name
	private GameObject firstObject; // first
	private GameObject lastObject; // last
	private GameObject currentObject; // current
	private float fruits_distance; //  distance

	IEnumerator LateStart (float time)
	{
		yield return new WaitForSeconds (time);
		//遅れて初期化処理
	}

	// 開始
	void Start ()
	{
		gameTimer = 30.0f;
		gameScore = 0;

		DropBall (45); //フルーツを指定した数上から降らせる

		StartCoroutine (LateStart (0.1F));
	}

	public void createFruits ()
	{
		DropBall (45); //フルーツを指定した数上から降らせる
	}

	// フルーツを生成する
	void DropBall (int count)
	{
		for (var i = 0; i < count; i++) {
			// リストの中から一つをランダムで選択する
			GameObject fruits = fruitsObjectList [Random.Range (0, fruitsObjectList.Count - 1)];
			float pos_x = Random.Range (-2.5f, 2.5f);
			float pos_y = Random.Range (8.8f, 10.2f);
			// 生成する
			GameObject obj = (GameObject)Instantiate (fruits, new Vector3 (pos_x, pos_y, 0), Quaternion.identity);
			// リストに追加する
			objectList.Add (obj);
		}

		isPaused = false;
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
		}

		if (Input.GetKeyDown ("space")) {
			addForce ();
		}

		drawLineBetweenObject ();

		gameTimer -= Time.deltaTime;
		gameTimerText.text = gameTimer.ToString ();

		if (0.0f >= gameTimer) {
			// タイマーが終わった時の処理
			isPaused = true;
			gameTimerText.text = "Game Over";
			// 必要があればタイマー再びセット
			//timer = 30.f;
		}

	}

	void OnDragStart ()
	{
		// クリックした時の処理
		if (Input.GetMouseButtonDown (0)) {
			// クリックした位置のオブジェクトを取得する
			GameObject obj = GetCurrentHitObject ();
			if (obj != null) {
				// 最初に選択したオブジェクトを保持
				firstObject = obj; 
				fruits_distance = obj.GetComponent<SpriteRenderer> ().bounds.size.x * 1.05f;
				Debug.Log ("float:" + fruits_distance);
				currentObjectName = firstObject.name;
				// リストに追加
				PushToList (obj);

				// 対象のもの以外のカラーを変更
//				foreach( GameObject mObj in objectList){
//					if(mObj.name != currentObjectName){
//						//colorChange(mObj);
//					}
//				}
			}
		}
	}

	void colorChange (GameObject obj)
	{
		obj.GetComponent<SpriteRenderer> ().color = new Color (0.3f, 0.3f, 0.3f, 1.0f);
	}

	void colorReset ()
	{
		foreach (GameObject mObj in objectList) {
			mObj.GetComponent<SpriteRenderer> ().color = new Color (1.0f, 1.0f, 1.0f, 1.0f);
		}
	}

	void OnDragEnd ()
	{

		if (removableObjectList.Count >= 3) {
			// 全部削除
			foreach (GameObject obj in removableObjectList) {
				// 削除
				objectList.Remove (obj);
				Destroy (obj);
			}

			if (removableObjectList.Count >= 7) {
				// スペシャルを追加
				GameObject spObj = (GameObject)Instantiate 
					(specialObjectList [0], new Vector3 (lastObject.transform.position.x, lastObject.transform.position.y, 0), Quaternion.identity);
				objectList.Add (spObj);
				// 削除した分を追加する
				DropBall (removableObjectList.Count - 1);
			} else {
				// 削除した分を追加する
				DropBall (removableObjectList.Count);
			}

		} else {
//			foreach (GameObject obj in removableObjectList) {
//				// 削除
//				//obj.transform.localScale = new Vector3 (0.3f, 0.3f, 1.0f);
//			}
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
	}

	//
	int fib (int n)
	{
		int f0;
		int f1;
		int f2;
		
		f0 = 0;
		f1 = 1;
		f2 = 0;

		while (f1<n) {
			// フィボナッチ数の出力(n>0)
			Debug.Log ("fib:" + f1);
			// フィボナッチ数の計算
			f2 = f1 + f0;
			// 変数の代入
			f0 = f1;
			f1 = f2;
		}

//		for(int i=0;i<n;i++){
//			kekka += i;
//			Debug.Log ("fib:" + kekka);
//		}
		 
		return f2;
	}

	void OnDragging ()
	{
		// 現在選択中のオブジェクト
		currentObject = GetCurrentHitObject ();

		if (currentObject != null) {
			// 最初のオブジェクトと名前が一致するか
			if (currentObject.name != firstObject.name)
				return; // 一致しない
			// 直前に追加したものと一致するか
			if (currentObject == lastObject)
				return; // 直前のものと一致
			// すでに追加済みのものは無視
			if (removableObjectList.Contains (currentObject))
				return; // 追加済み

			// 距離
			float dist = Vector2.Distance (currentObject.transform.position, lastObject.transform.position);
			//直前のボールと現在のボールの距離を計算
			if (dist <= fruits_distance) {
				//ボール間の距離が一定値以下のとき
				PushToList (currentObject); //消去するリストにボールを追加
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
			//rb.AddForce(Vector2.up * 200);
			float power_x = Random.Range (-15.0f, 15.0f);
			float power_y = Random.Range (3.0f, 10.0f);
			rb.AddForce (new Vector2 (power_x, power_y) * 800);
		}
	}
}
