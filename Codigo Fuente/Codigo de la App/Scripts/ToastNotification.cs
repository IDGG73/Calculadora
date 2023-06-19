using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Champis.UI
{
    public interface IToastShow
    {
        public void OnToastShow();
    }
    public interface IToastHide
    {
        public void OnToastHide();
    }

    public class ToastNotification : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI textDisplay;

        Animator animator;

        static Coroutine showCoroutine;
        static ToastNotification instance;

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            animator = GetComponent<Animator>();
        }

        public static void Show(string message, float duration = 3f)
        {
            instance.textDisplay.text = message;

            if (showCoroutine == null)
                showCoroutine = instance.StartCoroutine(instance._show(duration));
            else
            {
                instance.StopCoroutine(showCoroutine);
                showCoroutine = null;

                showCoroutine = instance.StartCoroutine(instance._show(duration));
            }
        }

        IEnumerator _show(float _duration)
        {
            yield return new WaitForEndOfFrame();
            LayoutRebuilder.ForceRebuildLayoutImmediate(gameObject.GetComponent<RectTransform>());

            instance.animator.SetTrigger("In");
            yield return new WaitForSeconds(_duration);
            instance.animator.SetTrigger("Out");

            showCoroutine = null;
            yield return null;
        }
    }

}
