using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class UIFollowObject : MonoBehaviour
{
    [SerializeField] Camera mainCamera;
    [SerializeField] Transform target;
    [SerializeField] Vector3 offset;
    [Space]
    [SerializeField] UpdateType updateType;
    [Space]
    [SerializeField] bool smoothFollow;
    [SerializeField, ConditionalHide(nameof(smoothFollow), true)] float smoothAmount = 5f;

    Vector3 screenPosition;
    RectTransform rectTransform;

    private void Awake()
    {
        if (!mainCamera)
            mainCamera = Camera.main;

        rectTransform = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        screenPosition = mainCamera.WorldToScreenPoint(target.position) + offset;
        screenPosition.z = rectTransform.position.z;

        rectTransform.position = screenPosition;
    }

    private void Update()
    {
        if (updateType == UpdateType.Update)
            RefreshPosition();
    }
    private void LateUpdate()
    {
        if (updateType == UpdateType.LateUpdate)
            RefreshPosition();
    }
    private void FixedUpdate()
    {
        if (updateType == UpdateType.FixedUpdate)
            RefreshPosition();
    }

    void RefreshPosition()
    {
        screenPosition = mainCamera.WorldToScreenPoint(target.position) + offset;
        screenPosition.z = rectTransform.position.z;

        if (!smoothFollow)
            rectTransform.position = screenPosition;
        else
            rectTransform.position = Vector3.Lerp(rectTransform.position, new Vector3(screenPosition.x, screenPosition.y, screenPosition.z), smoothAmount * Time.unscaledDeltaTime);
    }
}
