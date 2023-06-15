using Champis.UI;
using RoboRyanTron.SearchableEnum;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SelectableExtended : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, ISelectHandler, IDeselectHandler, ISubmitHandler, IModalWindowOpen, IModalWindowClose, IPointerDownHandler, IPointerUpHandler
{
    [System.Serializable]
    public struct NeighbourFindingMode
    {
        public bool overrideNavigation;
        [Space]
        public bool findUp;
        public bool findDown;
        public bool findLeft;
        public bool findRight;

        public NeighbourFindingMode(bool replaceNavigation = true, bool up = true, bool down = true, bool left = true, bool right = true)
        {
            overrideNavigation = replaceNavigation;
            findUp = up;
            findDown = down;
            findLeft = left;
            findRight = right;
        }
    }

    enum BranchingNavigationFindEvent { OnEnable, OnAwake, OnStart, OnUpdate, OnlyViaScripting }

    [Foldout("Interaction")]
    [Tooltip("If the mouse pointer enters this button, the button will be selected rather than highlighted.")]
    [SerializeField] bool forceSelection = true;
    [SerializeField] bool loseSelectionOnMouseExit = false;
    [Space]
    [SerializeField] bool useItemDescription;

    [Foldout("Audio")]
    [Tooltip("Set this to false if you don't want this button to sound when selected. Useful for overriding the default navigation sound.")]
    [SerializeField] bool useSelectionSound = true;
    [ConditionalHide("useSelectionSound", true)] [SerializeField] string navigationSoundName = "UI Navigation";
    [Tooltip("Set this to false if you don't want this button to sound when clicked. Useful for overriding the default submit sound.")]
    [SerializeField] bool useSubmitSound = true;
    [ConditionalHide("useSubmitSound", true)] [SerializeField] string submitSoundName = "UI Submit";
    [SerializeField] bool useToggleOffSound;
    [ConditionalHide("useToggleOffSound", true)] [SerializeField] string toggleOffSoundName = "UI Toggle Off";

    [Foldout("Automatic Finding")]
    [SerializeField] bool findNeighboursOnStart = false;
    [SerializeField] NeighbourFindingMode findingMode;

    [Foldout("Branch Navigation")]
    [SerializeField, Tooltip("Take the first Selectable to be active and enabled; and place it in the Navigation slot.\n\nButtons on top take priority.")] bool useBranchingNavigation;
    [SerializeField, Tooltip("When to check and set the branching navigation.\n\nYou can always set the branching navigation via script if needed.")] BranchingNavigationFindEvent branchingNavigationEvent;
    [SerializeField, Tooltip("Leaves the Navigation slot empty if all possible Selectables are not available.")] bool nullIfNoSelectable = true;
    [Space]
    [SerializeField] Selectable[] upSelectables;
    [SerializeField] Selectable[] downSelectables, leftSelectables, rightSelectables;

    [Foldout("Environment")]
    [Tooltip("Enable this button in a desktop environment?")] public bool desktop = true;
    [Tooltip("Enable this button in a console environment?")] public bool console = true;
    [Tooltip("Enable this button in a handheld environment?")] public bool handheld = true;

    [Foldout("Press To Invoke")]
    [Tooltip("If this button is pressed on a gamepad, this UI Button will be pressed aswell.\n\nTHIS ONLY WORKS FOR BUTTONS.")]
    [SearchableEnum] public InputSystemButtons invokeButton;
    [Space]
    [SearchableEnum] public KeyCode keyboardInvokeKey;
    [SearchableEnum] public KeyCode secondaryKeyboardInvokeKey;
    [Space]
    [SearchableEnum] public KeyCode comboKeycode = KeyCode.LeftShift;
    [SearchableEnum] public KeyCode secondaryComboKeycode = KeyCode.RightShift;
    [Space]
    [SerializeField] bool primaryKeyRequiresModifier;
    [SerializeField] bool secondaryKeyRequiresModifier = true;
    [Space]
    public bool blockIfModalIsOpen = true;
    public bool blockIfKeyboardIsOpen = true;

    [Foldout("Invoke Repeating")]
    [Tooltip("If enabled, this button will be pressed repeteadly while holding down the 'Invoke Button'.")]
    public bool invokeRepeating;
    [Tooltip("Time in seconds that the user must hold down 'Invoke Button' before starting to repeat the action.")]
    public float holdDelay = 0.5f;
    [Tooltip("Time in seconds to wait between every repeat.")]
    public float repeatDelay = 0.05f;

    [Foldout("Hold To Invoke")]
    [Tooltip("Whether you must hold this UI button to actually invoke it.")]
    [SerializeField] bool holdToInvoke;
    [Tooltip("Prevents the button to be filled multiple times without releasing the 'Invoke Button'. Once filled, the user must release the button and hold it back again to fill it one more time.")]
    [SerializeField] bool oneTimeFill = true;
    [Space]
    [Tooltip("The image which will be filled to represent the time holding.")]
    [SerializeField] Image fill;
    [Space]
    [Tooltip("How much time (in seconds) you must hold this UI button before invoking it.")]
    [SerializeField] float holdTime = 1;
    [Tooltip("How fast the fill goes down when the button is released before invoking it.")]
    [SerializeField] float subtractionSpeed = 0.5f;

    float timeAfterTab;

    float timeHolding;
    float repeatTime;

    bool isSelected;
    bool modalOpen;

    bool holding;
    bool alreadyHeldToInvoke;

    Selectable selectable;
    Button currentUIButton;
    Dropdown currentDropdown;
    Toggle currentToggle;
    OnScreenKeyboardKey onscreenKey;

    LocalizedItemDescription itemDescription;

    UnityEvent cachedOnSubmit;

    [ContextMenu("Find Neighbours Automatically")]
    public void _fn() => FindNeighbours(new NeighbourFindingMode());

    public void FindNeighbours(NeighbourFindingMode mode)
    {
        Selectable thisButton = GetComponent<Selectable>();

        if (thisButton.navigation.mode != Navigation.Mode.Automatic)
        {
            ChampisConsole.LogWarning("The navigation mode is not set to 'Automatic' in '" + thisButton.gameObject.name + "'. Switching...");

            Navigation tmp = thisButton.navigation;
            tmp.mode = Navigation.Mode.Automatic;
            thisButton.navigation = tmp;
        }

        Navigation newNav = new Navigation();
        newNav.mode = Navigation.Mode.Explicit;

        newNav.selectOnUp = (!mode.findUp && !mode.overrideNavigation && thisButton.navigation.selectOnUp != null) ? thisButton.navigation.selectOnUp : thisButton.FindSelectableOnUp();
        newNav.selectOnDown = (!mode.findDown && !mode.overrideNavigation && thisButton.navigation.selectOnDown != null) ? thisButton.navigation.selectOnDown : thisButton.FindSelectableOnDown();
        newNav.selectOnLeft = (!mode.findLeft && !mode.overrideNavigation && thisButton.navigation.selectOnLeft != null) ? thisButton.navigation.selectOnLeft : thisButton.FindSelectableOnLeft();
        newNav.selectOnRight = (!mode.findRight && !mode.overrideNavigation && thisButton.navigation.selectOnRight != null) ? thisButton.navigation.selectOnRight : thisButton.FindSelectableOnRight();

        thisButton.navigation = newNav;
    }

    public void SetBranchingNavigation()
    {
        if(!useBranchingNavigation)
        {
            ChampisConsole.LogWarning($"[{gameObject.name}] Branching Navigation is turned off, but you're trying to use it. Enable Branching Navigation in the Inspector");
            return;
        }

        Selectable thisButton = GetComponent<Selectable>();

        if (thisButton.navigation.mode != Navigation.Mode.Explicit)
        {
            ChampisConsole.LogWarning("The navigation mode is not set to 'Explicit' in '" + thisButton.gameObject.name + "'. Switching...");

            Navigation tmp = thisButton.navigation;
            tmp.mode = Navigation.Mode.Explicit;
            thisButton.navigation = tmp;
        }

        Navigation newNav = new Navigation();
        newNav.mode = Navigation.Mode.Explicit;

        int i = 0;

        #region Up Branch
        if(upSelectables.Length > 0)
        {
            for (i = 0; i < upSelectables.Length; i++)
            {
                if (upSelectables[i].gameObject.activeInHierarchy && upSelectables[i].interactable && upSelectables[i].navigation.mode != Navigation.Mode.None)
                {
                    newNav.selectOnUp = upSelectables[i];
                    break;
                }
                else
                {
                    if (i == upSelectables.Length - 1 && nullIfNoSelectable)
                        newNav.selectOnUp = nullIfNoSelectable ? null : thisButton.navigation.selectOnUp;
                }
            }
        }
        else { newNav.selectOnUp = thisButton.navigation.selectOnUp; }
        #endregion

        #region Down Branch
        if (downSelectables.Length > 0)
        {
            for (i = 0; i < downSelectables.Length; i++)
            {
                if (downSelectables[i].gameObject.activeInHierarchy && downSelectables[i].interactable && downSelectables[i].navigation.mode != Navigation.Mode.None)
                {
                    newNav.selectOnDown = downSelectables[i];
                    break;
                }
                else
                {
                    if (i == downSelectables.Length - 1)
                        newNav.selectOnDown = nullIfNoSelectable ? null : thisButton.navigation.selectOnDown;
                }
            }
        }
        else { newNav.selectOnDown = thisButton.navigation.selectOnDown; }
        #endregion

        #region Left Branch
        if(leftSelectables.Length > 0)
        {
            for (i = 0; i < leftSelectables.Length; i++)
            {
                if (leftSelectables[i].gameObject.activeInHierarchy && leftSelectables[i].interactable && leftSelectables[i].navigation.mode != Navigation.Mode.None)
                {
                    newNav.selectOnLeft = leftSelectables[i];
                    break;
                }
                else
                {
                    if (i == leftSelectables.Length - 1 && nullIfNoSelectable)
                        newNav.selectOnLeft = nullIfNoSelectable ? null : thisButton.navigation.selectOnLeft;
                }
            }
        }
        else { newNav.selectOnLeft = thisButton.navigation.selectOnLeft; }
        #endregion

        #region Right Branch
        if (rightSelectables.Length > 0)
        {
            for (i = 0; i < rightSelectables.Length; i++)
            {
                if (rightSelectables[i].gameObject.activeInHierarchy && rightSelectables[i].interactable && rightSelectables[i].navigation.mode != Navigation.Mode.None)
                {
                    newNav.selectOnRight = rightSelectables[i];
                    break;
                }
                else
                {
                    if (i == rightSelectables.Length - 1 && nullIfNoSelectable)
                        newNav.selectOnRight = nullIfNoSelectable ? null : thisButton.navigation.selectOnRight;
                }
            }
        }
        else { newNav.selectOnRight = thisButton.navigation.selectOnRight; }
        #endregion

        thisButton.navigation = newNav;
    }

    private void Awake()
    {
        selectable = GetComponent<Selectable>();
        currentUIButton = GetComponent<Button>();
        currentDropdown = GetComponent<Dropdown>();
        currentToggle = GetComponent<Toggle>();
        onscreenKey = GetComponent<OnScreenKeyboardKey>();

        isSelected = false;
        timeAfterTab = 0;

        if (useBranchingNavigation && branchingNavigationEvent == BranchingNavigationFindEvent.OnAwake)
            SetBranchingNavigation();
    }

    IEnumerator Start()
    {
        if (useItemDescription)
            itemDescription = GetComponent<LocalizedItemDescription>();

        if (holdToInvoke)
        {
            cachedOnSubmit = currentUIButton.onClick;
            currentUIButton.onClick = new Button.ButtonClickedEvent();
        }

        CheckEnvironmentType();

        yield return new WaitForEndOfFrame();

        if (findNeighboursOnStart)
            FindNeighbours(findingMode);

        yield return new WaitForEndOfFrame();

        if (useBranchingNavigation && branchingNavigationEvent == BranchingNavigationFindEvent.OnStart)
            SetBranchingNavigation();

        yield return null;
    }

    private void Update()
    {
        #region BRANCHING NAVIGATION
        if (useBranchingNavigation && branchingNavigationEvent == BranchingNavigationFindEvent.OnUpdate)
            SetBranchingNavigation();
        #endregion

        #region TAB TO NEXT
        if (isSelected)
        {
            timeAfterTab += Time.unscaledDeltaTime;
            timeAfterTab = Mathf.Clamp(timeAfterTab, 0f, 3f);
        }

        if (timeAfterTab > 0.3f && isSelected && Keyboard.current != null && Keyboard.current.tabKey.wasPressedThisFrame)
        {
            timeAfterTab = 0;

            if (selectable.navigation.mode == Navigation.Mode.Explicit)
            {
                if (selectable.navigation.selectOnRight)
                    EventSystem.current.SetSelectedGameObject(selectable.navigation.selectOnRight.gameObject);
                else if (selectable.navigation.selectOnDown)
                    EventSystem.current.SetSelectedGameObject(selectable.navigation.selectOnDown.gameObject);
            }
            else
            {
                if (selectable.FindSelectableOnRight())
                    EventSystem.current.SetSelectedGameObject(selectable.FindSelectableOnRight().gameObject);
                else if (selectable.FindSelectableOnDown())
                    EventSystem.current.SetSelectedGameObject(selectable.FindSelectableOnDown().gameObject);
            }
        }
        #endregion

        if (modalOpen && blockIfModalIsOpen || OnScreenKeyboard.IsShowing() && blockIfKeyboardIsOpen)
            return;

        if (invokeRepeating && holdToInvoke)
        {
            ChampisConsole.LogWarning($"'Invoke Repeating' and 'Hold to Invoke' are both enabled in '{gameObject.name}'. This button will be ignored.");
            return;
        }

        #region STANDARD BEHAVIOUR
        if (invokeButton != InputSystemButtons.none || currentUIButton != null)
        {
            if (!holdToInvoke)
            {
                switch (invokeButton)
                {
                    case InputSystemButtons.yButton:
                        if (Gamepad.current != null && (Gamepad.current.yButton.wasPressedThisFrame || Gamepad.current.triangleButton.wasPressedThisFrame))
                            ExecuteEvents.Execute(currentUIButton.gameObject, new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler);
                        break;
                    case InputSystemButtons.aButton:
                        if (Gamepad.current != null && (Gamepad.current.aButton.wasPressedThisFrame || Gamepad.current.crossButton.wasPressedThisFrame))
                            ExecuteEvents.Execute(currentUIButton.gameObject, new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler);
                        break;
                    case InputSystemButtons.bButton:
                        if (Gamepad.current != null && (Gamepad.current.bButton.wasPressedThisFrame || Gamepad.current.circleButton.wasPressedThisFrame))
                            ExecuteEvents.Execute(currentUIButton.gameObject, new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler);
                        break;
                    case InputSystemButtons.xButton:
                        if (Gamepad.current != null && (Gamepad.current.xButton.wasPressedThisFrame || Gamepad.current.squareButton.wasPressedThisFrame))
                            ExecuteEvents.Execute(currentUIButton.gameObject, new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler);
                        break;
                    case InputSystemButtons.leftShoulder:
                        if (Gamepad.current != null && Gamepad.current.leftShoulder.wasPressedThisFrame)
                            ExecuteEvents.Execute(currentUIButton.gameObject, new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler);
                        break;
                    case InputSystemButtons.rightShoulder:
                        if (Gamepad.current != null && Gamepad.current.rightShoulder.wasPressedThisFrame)
                            ExecuteEvents.Execute(currentUIButton.gameObject, new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler);
                        break;
                    case InputSystemButtons.leftTrigger:
                        if (Gamepad.current != null && Gamepad.current.leftTrigger.wasPressedThisFrame)
                            ExecuteEvents.Execute(currentUIButton.gameObject, new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler);
                        break;
                    case InputSystemButtons.rightTrigger:
                        if (Gamepad.current != null && Gamepad.current.rightTrigger.wasPressedThisFrame)
                            ExecuteEvents.Execute(currentUIButton.gameObject, new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler);
                        break;
                    case InputSystemButtons.leftStickButton:
                        if (Gamepad.current != null && Gamepad.current.leftStickButton.wasPressedThisFrame)
                            ExecuteEvents.Execute(currentUIButton.gameObject, new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler);
                        break;
                    case InputSystemButtons.rightStickButton:
                        if (Gamepad.current != null && Gamepad.current.rightStickButton.wasPressedThisFrame)
                            ExecuteEvents.Execute(currentUIButton.gameObject, new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler);
                        break;
                    case InputSystemButtons.start:
                        if (Gamepad.current != null && Gamepad.current.startButton.wasPressedThisFrame)
                            ExecuteEvents.Execute(currentUIButton.gameObject, new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler);
                        break;
                    case InputSystemButtons.select:
                        if (Gamepad.current != null && Gamepad.current.selectButton.wasPressedThisFrame)
                            ExecuteEvents.Execute(currentUIButton.gameObject, new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler);
                        break;
                    case InputSystemButtons.escapeKey:
                        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
                            ExecuteEvents.Execute(currentUIButton.gameObject, new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler);
                        break;
                    case InputSystemButtons.escapeAndBButton:
                        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame || Gamepad.current != null && Gamepad.current.buttonEast.wasPressedThisFrame)
                            ExecuteEvents.Execute(currentUIButton.gameObject, new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler);
                        break;
                }
            }
        }

        if (keyboardInvokeKey != KeyCode.None && Input.GetKeyDown(keyboardInvokeKey))
        {
            if (comboKeycode != KeyCode.None && !primaryKeyRequiresModifier && (Input.GetKey(comboKeycode) || Input.GetKey(secondaryComboKeycode)))
                return;

            if ((primaryKeyRequiresModifier && (Input.GetKey(comboKeycode) || Input.GetKey(secondaryComboKeycode))) || !primaryKeyRequiresModifier || (comboKeycode == KeyCode.None && secondaryComboKeycode == KeyCode.None))
                ExecuteEvents.Execute(currentUIButton.gameObject, new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler);
        }
        else if (secondaryKeyboardInvokeKey != KeyCode.None && Input.GetKeyDown(secondaryKeyboardInvokeKey))
        {
            if (comboKeycode != KeyCode.None && !secondaryKeyRequiresModifier && (Input.GetKey(comboKeycode) || Input.GetKey(secondaryComboKeycode)))
                return;

            if ((secondaryKeyRequiresModifier && (Input.GetKey(comboKeycode) || Input.GetKey(secondaryComboKeycode))) || !secondaryKeyRequiresModifier || (comboKeycode == KeyCode.None && secondaryComboKeycode == KeyCode.None))
                ExecuteEvents.Execute(currentUIButton.gameObject, new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler);
        }
        #endregion

        #region REPEAT BEHAVIOUR
        if (invokeRepeating)
        {
            switch (invokeButton)
            {
                case InputSystemButtons.yButton:
                    if (Gamepad.current != null && (Gamepad.current.yButton.isPressed || Gamepad.current.triangleButton.isPressed))
                        timeHolding += Time.deltaTime;
                    else
                        timeHolding = 0;
                    break;
                case InputSystemButtons.aButton:
                    if (Gamepad.current != null && (Gamepad.current.aButton.isPressed || Gamepad.current.crossButton.isPressed))
                        timeHolding += Time.deltaTime;
                    else
                        timeHolding = 0;
                    break;
                case InputSystemButtons.bButton:
                    if (Gamepad.current != null && (Gamepad.current.bButton.isPressed || Gamepad.current.circleButton.isPressed))
                        timeHolding += Time.deltaTime;
                    else
                        timeHolding = 0;
                    break;
                case InputSystemButtons.xButton:
                    if (Gamepad.current != null && (Gamepad.current.xButton.isPressed || Gamepad.current.squareButton.isPressed))
                        timeHolding += Time.deltaTime;
                    else
                        timeHolding = 0;
                    break;
                case InputSystemButtons.leftShoulder:
                    if (Gamepad.current != null && Gamepad.current.leftShoulder.isPressed)
                        timeHolding += Time.deltaTime;
                    else
                        timeHolding = 0;
                    break;
                case InputSystemButtons.rightShoulder:
                    if (Gamepad.current != null && Gamepad.current.rightShoulder.isPressed)
                        timeHolding += Time.deltaTime;
                    else
                        timeHolding = 0;
                    break;
                case InputSystemButtons.leftTrigger:
                    if (Gamepad.current != null && Gamepad.current.leftTrigger.isPressed)
                        timeHolding += Time.deltaTime;
                    else
                        timeHolding = 0;
                    break;
                case InputSystemButtons.rightTrigger:
                    if (Gamepad.current != null && Gamepad.current.rightTrigger.isPressed)
                        timeHolding += Time.deltaTime;
                    else
                        timeHolding = 0;
                    break;
                case InputSystemButtons.leftStickButton:
                    if (Gamepad.current != null && Gamepad.current.leftStickButton.isPressed)
                        timeHolding += Time.deltaTime;
                    else
                        timeHolding = 0;
                    break;
                case InputSystemButtons.rightStickButton:
                    if (Gamepad.current != null && Gamepad.current.rightStickButton.isPressed)
                        timeHolding += Time.deltaTime;
                    else
                        timeHolding = 0;
                    break;
                case InputSystemButtons.start:
                    if (Gamepad.current != null && Gamepad.current.startButton.isPressed)
                        timeHolding += Time.deltaTime;
                    else
                        timeHolding = 0;
                    break;
                case InputSystemButtons.select:
                    if (Gamepad.current != null && Gamepad.current.selectButton.isPressed)
                        timeHolding += Time.deltaTime;
                    else
                        timeHolding = 0;
                    break;
                case InputSystemButtons.escapeKey:
                    if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
                        timeHolding += Time.deltaTime;
                    else
                        timeHolding = 0;
                    break;
                case InputSystemButtons.escapeAndBButton:
                    if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame || Gamepad.current != null && Gamepad.current.buttonEast.wasPressedThisFrame)
                        timeHolding += Time.deltaTime;
                    else
                        timeHolding = 0;
                    break;
            }

            if (timeHolding > holdDelay)
            {
                repeatTime += Time.deltaTime;

                if (repeatTime > repeatDelay)
                {
                    ExecuteEvents.Execute(currentUIButton.gameObject, new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler);
                    repeatTime = 0;
                }
            }
        }
        #endregion

        #region HOLD TO INVOKE
        if (holdToInvoke)
        {
            if (Gamepad.current != null)
            {
                switch (invokeButton)
                {
                    case InputSystemButtons.yButton:
                        if (Gamepad.current.yButton.wasPressedThisFrame || Gamepad.current.triangleButton.wasPressedThisFrame)
                            holding = true;
                        if (Gamepad.current.yButton.wasReleasedThisFrame || Gamepad.current.triangleButton.wasReleasedThisFrame)
                            holding = false;
                        break;
                    case InputSystemButtons.aButton:
                        if (Gamepad.current.aButton.wasPressedThisFrame || Gamepad.current.crossButton.wasPressedThisFrame)
                            holding = true;
                        if (Gamepad.current.aButton.wasReleasedThisFrame || Gamepad.current.crossButton.wasReleasedThisFrame)
                            holding = false;
                        break;
                    case InputSystemButtons.bButton:
                        if (Gamepad.current.bButton.wasPressedThisFrame || Gamepad.current.circleButton.wasPressedThisFrame)
                            holding = true;
                        if (Gamepad.current.bButton.wasReleasedThisFrame || Gamepad.current.circleButton.wasReleasedThisFrame)
                            holding = false;
                        break;
                    case InputSystemButtons.xButton:
                        if (Gamepad.current.xButton.wasPressedThisFrame || Gamepad.current.squareButton.wasPressedThisFrame)
                            holding = true;
                        if (Gamepad.current.xButton.wasReleasedThisFrame || Gamepad.current.squareButton.wasReleasedThisFrame)
                            holding = false;
                        break;
                    case InputSystemButtons.leftShoulder:
                        if (Gamepad.current.leftShoulder.wasPressedThisFrame)
                            holding = true;
                        if (Gamepad.current.leftShoulder.wasReleasedThisFrame)
                            holding = false;
                        break;
                    case InputSystemButtons.rightShoulder:
                        if (Gamepad.current.rightShoulder.wasPressedThisFrame)
                            holding = true;
                        if (Gamepad.current.rightShoulder.wasReleasedThisFrame)
                            holding = false;
                        break;
                    case InputSystemButtons.leftTrigger:
                        if (Gamepad.current.leftTrigger.wasPressedThisFrame)
                            holding = true;
                        if (Gamepad.current.leftTrigger.wasReleasedThisFrame)
                            holding = false;
                        break;
                    case InputSystemButtons.rightTrigger:
                        if (Gamepad.current.rightTrigger.wasPressedThisFrame)
                            holding = true;
                        if (Gamepad.current.rightTrigger.wasReleasedThisFrame)
                            holding = false;
                        break;
                    case InputSystemButtons.leftStickButton:
                        if (Gamepad.current.leftStickButton.wasPressedThisFrame)
                            holding = true;
                        if (Gamepad.current.leftStickButton.wasReleasedThisFrame)
                            holding = false;
                        break;
                    case InputSystemButtons.rightStickButton:
                        if (Gamepad.current.rightStickButton.wasPressedThisFrame)
                            holding = true;
                        if (Gamepad.current.rightStickButton.wasReleasedThisFrame)
                            holding = false;
                        break;
                    case InputSystemButtons.start:
                        if (Gamepad.current.startButton.wasPressedThisFrame)
                            holding = true;
                        if (Gamepad.current.startButton.wasReleasedThisFrame)
                            holding = false;
                        break;
                    case InputSystemButtons.select:
                        if (Gamepad.current.selectButton.wasPressedThisFrame)
                            holding = true;
                        else
                            holding = false;
                        break;
                    case InputSystemButtons.escapeKey:
                        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
                            holding = true;
                        if (Keyboard.current != null && Keyboard.current.escapeKey.wasReleasedThisFrame)
                            holding = false;
                        break;
                    case InputSystemButtons.escapeAndBButton:
                        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame || Gamepad.current != null && Gamepad.current.buttonEast.wasPressedThisFrame)
                            holding = true;
                        if (Keyboard.current != null && Keyboard.current.escapeKey.wasReleasedThisFrame || Gamepad.current != null && Gamepad.current.buttonEast.wasReleasedThisFrame)
                            holding = false;
                        break;
                }
            }

            if (holding)
            {
                if (!alreadyHeldToInvoke || !oneTimeFill)
                    timeHolding += Time.deltaTime;
            }
            else
            {
                if (timeHolding > 0)
                    timeHolding -= Time.deltaTime * subtractionSpeed;

                alreadyHeldToInvoke = false;
            }

            fill.fillAmount = timeHolding / holdTime;

            if (fill.fillAmount == 1)
            {
                timeHolding = 0;
                alreadyHeldToInvoke = true;

                //cachedOnSubmit.Invoke();
                currentUIButton.onClick = (Button.ButtonClickedEvent)cachedOnSubmit;
                ExecuteEvents.Execute(currentUIButton.gameObject, new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler);
                currentUIButton.onClick = new Button.ButtonClickedEvent();
            }
        }
        #endregion
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!selectable.interactable)
            return;

        if (forceSelection)
            EventSystem.current.SetSelectedGameObject(gameObject);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (!selectable.interactable)
            return;

        if (loseSelectionOnMouseExit)
            EventSystem.current.SetSelectedGameObject(null);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!selectable.interactable)
            return;

        if (useSubmitSound && !holdToInvoke)
            AudioManager.PlaySound(submitSoundName);

        if (!forceSelection)
            EventSystem.current.SetSelectedGameObject(null);
    }

    public void OnPointerDown(PointerEventData data)
    {
        if (!selectable.interactable)
            return;

        if (holdToInvoke)
            holding = true;
    }
    public void OnPointerUp(PointerEventData data)
    {
        if (!selectable.interactable)
            return;

        if (holdToInvoke)
            holding = false;
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (!selectable.interactable)
            return;

        if (useSelectionSound)
            AudioManager.PlaySound(navigationSoundName);

        if (useItemDescription && itemDescription != null)
            itemDescription.ShowDescription();

        isSelected = true;
    }
    public void OnDeselect(BaseEventData eventData)
    {
        if (!selectable.interactable)
            return;

        if (useItemDescription)
            itemDescription.HideDescription();

        if (currentDropdown)
            currentDropdown.animator.SetBool("Open", false);

        isSelected = false;
    }
    public void OnSubmit(BaseEventData eventData)
    {
        if (!selectable.interactable)
            return;

        if (useSubmitSound)
        {
            if (currentToggle)
                AudioManager.PlaySound(currentToggle.isOn ? toggleOffSoundName : submitSoundName);
            else
                AudioManager.PlaySound(submitSoundName);
        }

        if (currentDropdown != null)
        {
            //GlobalSettings.dropdownIsActive = true;
            //GlobalSettings.currentDropdown = currentDropdown;
        }

        if (currentDropdown)
            currentDropdown.animator.SetBool("Open", true);

        if (onscreenKey != null)
            onscreenKey.SubmitKey();
    }

    public void CheckEnvironmentType()
    {
        switch (SystemInfo.deviceType)
        {
            case DeviceType.Desktop:
                selectable.gameObject.SetActive(desktop);
                break;
            case DeviceType.Console:
                selectable.gameObject.SetActive(console);
                break;
            case DeviceType.Handheld:
                selectable.gameObject.SetActive(handheld);
                break;
        }
    }

    public void OnModalWindowOpen()
    {
        modalOpen = true;
    }
    public void OnModalWindowClose()
    {
        modalOpen = false;
    }

    private void OnEnable()
    {
        if (useBranchingNavigation && branchingNavigationEvent == BranchingNavigationFindEvent.OnEnable)
            SetBranchingNavigation();
    }
    public void OnDisable()
    {
        if (gameObject == null || EventSystem.current == null)
            return;

        if (EventSystem.current.currentSelectedGameObject == gameObject)
            EventSystem.current.SetSelectedGameObject(null);
    }

    public void OpenURL(string url)
    {
        Application.OpenURL(url);
    }

    public void AnimatorEnableBool(string boolName) => selectable.animator.SetBool(boolName, true);
    public void AnimatorDisableBool(string boolName) => selectable.animator.SetBool(boolName, false);
}
