using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

[RequireComponent(typeof(VideoPlayer))]
public class CinematicVolumeBinder : MonoBehaviour
{
    [SerializeField] VideoPlayer videoPlayer;

    [ContextMenu("Get Component")]
    public void _getComp() => videoPlayer = GetComponent<VideoPlayer>();

    private void Start() => SetVolumeFromSettingsManager();
    public void SetVolumeFromSettingsManager() => videoPlayer.SetDirectAudioVolume(0, SettingsManager.gameSettings.cutscenesVolume);
}
