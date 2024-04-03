using System;

public enum GameplayMessageType
{
  ROOM_JOINED = 0,
  PLAYER_JOINED_GAME = 1,
  PLAYER_LEFT_GAME = 2,
  PLAYER_READY_STATE_CHANGED = 3,
  ALL_READY_GAME_STARTING = 4,
  GAME_EXPECTING_INPUT = 5,
  GAME_OVER_EARLY_INPUT = 6,
  GAME_OVER_WRONG_INPUT = 7,
  GAME_OVER_WIN = 8,
  GAME_OVER_DRAW = 9,
  PLAYER_REPLAY_READY_STATE_CHANGED = 10,
  ALL_REPLAY_READY_GAME_STARTING = 11,
};

[Serializable]
public class GameplayMessage
{
  public GameplayMessageType messageType;
};

[Serializable]
public class RoomJoinedMessage : GameplayMessage
{
  public PlayerAssignment playerAssignment;
  public LobbyPlayerDTO otherPlayer;
};

[Serializable]
public class PlayerJoinedRoomMessage : GameplayMessage
{
  public LobbyPlayerDTO player;
};

[Serializable]
public class PlayerLeftRoomMessage : GameplayMessage
{
  public PlayerAssignment playerAssignment;
};

[Serializable]
public class PlayerReadyStateChangedMessage : GameplayMessage
{
  public PlayerAssignment playerAssignment;
  public bool isReady;
};

[Serializable]
public class AllReadyGameStartingMessage : GameplayMessage { };

[Serializable]
public class GameExpectingInputMessage : GameplayMessage
{
  public ControllerButton expectedInput;
};

[Serializable]
public class GameOverEarlyInputMessage : GameplayMessage
{
  public GameResultDTO gameResult;
};

[Serializable]
public class GameOverWrongInputMessage : GameplayMessage
{
  public GameResultDTO gameResult;
};

[Serializable]
public class GameOverWinMessage : GameplayMessage
{
  public GameResultDTO gameResult;
};

[Serializable]
public class GameOverDrawMessage : GameplayMessage
{
  public GameResultDTO gameResult;
};

[Serializable]
public class PlayerReplayReadyStateChangedMessage : GameplayMessage
{
  public PlayerAssignment playerAssignment;
  public bool isReadyToReplay;
};

[Serializable]
public class AllReplayReadyGameStartingMessage : GameplayMessage
{ };
