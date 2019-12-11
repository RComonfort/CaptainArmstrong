using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class ScreenEdgeSpawner : MonoBehaviour
{
	[SerializeField] private GameObject prefabToSpawn;			//Prefab for the object to spawn
	[SerializeField] private float spawnTime = 5f;				//The time between wave spawns
	[SerializeField] private float delayBetweenSpawns = .3f;	//The delay between each object spawn within a wave
	[SerializeField] private int spawnAmount = 3;				//Amount of objects to spawn per wave
	[SerializeField] private float firstWaveDelay = 1f;			//Delay before the first wave
	
	
	private Camera mainCam;

    // Start is called before the first frame update
    void Start()
    {
		mainCam = Camera.main;

		Assert.IsNotNull(prefabToSpawn, "Must provide a prefab to spawn");

        InvokeRepeating("SpawnWave", firstWaveDelay, spawnTime);
    }

    private void SpawnWave()
	{
		StartCoroutine(SpawnWaveRoutine());
	}

	private IEnumerator SpawnWaveRoutine()
	{
		for (int objI = 0; objI < spawnAmount; objI++)
		{
			Vector3 spawnPos = PickRandomSpawnPos();

			Vector3 screenCenter = mainCam.ScreenToWorldPoint(new Vector3(Screen.width/2f, Screen.height/2f));
			Vector3 dir = (screenCenter - spawnPos).normalized;

			//Spawn objects looking at center of screen
			Quaternion spawnRot = Quaternion.LookRotation(Vector3.forward, Vector3.Cross(Vector3.forward, dir));
			

			//Create obj and offset its rotation randomly
			GameObject newObj = Instantiate(prefabToSpawn, spawnPos, spawnRot);
			newObj.transform.rotation = Quaternion.AngleAxis(Random.Range(-60f, 60f), newObj.transform.right);

			//Move away obj to hide its spawn off-screen
			Collider2D collider = newObj.GetComponent<Collider2D>();
			if (collider)
			{
				Vector3 bounds = collider.bounds.extents;
				bounds.z = 0;

				//push away from screen center by the hypothenuse of the obj's bounds on X and Y
				float magnitude = bounds.magnitude * 3f;
				newObj.transform.Translate(-dir * magnitude, Space.World);
			}

			yield return new WaitForSeconds(delayBetweenSpawns);
		}
	}

	public void ModifySpawnTime(float newTime)
	{
		spawnTime = newTime;
		CancelInvoke();
		InvokeRepeating("SpawnWave", firstWaveDelay, spawnTime);
	}

	private Vector3 PickRandomSpawnPos()
	{
		bool onLateralSide = Random.Range(0, 2) == 0;

		int heightPx, widthPx;
		if (onLateralSide) //if should spawn vertically
		{
			heightPx = Random.Range(0, Screen.height);
			widthPx = Random.Range(0,2) * Screen.width; //spawn either left or right of screen
		}
		else //spawn horizontally
		{
			widthPx = Random.Range(0, Screen.width);
			heightPx = Random.Range(0,2) * Screen.height; //spawn either top or bottom of screen
		}

		Vector3 spawnPoint = mainCam.ScreenToWorldPoint(new Vector3(widthPx, heightPx));
		spawnPoint.z = 0;

		return spawnPoint;
	}
}
