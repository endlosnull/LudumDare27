﻿using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {
	
	public GameObject explosionFxPrefab;
	
	public Transform thisTransform;
	public MeshRenderer thisRenderer;
	public Rigidbody thisRigidbody;
	
	public AudioClip explosionClip;
	
	public enum AIType {
		RANDOM,
		PASSIVE,
		RANDOM_PASSIVE,
		AGGRESSIVE,
		RANDOM_AGGRESSIVE
	}
	public AIType aiType;
	
	void Awake() {
		thisRenderer.material.color = new Color((float)Random.Range(0f, 1f), (float)Random.Range(0f, 1f), (float)Random.Range(0f, 1f));
		aiType = (AIType)Random.Range((int)AIType.RANDOM, (int)AIType.RANDOM_AGGRESSIVE + 1);
	}
	
	void FixedUpdate() {
		int mapWidth = Map.Instance.handler.MapWidth;
		int mapHeight = Map.Instance.handler.MapHeight;
		
		if (thisTransform.position.x < -mapWidth / 2 || thisTransform.position.x > mapWidth / 2 || 
			thisTransform.position.y < 0 || thisTransform.position.y > 13 ||
			thisTransform.position.z < -mapHeight / 2 || thisTransform.position.z > mapHeight / 2) {
			thisTransform.position = new Vector3((float)Random.Range(-(mapWidth - 1) / 2, (mapWidth - 1) / 2), (float)Random.Range(1f, 12f), (float)Random.Range(-(mapHeight - 1) / 2, (mapHeight - 1) / 2));
			return;
		}
		
		Player player = PlayerManager.Instance.CurPlayer;
		bool doRandom = false;
		if (player != null) {
			Vector3 dirToPlayer = player.thisTransform.position - thisTransform.position;
			bool doMove = false;
			float distanceCheck = 10f;
			if (aiType == AIType.PASSIVE || aiType == AIType.RANDOM_PASSIVE) {
				distanceCheck = 5f;
			}
			if (dirToPlayer.magnitude < distanceCheck) {
				doMove = true;
			}
			else {
				int result = Random.Range(0, 100);
				if (result < 10) {
					doMove = true;
				}
				else {
					doRandom = true;
				}
			}
			
			if (doMove) {
				dirToPlayer = dirToPlayer.normalized;
				if (aiType == AIType.PASSIVE || aiType == AIType.RANDOM_PASSIVE) {
					thisRigidbody.AddForce(dirToPlayer * -1 * (float)Random.Range(10f, 50f));
				}
				if (aiType == AIType.AGGRESSIVE || aiType == AIType.RANDOM_AGGRESSIVE) {
					thisRigidbody.AddForce(dirToPlayer * (float)Random.Range(10f, 50f));
				}
			}
		}
		
		if (doRandom || aiType == AIType.RANDOM || aiType == AIType.RANDOM_PASSIVE || aiType == AIType.RANDOM_AGGRESSIVE) {
			thisRigidbody.AddForce(new Vector3((float)Random.Range(-50f, 50f), (float)Random.Range(-50f, 50f), (float)Random.Range(-50f, 50f)));
		}
	}
	
	void Die() {
		GameObject.Destroy(gameObject);
		EnemyManager.Instance.RemoveEnemy(this);
	}
	
	public void OnCollisionEnter(Collision collision) {
		if (collision.collider == PlayerManager.Instance.CurPlayer.thisCollider) {
			PlayerManager.Instance.CurPlayer.DealDamage(10f);
			Die();
		}
		else if (collision.collider.tag == "Bullet") {
			PlayerManager.Instance.CurPlayer.score += 10;
			AudioSource.PlayClipAtPoint(explosionClip, thisTransform.position);
			GameObject newGO = (GameObject)Instantiate(explosionFxPrefab, Vector3.zero, Quaternion.identity);
			newGO.transform.position = thisTransform.position;
			ParticleSystem system = newGO.GetComponent<ParticleSystem>();
			if (system != null) {
				system.renderer.material.color = thisRenderer.material.color;
			}
			Die();
		}
	}
	
}
