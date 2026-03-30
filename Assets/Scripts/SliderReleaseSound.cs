using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SliderReleaseSound : MonoBehaviour, IPointerUpHandler
{
    public PauseMenuManager pauseMenuManager; // Assign in Inspector

    public void OnPointerUp(PointerEventData eventData)
    {
        if (pauseMenuManager != null)
        {
            pauseMenuManager.PlaySliderTick();
        }
    }
}