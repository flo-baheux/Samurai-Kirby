using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class FontApplier : MonoBehaviour
{
  void Start()
  {
    GetComponent<TextMeshProUGUI>().font = FontManager.instance.getCurrentFont();
    FontManager.instance.FontChanged += OnFontChangedUpdate;
  }

  private void OnFontChangedUpdate()
  {
    GetComponent<TextMeshProUGUI>().font = FontManager.instance.getCurrentFont();
  }

  private void OnDestroy()
  {
    FontManager.instance.FontChanged -= OnFontChangedUpdate;
  }
}
