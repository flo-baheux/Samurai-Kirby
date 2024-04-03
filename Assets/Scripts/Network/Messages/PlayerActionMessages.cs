using System;

public enum PlayerActionMessageType
{
  JOIN_GAME = 0,
  SET_READY_STATE = 1,
  NOTIFY_CAN_PLAY = 2,
  PLAYER_INPUT = 3,
  SET_REPLAY_READY_STATE = 4,
  LEAVE_GAME = 5,
};

[Serializable]
public class JoinGamePlayerActionMessage
{
  public JoinGamePlayerActionMessage(string nickname, Difficulty difficulty)
  {
    this.nickname = nickname;
    this.difficulty = difficulty;
  }

  public readonly PlayerActionMessageType messageType = PlayerActionMessageType.JOIN_GAME;
  public readonly string nickname;
  public readonly Difficulty difficulty;
};

[Serializable]
public class SetReadyStatePlayerActionMessage
{
  public SetReadyStatePlayerActionMessage(bool isReady)
  {
    this.isReady = isReady;
  }

  public readonly PlayerActionMessageType messageType = PlayerActionMessageType.SET_READY_STATE;

  public readonly bool isReady;
};

[Serializable]
public class NotifyCanPlayPlayerActionMessage
{
  public readonly PlayerActionMessageType messageType = PlayerActionMessageType.NOTIFY_CAN_PLAY;
};

[Serializable]
public class InputPlayerActionMessage
{
  public InputPlayerActionMessage(ControllerButton inputPressed, int timeToPress)
  {
    this.inputPressed = inputPressed;
    this.timeToPress = timeToPress;
  }

  public readonly PlayerActionMessageType messageType = PlayerActionMessageType.PLAYER_INPUT;

  public readonly ControllerButton inputPressed;
  public readonly int timeToPress;
};

[Serializable]
public class SetReplayReadyStatePlayerActionMessage
{
  public SetReplayReadyStatePlayerActionMessage(bool isReadyToReplay)
  {
    this.isReadyToReplay = isReadyToReplay;
  }

  public readonly PlayerActionMessageType messageType = PlayerActionMessageType.SET_REPLAY_READY_STATE;

  public readonly bool isReadyToReplay;
};

[Serializable]
public class LeaveGamePlayerActionMessage
{
  public readonly PlayerActionMessageType messageType = PlayerActionMessageType.LEAVE_GAME;
};
