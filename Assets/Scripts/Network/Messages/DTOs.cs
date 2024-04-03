using System;
using Newtonsoft.Json;

[Serializable]
public class LobbyPlayerDTO
{
  public string nickname;
  public PlayerAssignment assignment;
  public bool isReady;
};

[Serializable]
public class GameResultPlayerDTO
{
  public PlayerAssignment assignment;

  [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
  public ControllerButton? inputPressed;

  [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
  public int? timeToPress;
};

[Serializable]
public class GameResultDTO
{
  [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
  public PlayerAssignment? winningPlayer;
  public GameResultPlayerDTO player1;
  public GameResultPlayerDTO player2;
};
