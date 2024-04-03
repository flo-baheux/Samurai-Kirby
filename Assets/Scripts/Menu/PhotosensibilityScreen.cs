using System.Collections;
using UnityEngine;

public class PhotosensibilityScreen : MonoBehaviour
{
  [SerializeField] private GameObject startScreenGameObject;

  private readonly int transitionTime = 4;
  private static bool hasBeenDisplayed = false;

  private void Start()
  {
    StartCoroutine(transitionAfterSeconds(hasBeenDisplayed ? 0 : transitionTime));
  }

  void Update()
  {
    if (Input.anyKeyDown)
      SwitchToStartScreen();
  }

  private IEnumerator transitionAfterSeconds(int secondsBeforeTransition)
  {
    yield return new WaitForSeconds(secondsBeforeTransition);
    SwitchToStartScreen();
  }

  private void SwitchToStartScreen()
  {
    hasBeenDisplayed = true;
    startScreenGameObject.SetActive(true);
    gameObject.SetActive(false);
  }
}
