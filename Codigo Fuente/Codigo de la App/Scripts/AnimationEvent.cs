using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationEvent : MonoBehaviour
{
    [SerializeField] UnityEvent[] events;

    void InvokeEvent(int eventIndex) => events[eventIndex].Invoke();
}
