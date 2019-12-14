using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ObjectiveManager : MonoBehaviour
{
	[SerializeField] private GameObject gameOverScreen;
	[SerializeField] private CinemachineVirtualCamera CMCam;
	
	[Header("Comet Riding Phase")]
	[SerializeField] private GameObject shipPrefab;
	[SerializeField] private Vector2 shipSpawnRangeFromPlayer = new Vector2(30f, 60f);
	
	[Header("Ship Phase")]
	[SerializeField] private ScreenEdgeSpawner blackHoleSpawner;
	[SerializeField] private GameObject finalDestinationPrefab;	

	private EMatchState matchState = EMatchState.RepairingShip;
	private Player player;
	private GoalIndicator goalIndicator;
	private Transform ship;
	private Transform finalDest;
	private ScreenEdgeSpawner[] spawners;

	private int neededParts;

    // Start is called before the first frame update
    void Start()
    {
        player = Object.FindObjectOfType<Player>();

		//Suscribe to player events
		player.playerDeathEvent += OnPlayerDeath;
		player.playerBoardedShipEvent += OnPlayerBoardedShip;
		player.playerReachedExitEvent += OnPlayerEscaped;

		goalIndicator = player.GetComponent<GoalIndicator>();
		neededParts = player.TotalNeededComps();

		spawners = GetComponentsInChildren<ScreenEdgeSpawner>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (matchState)
		{
			case EMatchState.RepairingShip: RepairingShipStateUpdate();
				break;
			
			case EMatchState.SeekingShip: SeekingShipStateUpdate();
				break;
		}
    }

	private void RepairingShipStateUpdate()
	{
		int obtainedParts = player.TotalObtainedComps();

		if (obtainedParts == neededParts && neededParts != 0) //If player gathered all pieces
		{
			SpawnPrefabCloseToPlayer(shipPrefab, ref ship, shipSpawnRangeFromPlayer);

			matchState = EMatchState.SeekingShip;

			goalIndicator.SetGoal(ship);
			goalIndicator.enabled = true;
		}
	}

	private void SeekingShipStateUpdate()
	{
		int obtainedParts = player.TotalObtainedComps();

		if (obtainedParts != neededParts) //if player lost ship parts on his way to the ship
		{
			matchState = EMatchState.RepairingShip;
			goalIndicator.enabled = false;

			return;
		}
	}

	private void OnPlayerBoardedShip()
	{
		//Player reached ship
		matchState = EMatchState.Escaping;

		//Turn off all spawning
		foreach (ScreenEdgeSpawner s in spawners)
			s.enabled = false;
		//Enable spawning of blackholes
		blackHoleSpawner.enabled = true;

		//TODO: ZoomOut camera
		StartCoroutine(ZoomOutCam(10, 1));

		//Spawn final destination
		SpawnPrefabCloseToPlayer(finalDestinationPrefab, ref finalDest, shipSpawnRangeFromPlayer * 3);
		goalIndicator.SetGoal(finalDest);
		goalIndicator.SetNewBaseObject(ship.transform, ship.GetComponentInChildren<SpriteRenderer>().bounds);
	}

	private void OnPlayerEscaped()
	{
		matchState = EMatchState.GameOver_Win;

		player.allowMovementInput = false;

		//TODO: Show Game Over Winning GUI
	}

	private void OnPlayerDeath()
	{
		player.playerDeathEvent -= OnPlayerDeath;

		matchState = EMatchState.GameOver_Lose;

		//TODO: Show Game Over losing GUI
	}

	private void SpawnPrefabCloseToPlayer(GameObject prefab, ref Transform assignTo, Vector2 distRange)
	{
		if (assignTo || !prefab)
			return;

		float distance = Random.Range(distRange.x, distRange.y);
		Vector2 spawnPos = Random.insideUnitCircle * distance + (Vector2)player.transform.position;
		assignTo = Instantiate(prefab, spawnPos, Quaternion.identity).transform;
	}

	IEnumerator ZoomOutCam(float deltaOrthoSize, float duration)
	{
		float initialSize = CMCam.m_Lens.OrthographicSize;
		float targetSize = initialSize + deltaOrthoSize;
		float animTime = 0;

		while (CMCam.m_Lens.OrthographicSize != targetSize)
		{
			CMCam.m_Lens.OrthographicSize = Mathf.SmoothStep(initialSize, targetSize, animTime / duration);

			animTime += Time.deltaTime;

			yield return null;
		}
		
	}
}
