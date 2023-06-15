using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

namespace Champis.UI
{
    public interface IModalWindowOpen
    {
        public void OnModalWindowOpen();
    }
    public interface IModalWindowClose
    {
        public void OnModalWindowClose();
    }

    [Serializable]
    public struct ModalWindowIcon
    {
        public string identifier;
        public Sprite iconSprite;

        public ModalWindowIcon(string id, Sprite icon)
        {
            identifier = id;
            iconSprite = icon;
        }
    }

    public struct ModalWindowRequest
    {
        public string content;
        public string title;
        public string footnote;
        public ModalWindowButtons buttons;
        public string titleIcon;
        public Texture2D contentImage;
        public bool showToggle;
        public string toggleLabelText;
        public bool showInputField;
        public string inputFieldLabelText;
        public bool includeAlt;
        public string altLabel;

        public Selectable selectOnClose;

        public Action OnOk;
        public Action OnCancel;
        public Action OnYes;
        public Action OnNo;
        public Action onClose;
        public Action onAlt;

        public ModalWindowRequest(string cont, string header, string foot, ModalWindowButtons btns, string icon, Texture2D img, bool togg, string toggLab, bool input, string inputLab, bool alt, string altLab, Action okEvent, Action cancelEvent, Action yesEvent, Action noEvent, Action closeEvent, Action altEvent, Selectable selBtn)
        {
            content = cont;
            title = header;
            footnote = foot;
            buttons = btns;
            titleIcon = icon;
            contentImage = img;
            showToggle = togg;
            toggleLabelText = toggLab;
            showInputField = input;
            inputFieldLabelText = inputLab;
            includeAlt = alt;
            altLabel = altLab;

            selectOnClose = selBtn;

            OnOk = okEvent;
            OnCancel = cancelEvent;
            OnYes = yesEvent;
            OnNo = noEvent;
            onClose = closeEvent;
            onAlt = altEvent;
        }
    }

    public class ModalWindow : MonoBehaviour
    {
        [SerializeField] RectTransform rootLayoutGroup;
        [SerializeField] Image bgPanel;

        [Header("HEADER")]
        [SerializeField] GameObject headerHolder;
        [SerializeField] Image titleIconDisplay;
        [SerializeField] Text titleDisplay;
        [Tooltip("If enabled, the window title will always be in uppercase.")]
        [SerializeField] bool forceTitleUppercase = true;

        [Header("CONTENT")]
        [SerializeField] TextMeshProUGUI contentDisplay;
        [SerializeField] RawImage contentImageDisplay;
        [SerializeField] Text footnoteDisplay;
        [Space]
        [SerializeField] Toggle toggle;
        [SerializeField] InputField inputField;
        [Space]
        [SerializeField] Text toggleLabel;
        [SerializeField] Text inputFieldLabel;
        [Space]
        [SerializeField] GameObject toggleHolder;
        [SerializeField] GameObject inputFieldHolder;

        [Header("FOOTER")]
        [SerializeField] Button okButton;
        [SerializeField] Button cancelButton;
        [SerializeField] Button yesButton;
        [SerializeField] Button noButton;
        [Space]
        [SerializeField] Button alternativeButton;
        [SerializeField] TextMeshProUGUI alternativeButtonLabelDisplay;

        [Header("ICONS")]
        [SerializeField] List<ModalWindowIcon> icons = new List<ModalWindowIcon>();

        public static bool isOpen;

        public static Action OnOk;
        public static Action OnCancel;
        public static Action OnYes;
        public static Action OnNo;
        public static Action onOpen;
        public static Action onClose;
        public static Action onAlt;

        static Queue<ModalWindowRequest> requests = new Queue<ModalWindowRequest>();
        static ModalWindow instance;

        GameObject ogSelection;

        void Awake()
        {
            if(instance != null)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            gameObject.SetActive(false);

            OnOk = null;
            OnCancel = null;
            OnYes = null;
            OnNo = null;
            onAlt = null;
            onClose = null;
        }

        #region EDITOR FUNCTIONS
        [ContextMenu("Get References Automatically")]
        void GetReferences()
        {
            titleDisplay = transform.Find("Title Display").GetComponent<Text>();
            contentDisplay = transform.Find("Content Display").GetComponent<TextMeshProUGUI>();

            titleIconDisplay = transform.Find("Title Icon Display").GetComponent<Image>();
            contentImageDisplay = transform.Find("Content Image Display").GetComponent<RawImage>();

            okButton = transform.Find("Ok Button").GetComponent<Button>();
            cancelButton = transform.Find("Cancel Button").GetComponent<Button>();
            yesButton = transform.Find("Yes Button").GetComponent<Button>();
            noButton = transform.Find("No Button").GetComponent<Button>();
        }
        #endregion

        #region UI BUTTON FUNCTIONS
        public void OKButton()
        {
            if (OnOk != null)
                OnOk();

            Close();
        }
        public void CancelButton()
        {
            if (OnCancel != null)
                OnCancel();

            Close();
        }
        public void YesButton()
        {
            if (OnYes != null)
                OnYes();

            Close();
        }
        public void NoButton()
        {
            if (OnNo != null)
                OnNo();

            Close();
        }
        public void AltButton()
        {
            if (onAlt != null)
                onAlt();

            Close();
        }
        #endregion

        #region STATIC FUNCTIONS
        public static void Show(string content, string title = "Message", string footnote = "", ModalWindowButtons buttons = ModalWindowButtons.Ok, string titleIcon = "Info", Texture2D contentImage = null, bool showToggle = false, string toggleLabelText = "", bool showInputField = false, string inputFieldLabelText = "", bool includeAltButton = false, string altButtonLabel = "Alt", Selectable buttonToSelect = null)
        {
            requests.Enqueue(new ModalWindowRequest(content, title, footnote, buttons, titleIcon, contentImage, showToggle, toggleLabelText, showInputField, inputFieldLabelText, includeAltButton, altButtonLabel, OnOk, OnCancel, OnYes, OnNo, onClose, onAlt, buttonToSelect));

            if (requests.Count == 1)
                _show(requests.Peek());
        }
        static void _show(ModalWindowRequest request)
        {
            if (EventSystem.current == null)
            {
                ChampisConsole.LogError("Event System not found. Cannot open Modal Window.");
                return;
            }

            //If the currently selected button is not a modal window button, then cache it
            if (EventSystem.current.currentSelectedGameObject)
            {
                if (EventSystem.current.currentSelectedGameObject.name == instance.okButton.name) { }
                else if (EventSystem.current.currentSelectedGameObject.name == instance.cancelButton.name) { }
                else if (EventSystem.current.currentSelectedGameObject.name == instance.yesButton.name) { }
                else if (EventSystem.current.currentSelectedGameObject.name == instance.noButton.name) { }
                else if (EventSystem.current.currentSelectedGameObject.name == instance.alternativeButton.name) { }
                else
                    instance.ogSelection = EventSystem.current.currentSelectedGameObject;
            }
            else
                instance.ogSelection = null;

            isOpen = true;
            instance.gameObject.SetActive(true);
            instance.StartCoroutine(instance.ShowModal(request/*request.content, request.title, request.footnote, request.buttons, request.titleIcon, request.contentImage, request.showToggle, request.toggleLabelText, request.showInputField, request.inputFieldLabelText, request.includeAlt, request.altLabel*/));
        }

        public static void Close() => instance.StartCoroutine(instance._close());
        IEnumerator _close()
        {
            isOpen = false;

            onClose?.Invoke();

            OnOk = null;
            OnCancel = null;
            OnYes = null;
            OnNo = null;
            onClose = null;
            onAlt = null;

            yield return new WaitForEndOfFrame();

            Selectable selOG = requests.Peek().selectOnClose;
            requests.Dequeue();

            if (requests.Count != 0)
                _show(requests.Peek());
            else
            {
                yield return new WaitForEndOfFrame();

                if (EventSystem.current && ogSelection && selOG == null)
                    EventSystem.current.SetSelectedGameObject(ogSelection);
                if(selOG != null)
                    EventSystem.current.SetSelectedGameObject(selOG.gameObject);

                var openers = UniversalFunctions.FindInterface<IModalWindowClose>(true);

                foreach (IModalWindowClose o in openers)
                    o.OnModalWindowClose();

                yield return new WaitForEndOfFrame();
                instance.gameObject.SetActive(false);
            }
        }

        public static bool GetToggleState()
        {
            return instance.toggle.isOn;
        }
        public static string GetInputFieldText()
        {
            return instance.inputField.text;
        }
        #endregion

        #region PRIVATE FUNCTIONS
        IEnumerator ShowModal(ModalWindowRequest request/*string content, string title, string footnote, ModalWindowButtons buttons, string titleIcon, Texture2D contentImage, bool showToggle, string toggleLabelText, bool showInputField, string inputFieldLabeltext, bool includeAlt, string altLabel*/)
        {
            OnOk = request.OnOk;
            OnCancel = request.OnCancel;
            OnYes = request.OnYes;
            OnNo = request.OnNo;
            onClose = request.onClose;
            onAlt = request.onAlt;

            //Reset Toggle and Input Field
            toggle.isOn = false;
            inputField.text = string.Empty;

            //Enable Modal Window
            gameObject.SetActive(true);
            bgPanel.color = new Color(bgPanel.color.r, bgPanel.color.g, bgPanel.color.b);

            //Set Title Text
            titleDisplay.gameObject.SetActive(request.title != string.Empty);

            if (forceTitleUppercase)
                titleDisplay.text = request.title.ToUpper();
            else
                titleDisplay.text = request.title;

            //Set Title Icon
            if (request.titleIcon != null)
            {
                titleIconDisplay.gameObject.SetActive(true);
                titleIconDisplay.sprite = GetIcon(request.titleIcon);
            }
            else
                titleIconDisplay.gameObject.SetActive(false);

            //If Everything Inside the Header is Disabled, Hide the Header
            if (!titleIconDisplay.gameObject.activeInHierarchy && !titleDisplay.gameObject.activeInHierarchy)
                headerHolder.SetActive(false);

            //Set Content Text
            contentDisplay.text = request.content;

            //Set Content Image
            contentImageDisplay.gameObject.SetActive(request.contentImage != null);
            contentImageDisplay.texture = request.contentImage;

            //Set Footnote
            footnoteDisplay.gameObject.SetActive(request.footnote != string.Empty);
            footnoteDisplay.text = request.footnote;

            //Set Buttons
            switch (request.buttons)
            {
                case ModalWindowButtons.Ok:
                    okButton.gameObject.SetActive(true);
                    cancelButton.gameObject.SetActive(false);
                    yesButton.gameObject.SetActive(false);
                    noButton.gameObject.SetActive(false);
                    EventSystem.current.SetSelectedGameObject(okButton.gameObject);
                    break;
                case ModalWindowButtons.YesNo:
                    okButton.gameObject.SetActive(false);
                    cancelButton.gameObject.SetActive(false);
                    yesButton.gameObject.SetActive(true);
                    noButton.gameObject.SetActive(true);
                    EventSystem.current.SetSelectedGameObject(yesButton.gameObject);
                    break;
                case ModalWindowButtons.OkCancel:
                    okButton.gameObject.SetActive(true);
                    cancelButton.gameObject.SetActive(true);
                    yesButton.gameObject.SetActive(false);
                    noButton.gameObject.SetActive(false);
                    EventSystem.current.SetSelectedGameObject(okButton.gameObject);
                    break;
                case ModalWindowButtons.YesNoCancel:
                    okButton.gameObject.SetActive(false);
                    cancelButton.gameObject.SetActive(true);
                    yesButton.gameObject.SetActive(true);
                    noButton.gameObject.SetActive(true);
                    EventSystem.current.SetSelectedGameObject(yesButton.gameObject);
                    break;
            }

            alternativeButton.gameObject.SetActive(request.includeAlt);
            alternativeButtonLabelDisplay.text = request.altLabel;

            //Set Optional Toggle and Input Field
            toggleHolder.SetActive(request.showToggle);
            inputFieldHolder.SetActive(request.showInputField);
            toggleLabel.text = request.toggleLabelText;
            inputFieldLabel.text = request.inputFieldLabelText;

            SetButtonsNavigation(request.buttons, request.includeAlt);

            var openers = UniversalFunctions.FindInterface<IModalWindowOpen>(true);

            foreach (IModalWindowOpen o in openers)
                o.OnModalWindowOpen();

            yield return new WaitForEndOfFrame();

            LayoutRebuilder.ForceRebuildLayoutImmediate(rootLayoutGroup);

            onOpen?.Invoke();
        }

        void SetButtonsNavigation(ModalWindowButtons buttonsMode, bool useAlt)
        {
            switch (buttonsMode)
            {
                case ModalWindowButtons.Ok:
                    //Toggle SI, input SI
                    if (toggle.gameObject.activeInHierarchy && inputField.gameObject.activeInHierarchy)
                    {
                        okButton.navigation = new Navigation { mode = Navigation.Mode.Explicit, selectOnUp = inputField };
                        toggle.navigation = new Navigation { mode = Navigation.Mode.Explicit, selectOnDown = inputField };
                        inputField.navigation = new Navigation { mode = Navigation.Mode.Explicit, selectOnUp = toggle, selectOnDown = okButton };
                    }
                    //Toggle SI, input NO
                    else if (toggle.gameObject.activeInHierarchy && !inputField.gameObject.activeInHierarchy)
                    {
                        okButton.navigation = new Navigation { mode = Navigation.Mode.Explicit, selectOnUp = toggle };
                        toggle.navigation = new Navigation { mode = Navigation.Mode.Explicit, selectOnDown = okButton };
                    }
                    //Toggle NO, input SI
                    else if (!toggle.gameObject.activeInHierarchy && inputField.gameObject.activeInHierarchy)
                    {
                        okButton.navigation = new Navigation { mode = Navigation.Mode.Explicit, selectOnUp = inputField };
                        inputField.navigation = new Navigation { mode = Navigation.Mode.Explicit, selectOnDown = okButton };
                    }
                    //NINGUNO DE LOS DOS
                    else
                        okButton.navigation = new Navigation { mode = Navigation.Mode.None };

                    if (useAlt)
                    {
                        Navigation tmpNav = okButton.navigation;
                        tmpNav.selectOnRight = alternativeButton;

                        okButton.navigation = tmpNav;
                        alternativeButton.navigation = new Navigation { mode = Navigation.Mode.Explicit, selectOnLeft = okButton };
                    }
                    break;
                case ModalWindowButtons.OkCancel:
                    //Toggle SI, input SI
                    if (toggle.gameObject.activeInHierarchy && inputField.gameObject.activeInHierarchy)
                    {
                        okButton.navigation = new Navigation { mode = Navigation.Mode.Explicit, selectOnUp = inputField, selectOnRight = cancelButton };
                        cancelButton.navigation = new Navigation { mode = Navigation.Mode.Explicit, selectOnLeft = okButton, selectOnUp = inputField };

                        toggle.navigation = new Navigation { mode = Navigation.Mode.Explicit, selectOnDown = inputField };
                        inputField.navigation = new Navigation { mode = Navigation.Mode.Explicit, selectOnUp = toggle, selectOnDown = okButton };
                    }
                    //Toggle SI, input NO
                    else if (toggle.gameObject.activeInHierarchy && !inputField.gameObject.activeInHierarchy)
                    {
                        okButton.navigation = new Navigation { mode = Navigation.Mode.Explicit, selectOnUp = toggle, selectOnRight = cancelButton };
                        cancelButton.navigation = new Navigation { mode = Navigation.Mode.Explicit, selectOnLeft = okButton, selectOnUp = toggle };

                        toggle.navigation = new Navigation { mode = Navigation.Mode.Explicit, selectOnDown = okButton };
                    }
                    //Toggle NO, input SI
                    else if (!toggle.gameObject.activeInHierarchy && inputField.gameObject.activeInHierarchy)
                    {
                        okButton.navigation = new Navigation { mode = Navigation.Mode.Explicit, selectOnUp = inputField, selectOnRight = cancelButton };
                        cancelButton.navigation = new Navigation { mode = Navigation.Mode.Explicit, selectOnLeft = okButton, selectOnUp = inputField };

                        inputField.navigation = new Navigation { mode = Navigation.Mode.Explicit, selectOnDown = okButton };
                    }
                    //NINGUNO DE LOS DOS
                    else
                    {
                        okButton.navigation = new Navigation { mode = Navigation.Mode.Explicit, selectOnRight = cancelButton };
                        cancelButton.navigation = new Navigation { mode = Navigation.Mode.Explicit, selectOnLeft = okButton };
                    }

                    if (useAlt)
                    {
                        Navigation tmpNav = cancelButton.navigation;
                        tmpNav.selectOnRight = alternativeButton;

                        cancelButton.navigation = tmpNav;
                        alternativeButton.navigation = new Navigation { mode = Navigation.Mode.Explicit, selectOnLeft = cancelButton };
                    }
                    break;
                case ModalWindowButtons.YesNo:
                    //Toggle SI, input SI
                    if (toggle.gameObject.activeInHierarchy && inputField.gameObject.activeInHierarchy)
                    {
                        yesButton.navigation = new Navigation { mode = Navigation.Mode.Explicit, selectOnUp = inputField, selectOnRight = noButton };
                        noButton.navigation = new Navigation { mode = Navigation.Mode.Explicit, selectOnLeft = yesButton, selectOnUp = inputField };

                        toggle.navigation = new Navigation { mode = Navigation.Mode.Explicit, selectOnDown = inputField };
                        inputField.navigation = new Navigation { mode = Navigation.Mode.Explicit, selectOnUp = toggle, selectOnDown = yesButton };
                    }
                    //Toggle SI, input NO
                    else if (toggle.gameObject.activeInHierarchy && !inputField.gameObject.activeInHierarchy)
                    {
                        yesButton.navigation = new Navigation { mode = Navigation.Mode.Explicit, selectOnUp = toggle, selectOnRight = noButton };
                        noButton.navigation = new Navigation { mode = Navigation.Mode.Explicit, selectOnLeft = yesButton, selectOnUp = toggle };

                        toggle.navigation = new Navigation { mode = Navigation.Mode.Explicit, selectOnDown = yesButton };
                    }
                    //Toggle NO, input SI
                    else if (!toggle.gameObject.activeInHierarchy && inputField.gameObject.activeInHierarchy)
                    {
                        yesButton.navigation = new Navigation { mode = Navigation.Mode.Explicit, selectOnUp = inputField, selectOnRight = noButton };
                        noButton.navigation = new Navigation { mode = Navigation.Mode.Explicit, selectOnLeft = yesButton, selectOnUp = inputField };

                        inputField.navigation = new Navigation { mode = Navigation.Mode.Explicit, selectOnDown = yesButton };
                    }
                    //NINGUNO DE LOS DOS
                    else
                    {
                        yesButton.navigation = new Navigation { mode = Navigation.Mode.Explicit, selectOnRight = noButton };
                        noButton.navigation = new Navigation { mode = Navigation.Mode.Explicit, selectOnLeft = yesButton };
                    }

                    if (useAlt)
                    {
                        Navigation tmpNav = noButton.navigation;
                        tmpNav.selectOnRight = alternativeButton;

                        noButton.navigation = tmpNav;
                        alternativeButton.navigation = new Navigation { mode = Navigation.Mode.Explicit, selectOnLeft = noButton };
                    }
                    break;
                case ModalWindowButtons.YesNoCancel:
                    //Toggle SI, input SI
                    if (toggle.gameObject.activeInHierarchy && inputField.gameObject.activeInHierarchy)
                    {
                        yesButton.navigation = new Navigation { mode = Navigation.Mode.Explicit, selectOnUp = inputField, selectOnRight = noButton };
                        noButton.navigation = new Navigation { mode = Navigation.Mode.Explicit, selectOnLeft = yesButton, selectOnRight = cancelButton, selectOnUp = inputField };
                        cancelButton.navigation = new Navigation { mode = Navigation.Mode.Explicit, selectOnLeft = noButton, selectOnUp = inputField };

                        toggle.navigation = new Navigation { mode = Navigation.Mode.Explicit, selectOnDown = inputField };
                        inputField.navigation = new Navigation { mode = Navigation.Mode.Explicit, selectOnUp = toggle, selectOnDown = yesButton };
                    }
                    //Toggle SI, input NO
                    else if (toggle.gameObject.activeInHierarchy && !inputField.gameObject.activeInHierarchy)
                    {
                        yesButton.navigation = new Navigation { mode = Navigation.Mode.Explicit, selectOnUp = toggle, selectOnRight = noButton };
                        noButton.navigation = new Navigation { mode = Navigation.Mode.Explicit, selectOnLeft = yesButton, selectOnUp = toggle };
                        cancelButton.navigation = new Navigation { mode = Navigation.Mode.Explicit, selectOnLeft = noButton, selectOnUp = toggle };

                        toggle.navigation = new Navigation { mode = Navigation.Mode.Explicit, selectOnDown = yesButton };
                    }
                    //Toggle NO, input SI
                    else if (!toggle.gameObject.activeInHierarchy && inputField.gameObject.activeInHierarchy)
                    {
                        yesButton.navigation = new Navigation { mode = Navigation.Mode.Explicit, selectOnUp = inputField, selectOnRight = noButton };
                        noButton.navigation = new Navigation { mode = Navigation.Mode.Explicit, selectOnLeft = yesButton, selectOnUp = inputField };
                        cancelButton.navigation = new Navigation { mode = Navigation.Mode.Explicit, selectOnLeft = noButton, selectOnUp = inputField };

                        inputField.navigation = new Navigation { mode = Navigation.Mode.Explicit, selectOnDown = yesButton };
                    }
                    //NINGUNO DE LOS DOS
                    else
                    {
                        yesButton.navigation = new Navigation { mode = Navigation.Mode.Explicit, selectOnRight = noButton };
                        noButton.navigation = new Navigation { mode = Navigation.Mode.Explicit, selectOnLeft = yesButton, selectOnRight = cancelButton };
                        cancelButton.navigation = new Navigation { mode = Navigation.Mode.Explicit, selectOnLeft = noButton };
                    }

                    if (useAlt)
                    {
                        Navigation tmpNav = cancelButton.navigation;
                        tmpNav.selectOnRight = alternativeButton;

                        cancelButton.navigation = tmpNav;
                        alternativeButton.navigation = new Navigation { mode = Navigation.Mode.Explicit, selectOnLeft = cancelButton };
                    }
                    break;
            }
        }
        #endregion

        #region TITLE ICONS
        public static void AddIcon(string identifier, Sprite icon) => instance.icons.Add(new ModalWindowIcon(identifier, icon));
        public static void RemoveIcon(string identifier)
        {
            for(int i = 0; i < instance.icons.Count; i++)
                if(instance.icons[i].identifier == identifier)
                {
                    instance.icons.RemoveAt(i);
                    return;
                }
        }

        public static Sprite GetIcon(string identifier)
        {
            foreach (ModalWindowIcon ic in instance.icons)
                if (ic.identifier == identifier)
                    return ic.iconSprite;

            ChampisConsole.LogError($"Icon '{identifier}' was not found.");
            return null;
        }
        public static Sprite GetIcon(int index) { return instance.icons[index].iconSprite; }
        #endregion
    }
}
