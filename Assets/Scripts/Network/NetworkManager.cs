using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Newtonsoft.Json;
using System.Net.WebSockets;
using Unity.VisualScripting;

public class NetworkManager : MonoBehaviour
{
  // private TcpClient client;
  private ClientWebSocket client;

  // private NetworkStream stream;

  public static NetworkManager instance;
  public static int DATA_BUFFER_SIZE = 4096;

  public string serverHost;
  public int serverPort;

  public bool IsConnected
  {
    get;
    private set;
  } = false;

  public event Action OnConnect;
  public event Action OnFailureToConnect;
  public event Action OnDisconnect;

  public event Action<RoomJoinedMessage> OnRoomJoinedMessage;
  public event Action<PlayerJoinedRoomMessage> OnPlayerJoinedRoomMessage;
  public event Action<PlayerLeftRoomMessage> OnPlayerLeftRoomMessage;
  public event Action<PlayerReadyStateChangedMessage> OnPlayerReadyStateChangedMessage;
  public event Action<AllReadyGameStartingMessage> OnAllReadyGameStartingMessage;

  public event Action<GameExpectingInputMessage> OnGameExpectingInputMessage;
  public event Action<GameOverEarlyInputMessage> OnGameOverEarlyInputMessage;
  public event Action<GameOverWrongInputMessage> OnGameOverWrongInputMessage;
  public event Action<GameOverWinMessage> OnGameOverWinMessage;
  public event Action<GameOverDrawMessage> OnGameOverDrawMessage;
  public event Action<PlayerReplayReadyStateChangedMessage> OnPlayerReplayReadyStateChangedMessage;
  public event Action<AllReplayReadyGameStartingMessage> OnAllReplayReadyGameStartingMessage;

  private void Awake()
  {
    if (instance == null)
    {
      instance = this;
    }
    else if (instance != this)
    {
      Destroy(this);
    }
    DontDestroyOnLoad(gameObject);

    new Thread(() =>
    {
      Thread.CurrentThread.IsBackground = true;
      Debug.Log("NEW THREAD STARTED");
      ReceiveFromNetwork();
    }).Start();
  }

  public async Task ConnectToServerAsync()
  {
    try
    {
      if (IsConnected == false)
      {
        // client = new TcpClient();
        client = new ClientWebSocket();
        // await client.ConnectAsync(IPAddress.Parse(serverHost), serverPort);
        await client.ConnectAsync(new Uri("ws://" + IPAddress.Parse(serverHost).ToString() + ":" + serverPort), CancellationToken.None);
        // stream = client.GetStream();
        IsConnected = true;
        OnConnect?.Invoke();
      }
      else
        Debug.Log("Could not connect to server");

    }
    catch (Exception e)
    {
      Debug.LogException(e);
      Debug.Log("Failed to connect to server - " + e.Message);
      IsConnected = false;
      OnFailureToConnect?.Invoke();
      return;
    }
  }

  async void ReceiveFromNetwork()
  {
    while (true)
    {
      // if (!IsConnected || !stream.CanRead)
      //   continue;
      try
      {
        byte[] buffer = new byte[DATA_BUFFER_SIZE];
        WebSocketReceiveResult result = await client.ReceiveAsync(buffer, CancellationToken.None);
        // int bytesRead = stream.Read(buffer, 0, buffer.Length);
        int bytesRead = result.Count;
        if (result.Count > 0)
        {
          string receivedData = System.Text.Encoding.UTF8.GetString(buffer, 0, bytesRead);
          string[] receivedMessages = receivedData.Split('\n');
          foreach (string message in receivedMessages)
          {
            if (message.Length == 0) continue;
            Debug.Log(message);
            GameplayMessage gameplayMessage = JsonConvert.DeserializeObject<GameplayMessage>(message, new GameplayMessageConverter());
            UnityMainThread.instance.AddJob(() =>
            {
              switch (gameplayMessage)
              {
                case RoomJoinedMessage message:
                  OnRoomJoinedMessage?.Invoke(message);
                  break;
                case PlayerJoinedRoomMessage message:
                  OnPlayerJoinedRoomMessage?.Invoke(message);
                  break;
                case PlayerLeftRoomMessage message:
                  OnPlayerLeftRoomMessage?.Invoke(message);
                  break;
                case PlayerReadyStateChangedMessage message:
                  OnPlayerReadyStateChangedMessage?.Invoke(message);
                  break;
                case AllReadyGameStartingMessage message:
                  OnAllReadyGameStartingMessage?.Invoke(message);
                  break;
                case GameExpectingInputMessage message:
                  OnGameExpectingInputMessage?.Invoke(message);
                  break;
                case GameOverEarlyInputMessage message:
                  OnGameOverEarlyInputMessage?.Invoke(message);
                  break;
                case GameOverWrongInputMessage message:
                  OnGameOverWrongInputMessage?.Invoke(message);
                  break;
                case GameOverWinMessage message:
                  OnGameOverWinMessage?.Invoke(message);
                  break;
                case GameOverDrawMessage message:
                  OnGameOverDrawMessage?.Invoke(message);
                  break;
                case PlayerReplayReadyStateChangedMessage message:
                  OnPlayerReplayReadyStateChangedMessage?.Invoke(message);
                  break;
                case AllReplayReadyGameStartingMessage message:
                  OnAllReplayReadyGameStartingMessage?.Invoke(message);
                  break;
                default:
                  break;
              }
            });
          }
        }
      }
      catch (Exception e)
      {
        Debug.LogError("Exception raised in network thread - " + e.GetType().ToString() + ": " + e);
        IsConnected = false;
        await client.CloseAsync(WebSocketCloseStatus.ProtocolError, "", CancellationToken.None);

        client.Dispose();
        UnityMainThread.instance.AddJob(() => OnDisconnect?.Invoke());
      }
    }
  }

  public async Task DisconnectFromServerAsync()
  {
    if (client.State == WebSocketState.Open)
    {
      await SendToServerAsync("disconnect");
      await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
      OnDisconnect?.Invoke();
      IsConnected = false;
    }
    client.Dispose();
  }

  public async Task JoinRoom(string nickname, Difficulty difficulty)
  {
    JoinGamePlayerActionMessage message = new JoinGamePlayerActionMessage(nickname, difficulty);
    await SendToServerAsync(JsonConvert.SerializeObject(message));
  }

  public async Task LeaveRoom()
  {
    LeaveGamePlayerActionMessage message = new LeaveGamePlayerActionMessage();
    await SendToServerAsync(JsonConvert.SerializeObject(message));
  }

  public async Task NotifyServerPlayerPressedButton(ControllerButton pressedButton, int timeToPress)
  {
    InputPlayerActionMessage message = new InputPlayerActionMessage(pressedButton, timeToPress);
    await SendToServerAsync(JsonConvert.SerializeObject(message));
  }

  public async Task NotifyServerReplayReadyStateChanged(bool isReadyToReplay)
  {
    SetReplayReadyStatePlayerActionMessage message = new SetReplayReadyStatePlayerActionMessage(isReadyToReplay);
    await SendToServerAsync(JsonConvert.SerializeObject(message));
  }

  public async Task NotifyServerPlayerCanPlay()
  {
    NotifyCanPlayPlayerActionMessage message = new NotifyCanPlayPlayerActionMessage();
    await SendToServerAsync(JsonConvert.SerializeObject(message));
  }

  public async Task NotifyServerPlayerIsReady()
  {
    SetReadyStatePlayerActionMessage message = new SetReadyStatePlayerActionMessage(true);
    await SendToServerAsync(JsonConvert.SerializeObject(message));
  }

  public async Task NotifyServerPlayerIsNotReady()
  {
    SetReadyStatePlayerActionMessage message = new SetReadyStatePlayerActionMessage(false);
    await SendToServerAsync(JsonConvert.SerializeObject(message));
  }

  private async Task SendToServerAsync(string message)
  {
    try
    {
      // if (client.Connected && stream.CanWrite)
      //   await stream.WriteAsync(System.Text.Encoding.UTF8.GetBytes(message));
      // else
      //   Debug.Log("Could not send message to server");
      await client.SendAsync(System.Text.Encoding.UTF8.GetBytes(message), WebSocketMessageType.Binary, true, CancellationToken.None);
    }
    catch (Exception e)
    {
      Debug.LogException(e);

      return;
    }
  }

  private void OnDestroy()
  {
    if (client != null)
    {
      // if (client.Connected)
      // {
      //   client.Close();
      // }
      client.Dispose();
    }
  }
}




