using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GamepadRumbleManager : MonoBehaviour
{
    [System.Serializable]
    class GamepadRumbleRequest
    {
        public string identifier;

        public float leftMotorIntensity;
        public float rightMotorIntensity;

        public float duration;

        Gamepad gamepad;
        public Gamepad targetGamepad { get { return gamepad != null ? gamepad : Gamepad.current; } }

        //Management
        public bool paused;
        public float passedLifetime;

        public GamepadRumbleRequest(string id, float left, float right, float dur, Gamepad pad)
        {
            identifier = id;

            leftMotorIntensity = left;
            rightMotorIntensity = right;

            duration = dur;
            gamepad = pad;

            //Management
            paused = false;
            passedLifetime = 0f;
        }
    }

    [System.Serializable]
    class RumbleRequestCachedGamepad
    {
        public Gamepad gamepad;

        public float leftIntensity;
        public float rightIntensity;

        public RumbleRequestCachedGamepad(Gamepad pad) => gamepad = pad;
    }

    enum GamepadRumbleFocusBehaviour { Pause, Kill, None }

    [Tooltip("What should happen to all Rumble Requests if the game loses focus?\n\n" +
        "Pause All: Pauses all Rumble Requests and resumes them once focus is recovered.\n\n" +
        "Stop All: Cancels every Rumble Request completely, with no chance of resuming them on focus gain.\n\n" +
        "None: Rumble Requests remain untouched.\n\n\n" +
        "WARNING: Some platforms and/or Unity's Input System settings may force Rumble Requests to pause on focus loss.")]
    [SerializeField] GamepadRumbleFocusBehaviour onFocusLoss;

    [Foldout("Debug", readOnly = true)]
    [SerializeField] List<GamepadRumbleRequest> debugRequests = new List<GamepadRumbleRequest>();
    [SerializeField] List<RumbleRequestCachedGamepad> gamepads = new List<RumbleRequestCachedGamepad>();
    [Space]
    [SerializeField] bool gamepadsStopped;
    [Space]
    [SerializeField] bool focused;
    [SerializeField] bool iterate;

    static GamepadRumbleManager current;

    static List<RumbleRequestCachedGamepad> cachedGamepads { get { return current.gamepads; } set { current.gamepads = value; } }
    static List<GamepadRumbleRequest> rumbleRequests { get { return current.debugRequests; } set { current.debugRequests = value; } }

    private void OnApplicationFocus(bool focus)
    {
        focused = focus;

        //Lost focus
        if (!focus)
        {
            switch (onFocusLoss)
            {
                case GamepadRumbleFocusBehaviour.Pause: PauseAllRumbles(); break;
                case GamepadRumbleFocusBehaviour.Kill: PauseAllRumbles(); KillAllRumbles(); break;
            }
        }
        //Gain focus
        else
            ResumeAllRumbles();
    }

    private void Awake()
    {
        current = this;

        if (!Application.runInBackground && onFocusLoss == GamepadRumbleFocusBehaviour.None)
        {
            ChampisConsole.Log("[Gamepad Rumble Manager] 'On Focus Loss' is set to 'None' but the app is not allowed to run in the background. Changing to 'Pause'...");
            onFocusLoss = GamepadRumbleFocusBehaviour.Pause;
        }

        InputSystem.onDeviceChange += DeviceChange;
    }

    private void OnDestroy()
    {
        InputSystem.onDeviceChange -= DeviceChange;
    }

    void DeviceChange(InputDevice device, InputDeviceChange state)
    {
        switch (state)
        {
            case InputDeviceChange.Disconnected:
                RemoveCachedGamepad(device.deviceId);
                break;
        }
    }

    private void Update()
    {
        if (!iterate)
        {
            if (!gamepadsStopped && onFocusLoss == GamepadRumbleFocusBehaviour.Pause)
            {
                gamepadsStopped = true;

                foreach (RumbleRequestCachedGamepad rrcg in cachedGamepads)
                    rrcg.gamepad.SetMotorSpeeds(0f, 0f);
            }

            return;
        }

        gamepadsStopped = false;

        for(int j = 0; j < cachedGamepads.Count; j++)
        {
            if (GamepadHasPendingRumbleRequest(cachedGamepads[j].gamepad))
            {
                for (int i = 0; i < rumbleRequests.Count; i++)
                {
                    if (!rumbleRequests[i].paused)
                    {
                        if (rumbleRequests[i].passedLifetime < rumbleRequests[i].duration)
                        {
                            rumbleRequests[i].passedLifetime += Time.deltaTime;
                            StackMotorSpeeds(rumbleRequests[i].targetGamepad);
                        }
                        else
                        {
                            StackMotorSpeeds(rumbleRequests[i].targetGamepad);
                            rumbleRequests.RemoveAt(i);

                            i--;
                        }
                    }
                }
            }
            else
                cachedGamepads[j].gamepad.SetMotorSpeeds(0f, 0f);
        }
    }

    #region AddRumble() Variants
    public static void AddRumble(float intensity) => AddRumble(intensity, intensity, 1f, string.Empty, Gamepad.current);
    public static void AddRumble(float intensity, string identifier) => AddRumble(intensity, intensity, 1f, identifier, Gamepad.current);

    public static void AddRumble(float intensity, float duration) => AddRumble(intensity, intensity, duration, string.Empty, Gamepad.current);
    public static void AddRumble(float intensity, float duration, string identifier) => AddRumble(intensity, intensity, duration, identifier, Gamepad.current);

    public static void AddRumble(float intensity, Gamepad gamepad) => AddRumble(intensity, intensity, 1f, string.Empty, gamepad);
    public static void AddRumble(float intensity, string identifier, Gamepad gamepad) => AddRumble(intensity, intensity, 1f, identifier, gamepad);

    public static void AddRumble(float intensity, float duration, Gamepad gamepad) => AddRumble(intensity, intensity, duration, string.Empty, gamepad);
    public static void AddRumble(float intensity, float duration, string identifier, Gamepad gamepad) => AddRumble(intensity, intensity, duration, identifier, gamepad);

    public static void AddRumble(float leftMotorIntensity, float rightMotorIntensity, float duration) => AddRumble(leftMotorIntensity, rightMotorIntensity, duration, string.Empty, Gamepad.current);
    public static void AddRumble(float leftMotorIntensity, float rightMotorIntensity, float duration, string identifier) => AddRumble(leftMotorIntensity, rightMotorIntensity, duration, identifier, Gamepad.current);
    #endregion

    public static void AddRumble(float leftMotorIntensity, float rightMotorIntensity, float duration, string identifier, Gamepad targetGamepad)
    {
        if (Gamepad.all.Count == 0)
            return;

        string selectedIdentifier = string.IsNullOrEmpty(identifier) ? $"Rumble ({rumbleRequests.Count})" : identifier;

        if (!string.IsNullOrEmpty(identifier))
        {
            foreach (GamepadRumbleRequest grr in rumbleRequests)
                if (grr.identifier == identifier)
                    selectedIdentifier = identifier + $" ({rumbleRequests.Count})";
        }

        rumbleRequests.Add(new GamepadRumbleRequest(selectedIdentifier, leftMotorIntensity, rightMotorIntensity, duration, targetGamepad));
        CacheGamepad(rumbleRequests[rumbleRequests.Count - 1].targetGamepad);
    }

    public static void PauseRumble(string identifier)
    {
        foreach (GamepadRumbleRequest grr in rumbleRequests)
            if (grr.identifier == identifier)
            {
                grr.paused = true;
                grr.targetGamepad.SetMotorSpeeds(0f, 0f);

                return;
            }
    }
    public static void ResumeRumble(string identifier)
    {
        foreach (GamepadRumbleRequest grr in rumbleRequests)
            if (grr.identifier == identifier)
            {
                grr.paused = false;
                return;
            }
    }

    public static void PauseAllRumbles() => current.iterate = false;
    public static void ResumeAllRumbles() => current.iterate = true;

    public static void KillRumble(string identifier)
    {
        foreach (GamepadRumbleRequest grr in rumbleRequests)
            if (grr.identifier == identifier)
            {
                grr.passedLifetime = grr.duration;
                return;
            }
    }
    public static void KillAllRumbles()
    {
        foreach (GamepadRumbleRequest grr in rumbleRequests)
            grr.passedLifetime = grr.duration;
    }
    public static void KillAllRumbles(Gamepad targetGamepad)
    {
        foreach (GamepadRumbleRequest grr in rumbleRequests)
            if (grr.targetGamepad == targetGamepad)
            {
                rumbleRequests.Remove(grr);
                return;
            }
    }
    public static void KillAllRumbles(string targetGamepadName)
    {
        foreach (GamepadRumbleRequest grr in rumbleRequests)
            if (grr.targetGamepad.name == targetGamepadName)
            {
                rumbleRequests.Remove(grr);
                return;
            }
    }
    public static void KillAllRumbles(int targetGamepadID)
    {
        foreach (GamepadRumbleRequest grr in rumbleRequests)
            if (grr.targetGamepad.deviceId == targetGamepadID)
            {
                rumbleRequests.Remove(grr);
                return;
            }
    }

    static void StackMotorSpeeds(Gamepad gamepad)
    {
        float left = 0f;
        float right = 0f;

        foreach (GamepadRumbleRequest grr in rumbleRequests)
            if (grr.targetGamepad == gamepad)
            {
                left = left + grr.leftMotorIntensity;
                right = right + grr.rightMotorIntensity;
            }

        SetMotorSpeeds(gamepad, left, right);
    }
    static void SetMotorSpeeds(Gamepad gamepad, float leftIntensity, float rightIntensity, bool notify = true)
    {
        CacheGamepad(gamepad);
        RumbleRequestCachedGamepad tmp = GetGamepadWrapper(gamepad);

        if (notify)
        {
            tmp.leftIntensity = leftIntensity;
            tmp.rightIntensity = rightIntensity;
        }

        tmp.gamepad.SetMotorSpeeds(leftIntensity, rightIntensity);
    }

    static RumbleRequestCachedGamepad GetGamepadWrapper(Gamepad gamepad)
    {
        foreach (RumbleRequestCachedGamepad rrcg in cachedGamepads)
            if (rrcg.gamepad == gamepad)
                return rrcg;

        return null;
    }

    static bool GamepadIsCached(Gamepad gamepad, bool cacheIfNotFound = true)
    {
        foreach (RumbleRequestCachedGamepad rrcg in cachedGamepads)
            if (rrcg.gamepad == gamepad)
                return true;

        if (cacheIfNotFound)
            CacheGamepad(gamepad);

        return false;
    }
    static void CacheGamepad(Gamepad gamepad)
    {
        foreach (RumbleRequestCachedGamepad rrcg in cachedGamepads)
            if (rrcg.gamepad == gamepad)
                return;

        cachedGamepads.Add(new RumbleRequestCachedGamepad(gamepad));
    }
    static void RemoveCachedGamepad(Gamepad gamepad, bool killAssociatedRumbles = true)
    {
        if (killAssociatedRumbles)
            KillAllRumbles(gamepad);

        foreach (RumbleRequestCachedGamepad cg in cachedGamepads)
            if (cg.gamepad == gamepad)
            {
                cachedGamepads.Remove(cg);
                return;
            }
    }
    static void RemoveCachedGamepad(string gamepadName, bool killAssociatedRumbles = true)
    {
        if (killAssociatedRumbles)
            KillAllRumbles(gamepadName);

        foreach (RumbleRequestCachedGamepad cg in cachedGamepads)
            if (cg.gamepad.name == gamepadName)
            {
                cachedGamepads.Remove(cg);
                return;
            }
    }
    static void RemoveCachedGamepad(int gamepadID, bool killAssociatedRumbles = true)
    {
        if (killAssociatedRumbles)
            KillAllRumbles(gamepadID);

        foreach (RumbleRequestCachedGamepad cg in cachedGamepads)
            if (cg.gamepad.deviceId == gamepadID)
            {
                cachedGamepads.Remove(cg);
                return;
            }
    }


    static bool GamepadHasPendingRumbleRequest(Gamepad gamepad)
    {
        foreach (GamepadRumbleRequest grr in rumbleRequests)
            if (grr.targetGamepad == gamepad)
                return true;

        return false;
    }
}
