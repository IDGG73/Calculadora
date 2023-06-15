using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Champis.UI;

public class ModalWindowEvents : MonoBehaviour
{
    [SerializeField] UnityEvent onOpen;
    [SerializeField] UnityEvent onClose;

    public void SendModalEvents()
    {
        ModalWindow.onOpen = InvokeOpen;
        ModalWindow.onClose = InvokeClose;
    }

    void InvokeOpen() => onOpen.Invoke();
    void InvokeClose() => onClose.Invoke();
}
