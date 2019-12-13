using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveManager : MonoBehaviour
{
	[SerializeField] private float objectiveSqrDistTreshhold = 1f;
	[SerializeField] private GameObject gameOverScreen;

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

			goalIndicator.goal = ship;
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
		{
			s.enabled = false;
		}
		
		//Enabled spawning of blackholes
		blackHoleSpawner.enabled = true;

		//TODO: ZoomOut camera

		//Spawn final destination
		SpawnPrefabCloseToPlayer(finalDestinationPrefab, ref finalDest, shipSpawnRangeFromPlayer * 3);
		goalIndicator.goal = finalDest;
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
}
