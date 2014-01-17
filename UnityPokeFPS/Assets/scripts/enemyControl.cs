using UnityEngine;
using System.Collections;

namespace PokeFPS {

public class enemyControl : MonoBehaviour {

	public int enemyScore = 100; //敵スコア
	public int enemyLife = 20; //敵ライフ
	public Transform explosionEffect; //爆発プレハブ

	public Transform missile1; //発射
	public Transform missile2; //発射
	public GameObject missilePrefab; //ミサイルプレハブ
	
	public bool isShooting; //今発射している最中かどうか
	public LayerMask enemyLayer; //敵レイヤー

	public int gravity = 20; //重力
	public float visionAngle = 45; //45度の角度
	public float visionRange = 100; //視覚範囲

	private GameObject playerPrefab; //プレイヤープレハブ
	private float distanceToPlayer; //プレイヤーと敵の間の距離
	private Vector3 directionToPlayer; //プレイヤーと敵の間のベクトル
	private bool canSeePlayer = false; //プレイヤーが見られたかどうか
	private RaycastHit hit; //光線

	// Use this for initialization
	void Start () {
		playerPrefab=GameObject.Find("Player");
	}
	
	// Update is called once per frame
	void Update () {
		seekPlayer();
	}

	//プレイヤーを探す
	private void seekPlayer() {
		Vector3 moveDirection=Vector3.zero; //敵の方向
		directionToPlayer=playerPrefab.transform.position-transform.position;
		distanceToPlayer=Vector3.Distance(transform.position,playerPrefab.transform.position);

		float playerAngle=Vector3.Angle(directionToPlayer,transform.forward); //敵の位置からプレイヤーがいる位置の角度
		
		if(((Mathf.Abs(playerAngle) < visionAngle) && distanceToPlayer < visionRange) || (canSeePlayer == true)){ //角度と距離が合っていれば、またはプレイヤーが見られたことがある
			Physics.Linecast(transform.position, playerPrefab.transform.position, out hit, ~enemyLayer.value); //敵レイヤーを無視して、敵からプレイヤーへの光線を発射する
				
			if (hit.collider.name==playerPrefab.collider.name){ //プレイヤーが打たれた場合
				moveDirection=transform.forward; //手前の方向
				var tempRot=Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(directionToPlayer), 2*Time.deltaTime); //プレイヤーへ敵の回転の計算
				tempRot.x=0;
				tempRot.z=0;
				transform.rotation=tempRot; //回転はy軸だけによる
				canSeePlayer=true; //プレイヤーが見られた
				StartCoroutine(shootMissile());
			}
		}

		moveDirection.y-=gravity*Time.deltaTime; //y軸による重力
		CharacterController controller= (CharacterController)GetComponent("CharacterController");
		controller.Move(moveDirection*2*Time.deltaTime);
	}


	private IEnumerator shootMissile(){
		if (isShooting==false){ //打っている最中ではない場合
			isShooting=true;
			Instantiate (missilePrefab, missile1.position, missile1.rotation); //ミサイル1
			Instantiate (missilePrefab, missile2.position, missile2.rotation); //ミサイル2
			yield return new WaitForSeconds(2);	 //休憩２秒
			isShooting=false; //発射過程終了
		}
	}

	//敵に傷を付ける
	public void makeDamage(int damage){
		enemyLife-=damage; //ダメージを与える
		
		if (enemyLife<=0){	//敵が殺された場合	
			Instantiate(explosionEffect, transform.position, transform.rotation); //爆発
			Camera.main.SendMessageUpwards("ApplyPoints", enemyScore, SendMessageOptions.DontRequireReceiver); //GUIアップデート
				
			Destroy(gameObject); //絶滅
		}
	}

}

}