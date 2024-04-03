using System;
using System.Collections;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyScreen : MonoBehaviour
{
  [SerializeField] private AudioMixerSnapshot audioMixerSceneTransitionBGMOff;

  [SerializeField] private GameObject startScreen;
  [SerializeField] private GameObject lobbyScreen;

  [SerializeField] private Sprite p1NotReadySprite;
  [SerializeField] private Sprite p1ReadySprite;
  [SerializeField] private Sprite p2NotReadySprite;
  [SerializeField] private Sprite p2ReadySprite;

  [SerializeField] private AudioClip isReadySound;
  [SerializeField] private AudioClip isNotReadySound;

  private GameObject rulesGameObject;
  private GameObject playersGameObject;
  private GameObject readyButtonGameObject;
  private GameObject timerTransitionGameObject;

  private NetworkManager networkManager;

  private bool areHandlersRegistered = false;

  private void Awake()
  {
    rulesGameObject = transform.Find("Rules").gameObject;
    playersGameObject = transform.Find("Players").gameObject;
    readyButtonGameObject = transform.Find("ButtonReady").gameObject;
    timerTransitionGameObject = transform.Find("TransitionTimer").gameObject;
  }

  public void Start()
  {
    networkManager = NetworkManager.instance;
    registerNetworkHandlers();

    InitializeScreen();
  }

  public void OnEnable()
  {
    InitializeScreen();
  }

  public void InitializeScreen()
  {
    readyButtonGameObject.SetActive(true);
    playersGameObject.transform.Find("P1").gameObject.SetActive(false);
    playersGameObject.transform.Find("P1Waiting").gameObject.SetActive(true);
    playersGameObject.transform.Find("P1/Nickname").GetComponent<TextMeshProUGUI>().text = "";

    playersGameObject.transform.Find("P2").gameObject.SetActive(false);
    playersGameObject.transform.Find("P2Waiting").gameObject.SetActive(true);
    playersGameObject.transform.Find("P2/Nickname").GetComponent<TextMeshProUGUI>().text = "";

    readyButtonGameObject.GetComponent<Button>().Select();
    readyButtonGameObject.transform.Find("Label").GetComponent<TextMeshProUGUI>().text = "Ready!";

    displayDifficultySettings(PlayerDataStorage.chosenDifficulty);

    Player localPlayer = PlayerDataStorage.getLocalPlayer();

    Sprite localPlayerSprite = localPlayer.Assignment == PlayerAssignment.Player1 ?
      (localPlayer.IsReady ? p1ReadySprite : p1NotReadySprite)
      : (localPlayer.IsReady ? p2ReadySprite : p2NotReadySprite);

    playersGameObject.transform.Find(getPlayerGameObjectName(localPlayer)).gameObject.SetActive(true);
    playersGameObject.transform.Find(getPlayerGameObjectName(localPlayer) + "Waiting").gameObject.SetActive(false);
    playersGameObject.transform.Find(getPlayerGameObjectName(localPlayer) + "/Nickname").GetComponent<TextMeshProUGUI>().text = localPlayer.Nickname;
    playersGameObject.transform.Find(getPlayerGameObjectName(localPlayer) + "/Sprite").GetComponent<Image>().sprite = localPlayerSprite;
    readyButtonGameObject.transform.Find("Label").GetComponent<TextMeshProUGUI>().text = localPlayer.IsReady ? "Cancel" : "Ready!";

    PlayerAssignment otherPlayerAssignment = localPlayer.Assignment == PlayerAssignment.Player1 ? PlayerAssignment.Player2 : PlayerAssignment.Player1;
    if (PlayerDataStorage.players.ContainsKey(otherPlayerAssignment))
    {
      Player otherPlayer = PlayerDataStorage.players[otherPlayerAssignment];
      Sprite otherPlayerSprite = otherPlayer.Assignment == PlayerAssignment.Player1 ?
        (otherPlayer.IsReady ? p1ReadySprite : p1NotReadySprite)
        : (otherPlayer.IsReady ? p2ReadySprite : p2NotReadySprite);
      playersGameObject.transform.Find(getPlayerGameObjectName(otherPlayer)).gameObject.SetActive(true);
      playersGameObject.transform.Find(getPlayerGameObjectName(otherPlayer) + "Waiting").gameObject.SetActive(false);
      playersGameObject.transform.Find(getPlayerGameObjectName(otherPlayer) + "/Nickname").GetComponent<TextMeshProUGUI>().text = otherPlayer.Nickname;
      playersGameObject.transform.Find(getPlayerGameObjectName(otherPlayer) + "/Sprite").GetComponent<Image>().sprite = otherPlayerSprite;
    }
  }

  public async void OnClickReady()
  {
    Player localPlayer = PlayerDataStorage.getLocalPlayer();
    Func<Task> notifyServer = localPlayer.IsReady ? networkManager.NotifyServerPlayerIsNotReady : networkManager.NotifyServerPlayerIsReady;
    await notifyServer();
  }

  public void PlayerJoinedRoomMessageHandler(PlayerJoinedRoomMessage message)
  {
    Player player = new Player(message.player.nickname, message.player.assignment, false);
    PlayerDataStorage.players[player.Assignment] = player;

    Sprite playerSprite = player.Assignment == PlayerAssignment.Player1 ? p1NotReadySprite : p2NotReadySprite;
    playersGameObject.transform.Find(getPlayerGameObjectName(player)).gameObject.SetActive(true);
    playersGameObject.transform.Find(getPlayerGameObjectName(player) + "Waiting").gameObject.SetActive(false);
    playersGameObject.transform.Find(getPlayerGameObjectName(player) + "/Nickname").GetComponent<TextMeshProUGUI>().text = player.Nickname;
    playersGameObject.transform.Find(getPlayerGameObjectName(player) + "/Sprite").GetComponent<Image>().sprite = playerSprite;
  }

  public void PlayerLeftRoomMessageHandler(PlayerLeftRoomMessage message)
  {
    if (!PlayerDataStorage.players.ContainsKey(message.playerAssignment))
      return;

    Player player = PlayerDataStorage.players[message.playerAssignment];
    Sprite playerSprite = player.Assignment == PlayerAssignment.Player1 ? p1NotReadySprite : p2NotReadySprite;
    playersGameObject.transform.Find(getPlayerGameObjectName(player)).gameObject.SetActive(false);
    playersGameObject.transform.Find(getPlayerGameObjectName(player) + "Waiting").gameObject.SetActive(true);
    playersGameObject.transform.Find(getPlayerGameObjectName(player) + "/Nickname").GetComponent<TextMeshProUGUI>().text = "";
    playersGameObject.transform.Find(getPlayerGameObjectName(player) + "/Sprite").GetComponent<Image>().sprite = playerSprite;
    PlayerDataStorage.players.Remove(message.playerAssignment);
  }


  public void PlayerReadyStateChangedMessageHandler(PlayerReadyStateChangedMessage message)
  {
    Player player = PlayerDataStorage.players[message.playerAssignment];
    player.IsReady = message.isReady;

    Sprite sprite =
      player.Assignment == PlayerAssignment.Player1 ?
        (player.IsReady ? p1ReadySprite : p1NotReadySprite)
        : (player.IsReady ? p2ReadySprite : p2NotReadySprite);

    GetComponent<AudioSource>().PlayOneShot(player.IsReady ? isReadySound : isNotReadySound);

    playersGameObject.transform.Find(getPlayerGameObjectName(player) + "/Sprite").GetComponent<Image>().sprite = sprite;
    if (player.IsLocalPlayer)
    {
      readyButtonGameObject.transform.Find("Label").GetComponent<TextMeshProUGUI>().text = player.IsReady ? "Cancel" : "Ready!";
    }
  }

  public void serverNotifyPlayerDisconnected(PlayerAssignment pa)
  {
    Player player = PlayerDataStorage.players[pa];
    playersGameObject.transform.Find(getPlayerGameObjectName(player) + "/Nickname").GetComponent<TextMeshProUGUI>().text = "";
    playersGameObject.transform.Find(getPlayerGameObjectName(player) + "/Sprite").GetComponent<Image>().sprite = pa == PlayerAssignment.Player1 ? p1NotReadySprite : p2NotReadySprite;
    playersGameObject.transform.Find(getPlayerGameObjectName(player)).gameObject.SetActive(false);
    playersGameObject.transform.Find(getPlayerGameObjectName(player) + "Waiting").gameObject.SetActive(true);
    PlayerDataStorage.players.Remove(pa);
  }

  public void AllReadyGameStartingMessageHandler(AllReadyGameStartingMessage message)
  {
    StartCoroutine(timedTransitionToGameplay());
  }

  private string getPlayerGameObjectName(Player player)
  {
    return player.Assignment == PlayerAssignment.Player1 ? "P1" : "P2";
  }

  private IEnumerator timedTransitionToGameplay()
  {
    readyButtonGameObject.SetActive(false);
    yield return new WaitForSecondsRealtime(0.2f);
    timerTransitionGameObject.SetActive(true);

    int timerValue = 3;
    audioMixerSceneTransitionBGMOff.TransitionTo(timerValue);
    while (timerValue > 0)
    {
      if (PlayerDataStorage.players.Count != 2)
      {
        timerTransitionGameObject.SetActive(false);
        readyButtonGameObject.SetActive(true);
        yield break;
      }
      timerTransitionGameObject.GetComponent<TextMeshProUGUI>().text = timerValue.ToString();
      yield return new WaitForSecondsRealtime(1);
      timerValue--;
    }
    SceneManager.LoadSceneAsync("GameplayScene");
  }

  private void displayDifficultySettings(Difficulty difficulty)
  {
    switch (difficulty)
    {
      case Difficulty.EASY:
        rulesGameObject.transform.Find("DifficultyContainer/Easy").gameObject.SetActive(true);
        break;
      case Difficulty.MEDIUM:
        rulesGameObject.transform.Find("DifficultyContainer/Medium").gameObject.SetActive(true);
        break;
      case Difficulty.HARD:
        rulesGameObject.transform.Find("DifficultyContainer/Hard").gameObject.SetActive(true);
        break;
    }

    Transform ControllerButtons = rulesGameObject.transform.Find("ControllerButtons");

    for (int i = 0; i < 4; i++)
    {
      bool shouldDisplayButton = i < GameSettings.NbButtonsPerDifficulty[difficulty];
      ControllerButtons.GetChild(i).gameObject.SetActive(shouldDisplayButton);
    }
  }

  public void onClickLeaveLobby()
  {
    _ = networkManager.DisconnectFromServerAsync();
    resetBackToStartScreen();
  }

  private void OnDisconnectFromServerHandler()
  {
    resetBackToStartScreen();
  }

  private void resetBackToStartScreen()
  {
    PlayerDataStorage.reset();
    startScreen.SetActive(true);
    gameObject.SetActive(false);
  }

  private void OnDestroy()
  {
    unregisterNetworkHandlers();
  }

  private void registerNetworkHandlers()
  {
    if (!areHandlersRegistered)
    {
      networkManager.OnDisconnect += OnDisconnectFromServerHandler;

      networkManager.OnPlayerJoinedRoomMessage += PlayerJoinedRoomMessageHandler;
      networkManager.OnPlayerLeftRoomMessage += PlayerLeftRoomMessageHandler;
      networkManager.OnPlayerReadyStateChangedMessage += PlayerReadyStateChangedMessageHandler;
      networkManager.OnAllReadyGameStartingMessage += AllReadyGameStartingMessageHandler;
      areHandlersRegistered = true;
    }
  }

  private void unregisterNetworkHandlers()
  {
    if (areHandlersRegistered)
    {
      networkManager.OnDisconnect -= OnDisconnectFromServerHandler;

      networkManager.OnPlayerJoinedRoomMessage -= PlayerJoinedRoomMessageHandler;
      networkManager.OnPlayerLeftRoomMessage -= PlayerLeftRoomMessageHandler;
      networkManager.OnPlayerReadyStateChangedMessage -= PlayerReadyStateChangedMessageHandler;
      networkManager.OnAllReadyGameStartingMessage -= AllReadyGameStartingMessageHandler;
      areHandlersRegistered = false;
    }
  }
}
