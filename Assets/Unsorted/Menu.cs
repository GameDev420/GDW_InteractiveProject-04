using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

public class Menu : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private string openAnimationTriggerName = "Open", closeAnimationTriggerName = "Close";
    private Button[] menuButtons;

    public UnityEvent onOpen;
    public UnityEvent onClose;

    private void OnEnable()
    {
        if (animator == null && !TryGetComponent(out animator)) Debug.LogWarning("Animator component reference not set on this menu!", this);

        menuButtons = GetComponentsInChildren<Button>();
    }

    void Open()
    {
        if (animator != null) animator.SetTrigger(openAnimationTriggerName);
        else gameObject.SetActive(true);

        EnableButtonInteraction();

        if (onOpen != null) onOpen.Invoke();
    }
    void Close()
    {
        if (animator != null) animator.SetTrigger(closeAnimationTriggerName);
        else gameObject.SetActive(false);

        DisableButtonInteraction();

        if (onClose != null) onClose.Invoke();
    }

    public void Open(float delay = 0.0f)
    {
        StopAllCoroutines();
        if (delay > 0) StartCoroutine(WaitToOpenOrClose(delay, true));
        else Open();
    }
    public void Close(float delay = 0.0f)
    {
        StopAllCoroutines();

        if (delay > 0) StartCoroutine(WaitToOpenOrClose(delay, false));
        else Close();
    }

    private void EnableButtonInteraction()
    {
        if (menuButtons == null || menuButtons.Length == 0) return;

        foreach (var button in menuButtons)
        {
            if (button != null) button.interactable = true;
        }
    }
    private void DisableButtonInteraction()
    {
        if (menuButtons == null || menuButtons.Length == 0) return;

        foreach (var button in menuButtons)
        {
            if (button != null) button.interactable = false;
        }
    }

    IEnumerator WaitToOpenOrClose(float delay, bool open)
    {
        yield return new WaitForSeconds(delay);
        if (open) Open();
        else Close();
    }
}
