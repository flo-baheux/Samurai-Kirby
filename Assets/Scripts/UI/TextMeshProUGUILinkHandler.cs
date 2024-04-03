using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TextMeshProUGUILinkHandler : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
  [SerializeField] private Color hoverColor = Color.blue;
  private TextMeshProUGUI textComponent;
  private Color initialColor;

  private void Awake()
  {
    textComponent = GetComponent<TextMeshProUGUI>();
    initialColor = textComponent.color;
  }

  public void OnPointerClick(PointerEventData eventData)
  {
    int linkIndex = TMP_TextUtilities.FindIntersectingLink(textComponent, eventData.position, eventData.pressEventCamera);
    if (linkIndex != -1)
    {
      string url = textComponent.textInfo.linkInfo[linkIndex].GetLinkID();

      if (url.StartsWith("https"))
        Application.OpenURL(url);
      else
        Debug.LogError("Tried to open non-https URL [" + url + "]");
    }
  }

  public void OnPointerEnter(PointerEventData eventData)
  {
    textComponent.color = hoverColor;
  }

  public void OnPointerExit(PointerEventData eventData)
  {
    textComponent.color = initialColor;
  }
}