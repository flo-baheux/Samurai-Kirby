using UnityEngine;
using UnityEngine.EventSystems;

public class CreditsScreen : MonoBehaviour
{
  [SerializeField] private GameObject startScreenGameObject;

  void Update()
  {
    if (Input.anyKeyDown && !Input.GetMouseButtonDown(0))
    {
      startScreenGameObject.SetActive(true);
      gameObject.SetActive(false);
    }
  }

  private void OnEnable()
  {
    EventSystem.current.SetSelectedGameObject(null);
  }
}
