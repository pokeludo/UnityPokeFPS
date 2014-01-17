using UnityEngine;
using System.Collections;

namespace PokeFPS {

public class missileControl : MonoBehaviour {

	public int damage = 4; //ダメージの値
	public GameObject explosionPrefab; //爆発のプレハブ
	public GameObject particleObject; //パーティクル

	private GameObject playerPrefab;
	

	// Use this for initialization
	void Start () {
		playerPrefab = GameObject.Find("Player");
		StartCoroutine(autoDestruct());
	}
	
	// Update is called once per frame
	void Update () {
		transform.Translate(Vector3.forward*10*Time.deltaTime); //手前へ進む
	}

	private IEnumerator autoDestruct(){
		yield return new WaitForSeconds(10); //発射されたら、ミサイルは10秒の寿命になる
		Destroy(gameObject);
	}

	//接触認識
	public void OnTriggerEnter(Collider other){
		if (other.name == playerPrefab.collider.name){ //プレイヤーとの接触の場合
			Camera.main.SendMessageUpwards("makeDamage", damage, SendMessageOptions.DontRequireReceiver); //GUIアップデート
		}
		particleObject.transform.parent = null;
		particleObject.particleEmitter.emit = false; //パーティクル拡散停止
		Instantiate (explosionPrefab, transform.position, transform.rotation);
		Destroy (gameObject);
	}


}
}
