using UnityEngine;
using UnityEngine.UI;

public class StartScreen : MonoBehaviour
{
  [SerializeField] private GameObject joinGameScreen;
  [SerializeField] private GameObject lobbyScreen;
  [SerializeField] private GameObject settingsScreen;
  [SerializeField] private GameObject creditsScreen;

  private NetworkManager networkManager;

  private void Start()
  {
    networkManager = NetworkManager.instance;

    if (networkManager.IsConnected && PlayerDataStorage.getLocalPlayer() != null)
    {
      lobbyScreen.SetActive(true);
      gameObject.SetActive(false);
    }

    transform.Find("Menu/ButtonPlay").GetComponent<Button>().Select();
  }

  public void OnClickGoToJoinGameScreen()
  {
    joinGameScreen.SetActive(true);
    gameObject.SetActive(false);
  }

  public void OnClickGoToSettingsScreen()
  {
    settingsScreen.SetActive(true);
    gameObject.SetActive(false);
  }

  public void OnClickGoToCreditsScreen()
  {
    creditsScreen.SetActive(true);
    gameObject.SetActive(false);
  }

  public void OnClickQuitGame()
  {
#if UNITY_EDITOR
    UnityEditor.EditorApplication.isPlaying = false;
#endif
    Application.Quit();
  }

  private void OnEnable()
  {
    transform.Find("Menu/ButtonPlay").GetComponent<Button>().Select();
  }
}
