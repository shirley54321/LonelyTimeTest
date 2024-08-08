using UnityEngine;

public class BasePanel : MonoBehaviour
{
    protected string Name;

    /// <summary>
    /// Activate or deactivate the panel.
    /// </summary>
    /// <param name="active">True to activate, false to deactivate.</param>
    public virtual void PanelSetActive(bool active)
    {
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        if (active)
        {
            canvasGroup.alpha = 1;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
        else
        {
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }

    /// <summary>
    /// Open the panel.
    /// </summary>
    /// <param name="name">Name to assign to the panel.</param>
    public virtual void OpenPanel(string name)
    {
        Name = name;
        PanelSetActive(true);
    }

    /// <summary>
    /// Close the panel.
    /// </summary>
    public virtual void ClosePanel()
    {
        PanelSetActive(false);
    }
}
