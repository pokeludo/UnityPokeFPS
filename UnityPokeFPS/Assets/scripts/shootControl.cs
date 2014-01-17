using UnityEngine;
using System.Collections;

public class shootControl : MonoBehaviour {

	public int playerLife = 20; //プレイヤープレハブ
	public float fireRate = 0.30f;  //連射速度
	public GameObject impactPrefab;
	public bool canFire = true;
	public float damageHit = 4; //力
	public AudioClip shootSound;

	private int gainPoint = 0;
	private GameObject menuScore;
	private GameObject hitPointScore;
	private Quaternion tempRot;

	// Use this for initialization
	void Start () {
		Screen.showCursor = false;

		menuScore = GameObject.Find("ScorePoints");
		hitPointScore = GameObject.Find("ScoreHitPoints");

		GUIText menuGUI = menuScore.GetComponent<GUIText>();
		menuGUI.text = "スコア : " + gainPoint;
		GUIText hitPointGUI = hitPointScore.GetComponent<GUIText>();
		hitPointGUI.text = "ライフ : " + playerLife;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButton("Fire1")) {
			fire();
		}
	}

	//スコラ追加
	public void ApplyPoints(int points)  {
		gainPoint = gainPoint + points;
		GUIText menuGUI = menuScore.GetComponent<GUIText>();
		menuGUI.text = "スコア : " + gainPoint;
	}

	//ダメージを受ける
	public void makeDamage(int damage) {
		playerLife -= damage;
		GUIText hitPointGUI = hitPointScore.GetComponent<GUIText>();
		hitPointGUI.text="ライフ : " + playerLife;
		
		if (playerLife<=0){ //ゲームオーバー
			Application.LoadLevel(0);
		}
	}

	//打つ命令
	private void fire() {
		if (canFire == true) {
			canFire = false;
			StartCoroutine(FireOneShot()); 
		}
	}

	//打つ行動
	private IEnumerator FireOneShot(){
		audio.PlayOneShot(shootSound);
		
		Vector3 direction = transform.TransformDirection(Vector3.forward); //前へ打つ
		RaycastHit hit;
		
		if (Physics.Raycast(transform.position, direction, out hit, 100)){ //カメラから前へ光線を発射
			
			tempRot = Quaternion.FromToRotation(Vector3.up, hit.normal);		
			Instantiate(impactPrefab, hit.point, tempRot);
			
			if (hit.rigidbody) { //物理の影響を受ける場合
				hit.rigidbody.AddForce(1000 * direction);
			}
			
			hit.collider.SendMessageUpwards("makeDamage", damageHit, SendMessageOptions.DontRequireReceiver);						
		}
		yield return new WaitForSeconds(fireRate); //連射制御
		
		canFire = true;
	}


}
