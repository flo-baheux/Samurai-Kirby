using System.Collections.Generic;
using UnityEngine;


/* 
** Cannot use UnityEngine.InputSystem.LowLevel.GamepadButton
** because we mix controller input with keyboard
*/
public enum ControllerButton
{
  NORTH = 0,
  WEST = 1,
  EAST = 2,
  SOUTH = 3,
};

static public class ControllerButtonSpriteManager
{
  static private Dictionary<ControllerButton, Dictionary<ControllerType, Sprite>> controllerButtonSprites
   = new Dictionary<ControllerButton, Dictionary<ControllerType, Sprite>>() {
    { ControllerButton.NORTH, new Dictionary<ControllerType, Sprite>() },
    { ControllerButton.WEST, new Dictionary<ControllerType, Sprite>() },
    { ControllerButton.EAST, new Dictionary<ControllerType, Sprite>() },
    { ControllerButton.SOUTH, new Dictionary<ControllerType, Sprite>() },
  };

  static public Sprite defaultSprite;

  [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
  static void loadDefaultSprite()
  {
    const string defaultSpriteFilename = "DefaultSprite";
    defaultSprite = Resources.Load<Sprite>("Sprites/" + defaultSpriteFilename);
    if (!defaultSprite)
    {
      Debug.LogError("No " + defaultSpriteFilename + " found in Assets/Resources/Sprites");
    }
  }


  [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
  static void loadControllerButtonSprites()
  {
    Sprite[] sprites = Resources.LoadAll<Sprite>("Sprites/controllerButtonIcons");
    controllerButtonSprites[ControllerButton.NORTH][ControllerType.XBOX] = sprites[0];
    controllerButtonSprites[ControllerButton.NORTH][ControllerType.PS] = sprites[1];
    controllerButtonSprites[ControllerButton.NORTH][ControllerType.SWITCH] = sprites[2];
    controllerButtonSprites[ControllerButton.NORTH][ControllerType.KEYBOARD] = sprites[3];

    controllerButtonSprites[ControllerButton.WEST][ControllerType.XBOX] = sprites[4];
    controllerButtonSprites[ControllerButton.WEST][ControllerType.PS] = sprites[5];
    controllerButtonSprites[ControllerButton.WEST][ControllerType.SWITCH] = sprites[6];
    controllerButtonSprites[ControllerButton.WEST][ControllerType.KEYBOARD] = sprites[7];

    controllerButtonSprites[ControllerButton.EAST][ControllerType.XBOX] = sprites[8];
    controllerButtonSprites[ControllerButton.EAST][ControllerType.PS] = sprites[9];
    controllerButtonSprites[ControllerButton.EAST][ControllerType.SWITCH] = sprites[10];
    controllerButtonSprites[ControllerButton.EAST][ControllerType.KEYBOARD] = sprites[11];

    controllerButtonSprites[ControllerButton.SOUTH][ControllerType.XBOX] = sprites[12];
    controllerButtonSprites[ControllerButton.SOUTH][ControllerType.PS] = sprites[13];
    controllerButtonSprites[ControllerButton.SOUTH][ControllerType.SWITCH] = sprites[14];
    controllerButtonSprites[ControllerButton.SOUTH][ControllerType.KEYBOARD] = sprites[15];
  }

  static public Sprite getSprite(ControllerButton button, ControllerType type)
  {
    if (controllerButtonSprites.ContainsKey(button) && controllerButtonSprites[button].ContainsKey(type))
    {
      return controllerButtonSprites[button][type];
    }
    else
    {
      Debug.LogError("Failed to get sprite for button " + button + " with controller " + type + ". Fail over to default sprite.");
      return defaultSprite;
    }
  }
}
