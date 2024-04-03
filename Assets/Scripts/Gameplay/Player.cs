public enum PlayerAssignment
{
  Player1,
  Player2
}

public class Player
{
  public Player(string nickname, PlayerAssignment assignment, bool isLocalPlayer)
  {
    Nickname = nickname;
    Assignment = assignment;
    IsLocalPlayer = isLocalPlayer;
    IsReady = false;
    IsReadyToReplay = false;
  }

  public PlayerAssignment Assignment { get; }
  public string Nickname { get; }
  public bool IsLocalPlayer { get; }
  public bool IsReady { get; set; }
  public bool IsReadyToReplay { get; set; }
}
