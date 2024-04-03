using UnityEngine;
using UnityEngine.UI;

public class UIButtonController : MonoBehaviour
{
  public ControllerButton controllerButton;

  private GameControllerDetector gameControllerDetector;
  private ControllerType currentControllerType = ControllerType.KEYBOARD;

  void Start()
  {
    gameControllerDetector = GameControllerDetector.instance;
    GetComponent<Image>().sprite = ControllerButtonSpriteManager.getSprite(controllerButton, currentControllerType);
  }

  void Update()
  {
    if (gameControllerDetector.getCurrentControllerType() != currentControllerType)
    {
      currentControllerType = gameControllerDetector.getCurrentControllerType();
      GetComponent<Image>().sprite = ControllerButtonSpriteManager.getSprite(controllerButton, currentControllerType);
    }
  }
}
