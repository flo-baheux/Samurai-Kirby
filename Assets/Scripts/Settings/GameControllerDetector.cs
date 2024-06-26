using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.Switch;

public enum ControllerType
{
  XBOX,
  PS,
  SWITCH,
  KEYBOARD
};

public class GameControllerDetector : MonoBehaviour
{
  public static GameControllerDetector instance;

  private ControllerType currentControllerType = ControllerType.KEYBOARD;

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
  }

  // Update is called once per frame
  void Update()
  {
    if (Gamepad.current != null)
    {
      if (Gamepad.current is DualShockGamepad)
      {
        currentControllerType = ControllerType.PS;
      }
      else if (Gamepad.current is SwitchProControllerHID)
      {
        currentControllerType = ControllerType.SWITCH;
      }
      else
      {
        // default to xbox if a gamepad is detected
        currentControllerType = ControllerType.XBOX;
      }
    }
    else
    {
      currentControllerType = ControllerType.KEYBOARD;
    }
  }

  public ControllerType getCurrentControllerType()
  {
    return currentControllerType;
  }
}
