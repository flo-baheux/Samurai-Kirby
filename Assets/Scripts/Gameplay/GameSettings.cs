using System.Collections.Generic;

public enum Difficulty
{
  EASY,
  MEDIUM,
  HARD
};

public static class GameSettings
{
  public static readonly Difficulty defaultDifficulty = Difficulty.MEDIUM;

  public static Dictionary<Difficulty, int> NbButtonsPerDifficulty = new Dictionary<Difficulty, int> {
    {Difficulty.EASY, 1},
    {Difficulty.MEDIUM, 2},
    {Difficulty.HARD, 4},
  };
}
