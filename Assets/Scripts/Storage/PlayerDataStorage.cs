using System;
using System.Collections.Generic;
using System.Linq;

public static class PlayerDataStorage
{
  public static string localNickname;
  private static Difficulty defaultDifficulty = Difficulty.MEDIUM;

  public static Difficulty chosenDifficulty = defaultDifficulty;
  public static Dictionary<PlayerAssignment, Player> players = new Dictionary<PlayerAssignment, Player>();

  public static Player getLocalPlayer()
  {
    return Array.Find(players.Values.ToArray(), (player) => player.IsLocalPlayer);
  }

  public static void reset()
  {
    players.Clear();
    chosenDifficulty = defaultDifficulty;
    localNickname = "";
  }
}
