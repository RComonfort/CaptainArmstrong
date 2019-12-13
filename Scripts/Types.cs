public enum EMovementEntityType
{
    Comet,
	Spaceship,
	DebrisAdrift
}

public enum EPlayerState
{
	OnComet,
	OnSpaceShip,
	Jumping,
	Dead
}

public enum EDestroyPolicy
{
	ByTime,
	ByLeavingScreen
}

public enum EShipComponent
{
	Cog,
	Panel,
	Battery
}

public enum EMatchState
{
	RepairingShip,
	SeekingShip,
	Escaping,
	GameOver_Lose,
	GameOver_Win
}
