using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JoinGameScreen : MonoBehaviour
{
  [SerializeField] private GameObject startScreen;
  [SerializeField] private GameObject lobbyScreen;

  private TMP_InputField NicknameInputField;

  private NetworkManager networkManager;

  private bool areHandlersRegistered = false;

  private void Awake()
  {
    PlayerDataStorage.chosenDifficulty = Difficulty.MEDIUM;
    transform.Find("Rules/DifficultyContainer/Medium").GetComponent<Button>().Select();
    displayDifficultyAsSet();

    NicknameInputField = transform.Find("InputFieldOverlay/InputFieldNickname").GetComponent<TMP_InputField>();
  }

  private void Start()
  {
    networkManager = NetworkManager.instance;
    registerNetworkHandlers();

    transform.Find("Rules/DifficultyContainer/Medium").GetComponent<Button>().Select();
  }

  public async void OnClickJoinLobby()
  {
    string localNickname = NicknameInputField.text;
    if (localNickname.Length == 0)
    {
      Debug.Log("Nickname is required"); // TODO: Display error in UI
      return;
    }
    PlayerDataStorage.localNickname = localNickname;
    await networkManager.ConnectToServerAsync();
    await networkManager.JoinRoom(localNickname, PlayerDataStorage.chosenDifficulty);
  }

  public void OnClickSetDifficultyEasy()
  {
    PlayerDataStorage.chosenDifficulty = Difficulty.EASY;
    displayDifficultyAsSet();
  }

  public void OnClickSetDifficultyMedium()
  {
    PlayerDataStorage.chosenDifficulty = Difficulty.MEDIUM;
    displayDifficultyAsSet();
  }

  public void OnClickSetDifficultyHard()
  {
    PlayerDataStorage.chosenDifficulty = Difficulty.HARD;
    displayDifficultyAsSet();
  }

  public void OnSelectDisplayDifficultyEasy()
  {
    displayDifficultySettings(Difficulty.EASY);
  }
  public void OnSelectDisplayDifficultyMedium()
  {
    displayDifficultySettings(Difficulty.MEDIUM);
  }
  public void OnSelectDisplayDifficultyHard()
  {
    displayDifficultySettings(Difficulty.HARD);
  }

  public void displayDifficultyAsSet()
  {
    displayDifficultySettings(PlayerDataStorage.chosenDifficulty);
  }

  private void displayDifficultySettings(Difficulty difficulty)
  {
    Transform ControllerButtons = transform.Find("Rules/ControllerButtons");

    for (int i = 0; i < 4; i++)
    {
      bool shouldDisplayButton = i < GameSettings.NbButtonsPerDifficulty[difficulty];
      ControllerButtons.GetChild(i).gameObject.SetActive(shouldDisplayButton);
    }
  }

  public void onClickBackToMenu()
  {
    startScreen.SetActive(true);
    gameObject.SetActive(false);
  }

  private void RoomJoinedMessageHandler(RoomJoinedMessage message)
  {
    Player localPlayer = new Player(PlayerDataStorage.localNickname, message.playerAssignment, true);
    PlayerDataStorage.players[localPlayer.Assignment] = localPlayer;

    if (message.otherPlayer != null)
    {
      Player otherPlayer = new Player(message.otherPlayer.nickname, message.otherPlayer.assignment, false)
      {
        IsReady = message.otherPlayer.isReady
      };
      PlayerDataStorage.players[otherPlayer.Assignment] = otherPlayer;
    }

    lobbyScreen.SetActive(true);
    gameObject.SetActive(false);
  }

  private void FailureToConnectHandler()
  {
    // TODO: Error modal
    // On "join a game" click, should see a loading animation
    // then either it works, transition to next screen
    // or it fails with an error modal (good enough)
    Debug.Log("Failure to connect to the server");
  }

  private void registerNetworkHandlers()
  {
    if (!areHandlersRegistered)
    {
      networkManager.OnFailureToConnect += FailureToConnectHandler;
      networkManager.OnRoomJoinedMessage += RoomJoinedMessageHandler;
      areHandlersRegistered = true;
    }
  }

  private void unregisterNetworkHandlers()
  {
    if (areHandlersRegistered)
    {
      networkManager.OnFailureToConnect -= FailureToConnectHandler;
      networkManager.OnRoomJoinedMessage -= RoomJoinedMessageHandler;
      areHandlersRegistered = false;
    }
  }

  private void OnDestroy()
  {
    unregisterNetworkHandlers();
  }
}

