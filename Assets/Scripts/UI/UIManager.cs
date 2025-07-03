using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private List<CanvasGroup> interactivePanels = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void BlockAllExcept(CanvasGroup allowedPanel)
    {
        foreach (var panel in interactivePanels)
        {
            bool allow = panel == allowedPanel;
            panel.interactable = allow;
            panel.blocksRaycasts = allow;
        }
    }

    public void UnblockAll()
    {
        foreach (var panel in interactivePanels)
        {
            panel.interactable = true;
            panel.blocksRaycasts = true;
        }
    }

    public bool IsInteractionBlocked()
    {
        foreach (var group in interactivePanels)
        {
            if (!group.interactable)
                return true;
        }
        return false;
    }
}