using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class GameplayMessageConverter : JsonConverter
{
  public override bool CanConvert(Type objectType)
  {
    return typeof(GameplayMessage).IsAssignableFrom(objectType);
  }

  public override object ReadJson(JsonReader reader,
      Type objectType, object existingValue, JsonSerializer serializer)
  {
    JObject jsonObject = JObject.Load(reader);
    GameplayMessage message;
    switch ((GameplayMessageType)(int)jsonObject["messageType"])
    {
      case GameplayMessageType.ROOM_JOINED:
        message = new RoomJoinedMessage();
        break;
      case GameplayMessageType.PLAYER_JOINED_GAME:
        message = new PlayerJoinedRoomMessage();
        break;
      case GameplayMessageType.PLAYER_LEFT_GAME:
        message = new PlayerLeftRoomMessage();
        break;
      case GameplayMessageType.PLAYER_READY_STATE_CHANGED:
        message = new PlayerReadyStateChangedMessage();
        break;
      case GameplayMessageType.ALL_READY_GAME_STARTING:
        message = new AllReadyGameStartingMessage();
        break;
      case GameplayMessageType.GAME_EXPECTING_INPUT:
        message = new GameExpectingInputMessage();
        break;
      case GameplayMessageType.GAME_OVER_EARLY_INPUT:
        message = new GameOverEarlyInputMessage();
        break;
      case GameplayMessageType.GAME_OVER_WRONG_INPUT:
        message = new GameOverWrongInputMessage();
        break;
      case GameplayMessageType.GAME_OVER_WIN:
        message = new GameOverWinMessage();
        break;
      case GameplayMessageType.GAME_OVER_DRAW:
        message = new GameOverDrawMessage();
        break;
      case GameplayMessageType.PLAYER_REPLAY_READY_STATE_CHANGED:
        message = new PlayerReplayReadyStateChangedMessage();
        break;
      case GameplayMessageType.ALL_REPLAY_READY_GAME_STARTING:
        message = new AllReplayReadyGameStartingMessage();
        break;

      default:
        message = new GameplayMessage();
        break;
    }

    serializer.Populate(jsonObject.CreateReader(), message);
    return message;
  }

  public override bool CanWrite
  {
    get { return false; }
  }

  public override void WriteJson(JsonWriter writer,
      object value, JsonSerializer serializer)
  {
    throw new NotImplementedException();
  }
}