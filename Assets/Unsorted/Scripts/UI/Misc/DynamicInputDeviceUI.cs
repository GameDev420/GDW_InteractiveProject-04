using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DynamicInputDeviceUI : MonoBehaviour
{
    const string KeyboardAndMouseScheme = "Keyboard"; // doesn't have to be the exact name since it just checks if scheme name *contains* this.
    const string GamepadScheme = "Gamepad";

    [SerializeField] Selectable defaultGamepadSelectable;
    [Space]
    [Tooltip("Note that this ")]
    [SerializeField] bool autoFindChildElements = true;
    [SerializeField] Selectable[] allElements;

    Selectable lastGamepadSelectable;
    string lastControlScheme = "";

    private void Awake()
    {
        lastGamepadSelectable = defaultGamepadSelectable;

        if (autoFindChildElements)
        {
            var all = allElements.ToList();
            var childElements = GetComponentsInChildren<Selectable>();

            foreach (var c in childElements)
            {
                if (!all.Contains(c)) all.Add(c);
            }

            allElements = all.ToArray();
        }

        SwitchToKeyboardAndMouse();
    }

    private void OnEnable()
    {
        FindAnyObjectByType<PlayerInput>().onControlsChanged += OnControlsChanged;
    }

    private void OnDisable()
    {
        var playerInput = FindAnyObjectByType<PlayerInput>();
        if (playerInput != null) playerInput.onControlsChanged -= OnControlsChanged;
    }

    void OnControlsChanged(PlayerInput p)
    {
        if (p.currentControlScheme == lastControlScheme) return;

        if (p.currentControlScheme.Contains(KeyboardAndMouseScheme)) SwitchToKeyboardAndMouse();
        else if (p.currentControlScheme.Contains(GamepadScheme)) SwitchToGamepad();
        else Debug.Log($"Unsupported UI input scheme detected: {p.currentControlScheme}", this);

        lastControlScheme = p.currentControlScheme;
    }

    void SwitchToGamepad()
    {
        DeselectAll();
        SetNavigationMode(Navigation.Mode.Automatic);
        if (lastGamepadSelectable != null) lastGamepadSelectable.Select();
    }
    void SwitchToKeyboardAndMouse()
    {
        DeselectAll();
        SetNavigationMode(Navigation.Mode.None);
    }

    // Utils
    void DeselectAll()
    {
        var eventSystem = FindAnyObjectByType<EventSystem>();
        var e = new BaseEventData(eventSystem);

        for (int i = 0; i < allElements.Length; i++)
            if (allElements[i] != null)
                allElements[i].OnDeselect(e);
    }

    void SetNavigationMode(Navigation.Mode mode)
    {
        for (int i = 0; i < allElements.Length; i++)
        {
            if (allElements[i] != null)
            {
                var nav = allElements[i].navigation;
                nav.mode = mode;
                allElements[i].navigation = nav;
            }
        }
    }
}
