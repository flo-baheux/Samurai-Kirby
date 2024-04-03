using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameplayManager : MonoBehaviour
{
  private enum GameState
  {
    INIT,
    CAN_PLAY,
    GAME_EXPECTS_INPUT,
    GAME_OVER,
  };

  private NetworkManager networkManager;
  private PlayerInput playerInputManager;

  [SerializeField] private GameObject UIGameObject;
  private GameObject AnimatedGameStartUI;

  private GameObject IngameUI;
  private GameObject ButtonToPressGameObject;
  private GameObject AttentionCatcherGameObject;

  private GameObject GameoverUI;
  private GameObject ScoreboardGameObject;
  private GameObject ScoreboardPlayersGameObject;
  private GameObject ScoreboardWinnerText;
  private GameObject GameoverMenu;
  private GameObject GameoverTransitionTimer;

  [SerializeField] private GameObject GameplayGameObject;
  private GameObject GameplayPlayersGameObject;

  [SerializeField] private AudioClip isReadyToReplaySound;
  [SerializeField] private AudioClip isNotReadyToReplaySound;

  [SerializeField] private AudioClip winAudioClip;
  [SerializeField] private AudioClip lossAudioClip;

  private GameState gameState = GameState.INIT;

  private bool areHandlersRegistered = false;

  private float timer = 0.0f;

  private void Awake()
  {
    Application.targetFrameRate = 60;

    // Retrieve gameobjects
    AnimatedGameStartUI = UIGameObject.transform.Find("AnimatedGameStart").gameObject;

    IngameUI = UIGameObject.transform.Find("Ingame").gameObject;
    AttentionCatcherGameObject = IngameUI.transform.Find("AttentionZone/AttentionCatcher").gameObject;
    ButtonToPressGameObject = IngameUI.transform.Find("AttentionZone/ButtonToPress").gameObject;

    GameoverUI = UIGameObject.transform.Find("Gameover").gameObject;
    ScoreboardGameObject = GameoverUI.transform.Find("Scoreboard").gameObject;
    ScoreboardPlayersGameObject = ScoreboardGameObject.transform.Find("Players").gameObject;
    ScoreboardWinnerText = ScoreboardGameObject.transform.Find("WinnerText").gameObject;
    GameoverMenu = GameoverUI.transform.Find("Menu").gameObject;
    GameoverTransitionTimer = GameoverUI.transform.Find("TransitionTimer").gameObject;

    GameplayPlayersGameObject = GameplayGameObject.transform.Find("Players").gameObject;

    // Retrieve components
    playerInputManager = GetComponent<PlayerInput>();

    // Setup scene
    AnimatedGameStartUI.SetActive(true);
    IngameUI.SetActive(false);
    AttentionCatcherGameObject.SetActive(false);
    ButtonToPressGameObject.SetActive(false);

    GameplayGameObject.SetActive(true);
    GameplayPlayersGameObject.SetActive(true);

    GameoverUI.SetActive(false);

    if (PlayerPrefs.HasKey("FlashingEnabled"))
    {
      GameplayGameObject.transform.Find("Flashbang").gameObject.SetActive(PlayerPrefs.GetInt("FlashingEnabled") != 0);
    }
  }

  void Start()
  {
    networkManager = NetworkManager.instance;
    registerNetworkHandlers();

    setGameplayPlayersNickname();
    StartCoroutine(canPlayWhenStartingAnimationIsOver());
  }

  void Update()
  {
    switch (gameState)
    {
      case GameState.CAN_PLAY:
        handlePlayerInputs();
        break;
      case GameState.GAME_EXPECTS_INPUT:
        handlePlayerInputs();
        timer += Time.deltaTime;
        break;
      case GameState.INIT:
      case GameState.GAME_OVER:
      default:
        break;
    }
  }

  public async void handlePlayerInputs()
  {
    ControllerButton buttonPressed;

    if (playerInputManager.actions["A"].triggered)
      buttonPressed = ControllerButton.SOUTH;
    else if (playerInputManager.actions["B"].triggered)
      buttonPressed = ControllerButton.EAST;
    else if (playerInputManager.actions["X"].triggered)
      buttonPressed = ControllerButton.WEST;
    else if (playerInputManager.actions["Y"].triggered)
      buttonPressed = ControllerButton.NORTH;
    else
      return;

    int timerMs = (int)Math.Truncate(timer * 1000);
    await networkManager.NotifyServerPlayerPressedButton(buttonPressed, timerMs);
    GameplayGameObject.GetComponent<Animator>().Play("WinAnimationFirstPart");
    AttentionCatcherGameObject.SetActive(false);
  }

  public void GameExpectingInputMessageHandler(GameExpectingInputMessage message)
  {
    if (gameState == GameState.GAME_OVER)
    {
      return;
    }

    gameState = GameState.GAME_EXPECTS_INPUT;

    ControllerButton controllerButtonToPress = message.expectedInput;

    IngameUI.SetActive(true);
    ButtonToPressGameObject.SetActive(true);
    AttentionCatcherGameObject.SetActive(true);
    ButtonToPressGameObject.GetComponent<UIButtonController>().controllerButton = controllerButtonToPress;
  }

  public void GameOverEarlyInputMessageHandler(GameOverEarlyInputMessage message)
  {
    gameState = GameState.GAME_OVER;
    StartCoroutine(displayWinAnimationThenGameOverUI(message.gameResult));
  }

  public void GameOverWrongInputMessageHandler(GameOverWrongInputMessage message)
  {
    gameState = GameState.GAME_OVER;
    StartCoroutine(displayWinAnimationThenGameOverUI(message.gameResult));
  }

  public void GameOverWinMessageHandler(GameOverWinMessage message)
  {
    gameState = GameState.GAME_OVER;
    StartCoroutine(displayWinAnimationThenGameOverUI(message.gameResult));
  }

  public void GameOverDrawMessageHandler(GameOverDrawMessage message)
  {
    gameState = GameState.GAME_OVER;
    StartCoroutine(displayWinAnimationThenGameOverUI(message.gameResult));
  }

  public void PlayerReplayReadyStateChangedMessageHandler(PlayerReplayReadyStateChangedMessage message)
  {
    PlayerAssignment pa = message.playerAssignment;
    Player player = PlayerDataStorage.players[pa];
    player.IsReadyToReplay = message.isReadyToReplay;

    if (player.IsLocalPlayer)
      GameoverMenu.transform.Find("ButtonReplay/Label").GetComponent<TextMeshProUGUI>().text = player.IsReadyToReplay ? "Cancel" : "Ready";

    GameoverUI.transform.Find("ReplayReady/" + getPlayerGameObjectName(player)).gameObject.SetActive(player.IsReadyToReplay);

    GetComponent<AudioSource>().PlayOneShot(player.IsReadyToReplay ? isReadyToReplaySound : isNotReadyToReplaySound);
  }

  public void AllReplayReadyGameStartingMessageHandler(AllReplayReadyGameStartingMessage message)
  {
    timer = 0.0f;
    gameState = GameState.INIT;
    StartCoroutine(timedTransitionToReplay());
  }

  public async void onClickReadyToReplay()
  {
    await networkManager.NotifyServerReplayReadyStateChanged(PlayerDataStorage.getLocalPlayer().IsReadyToReplay ? false : true);
  }

  public async void OnClickBackToMainMenu()
  {
    await networkManager.DisconnectFromServerAsync();
  }

  private string getPlayerGameObjectName(Player player)
  {
    return player.Assignment == PlayerAssignment.Player1 ? "P1" : "P2";
  }

  private void setGameplayPlayersNickname()
  {
    foreach (Player player in PlayerDataStorage.players.Values)
    {
      GameplayPlayersGameObject.transform.Find(getPlayerGameObjectName(player) + "/Nickname").GetComponent<TextMeshPro>().text = player.Nickname;
    }
  }

  private IEnumerator timedTransitionToReplay()
  {
    GameoverMenu.SetActive(false);
    GameoverTransitionTimer.SetActive(true);

    int timerValue = 3;
    while (timerValue > 0)
    {
      GameoverTransitionTimer.GetComponent<TextMeshProUGUI>().text = timerValue.ToString();
      yield return new WaitForSecondsRealtime(1);
      timerValue--;
    }
    SceneManager.LoadSceneAsync("GameplayScene");
  }

  private IEnumerator canPlayWhenStartingAnimationIsOver()
  {
    if (gameState != GameState.CAN_PLAY)
    {
      yield return new WaitUntil(() => AnimatedGameStartUI.transform.Find("backgroundFilter").GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);
      gameState = GameState.CAN_PLAY;
      _ = networkManager.NotifyServerPlayerCanPlay();
    }
  }

  private IEnumerator displayWinAnimationThenGameOverUI(GameResultDTO gameResult)
  {
    Animator animator = GameplayGameObject.GetComponent<Animator>();
    AnimatorStateInfo animatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);

    AttentionCatcherGameObject.SetActive(false);

    if (animatorStateInfo.IsName("NotStarted"))
    {
      animator.Play("WinAnimationFirstPart");
      yield return new WaitForSeconds(0.1f);
    }

    if (animator.GetCurrentAnimatorStateInfo(0).IsName("WinAnimationFirstPart") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0f)
    {
      yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);
      animator.Play(gameResult.winningPlayer == PlayerAssignment.Player1 ? "Player1Won" : "Player2Won");
    }

    yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);
    GetComponent<AudioSource>().PlayOneShot(PlayerDataStorage.getLocalPlayer().Assignment == gameResult.winningPlayer ? winAudioClip : lossAudioClip);
    yield return new WaitForSeconds(2f);

    GameoverUI.SetActive(true);
    ScoreboardGameObject.SetActive(true);

    // player 1
    Player p1 = PlayerDataStorage.players[PlayerAssignment.Player1];
    ScoreboardPlayersGameObject.transform.Find("P1/Nickname").GetComponent<TextMeshProUGUI>().text = p1.Nickname;
    if (gameResult.player1.inputPressed.HasValue)
      ScoreboardPlayersGameObject.transform.Find("P1/ButtonPressed").GetComponent<UIButtonController>().controllerButton = gameResult.player1.inputPressed.Value;
    else
      ScoreboardPlayersGameObject.transform.Find("P1/ButtonPressed").gameObject.SetActive(false);

    if (gameResult.player1.timeToPress.HasValue)
      ScoreboardPlayersGameObject.transform.Find("P1/TimeToPress").GetComponent<TextMeshProUGUI>().text = string.Format("{0} ms", gameResult.player1.timeToPress.Value);
    else
      ScoreboardPlayersGameObject.transform.Find("P1/TimeToPress").gameObject.SetActive(false);

    // player 2
    Player p2 = PlayerDataStorage.players[PlayerAssignment.Player2];
    ScoreboardPlayersGameObject.transform.Find("P2/Nickname").GetComponent<TextMeshProUGUI>().text = p2.Nickname;
    if (gameResult.player2.inputPressed.HasValue)
      ScoreboardPlayersGameObject.transform.Find("P2/ButtonPressed").GetComponent<UIButtonController>().controllerButton = gameResult.player2.inputPressed.Value;
    else
      ScoreboardPlayersGameObject.transform.Find("P2/ButtonPressed").gameObject.SetActive(false);

    if (gameResult.player2.timeToPress.HasValue)
      ScoreboardPlayersGameObject.transform.Find("P2/TimeToPress").GetComponent<TextMeshProUGUI>().text = string.Format("{0} ms", gameResult.player2.timeToPress.Value);
    else
      ScoreboardPlayersGameObject.transform.Find("P2/TimeToPress").gameObject.SetActive(false);

    if (gameResult.winningPlayer.HasValue)
      ScoreboardWinnerText.GetComponent<TextMeshProUGUI>().text = PlayerDataStorage.players[gameResult.winningPlayer.Value].Nickname + " Win!";
    else
      ScoreboardWinnerText.GetComponent<TextMeshProUGUI>().text = "Draw!";

    GameoverMenu.transform.Find("ButtonReplay").GetComponent<Button>().Select();
  }


  private void OnPlayerLeftRoomMessageHandler(PlayerLeftRoomMessage message)
  {
    PlayerAssignment pa = message.playerAssignment;
    PlayerDataStorage.players.Remove(pa);
    SceneManager.LoadSceneAsync("MainMenuScene");
  }

  private void OnDisconnectFromServerHandler()
  {
    PlayerDataStorage.reset();
    SceneManager.LoadSceneAsync("MainMenuScene");
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
      networkManager.OnPlayerLeftRoomMessage += OnPlayerLeftRoomMessageHandler;

      networkManager.OnGameExpectingInputMessage += GameExpectingInputMessageHandler;
      networkManager.OnGameOverEarlyInputMessage += GameOverEarlyInputMessageHandler;
      networkManager.OnGameOverWrongInputMessage += GameOverWrongInputMessageHandler;
      networkManager.OnGameOverWinMessage += GameOverWinMessageHandler;
      networkManager.OnGameOverDrawMessage += GameOverDrawMessageHandler;
      networkManager.OnPlayerReplayReadyStateChangedMessage += PlayerReplayReadyStateChangedMessageHandler;
      networkManager.OnAllReplayReadyGameStartingMessage += AllReplayReadyGameStartingMessageHandler;
      areHandlersRegistered = true;
    }
  }

  private void unregisterNetworkHandlers()
  {
    if (areHandlersRegistered)
    {
      networkManager.OnDisconnect -= OnDisconnectFromServerHandler;
      networkManager.OnPlayerLeftRoomMessage -= OnPlayerLeftRoomMessageHandler;

      networkManager.OnGameExpectingInputMessage -= GameExpectingInputMessageHandler;
      networkManager.OnGameOverEarlyInputMessage -= GameOverEarlyInputMessageHandler;
      networkManager.OnGameOverWrongInputMessage -= GameOverWrongInputMessageHandler;
      networkManager.OnGameOverWinMessage -= GameOverWinMessageHandler;
      networkManager.OnGameOverDrawMessage -= GameOverDrawMessageHandler;
      networkManager.OnPlayerReplayReadyStateChangedMessage -= PlayerReplayReadyStateChangedMessageHandler;
      networkManager.OnAllReplayReadyGameStartingMessage -= AllReplayReadyGameStartingMessageHandler;
      areHandlersRegistered = false;
    }
  }
}


