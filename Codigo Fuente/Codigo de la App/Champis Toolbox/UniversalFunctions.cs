using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XInput;

public static class RendererExtensions
{
    public static bool IsVisibleFrom(this Renderer renderer, Camera camera)
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
        return GeometryUtility.TestPlanesAABB(planes, renderer.bounds);
    }
}
public static class IListExtensions
{
    /// <summary>
    /// Shuffles the element order of the specified list.
    /// </summary>
    public static void Shuffle<T>(this IList<T> ts)
    {
        var last = ts.Count - 1;

        for (var i = 0; i < last; ++i)
        {
            var r = UnityEngine.Random.Range(i, ts.Count);
            var tmp = ts[i];
            ts[i] = ts[r];
            ts[r] = tmp;
        }
    }

    public static void Swap<T>(this IList<T> list, int indexA, int indexB)
    {
        T tmp = list[indexA];

        list[indexA] = list[indexB];
        list[indexB] = tmp;
    }

    public static bool IsInsideArrayBounds<T>(this IList<T> array, int valueToCheck)
    {
        if (valueToCheck > -1 && array.Count > 0)
            return true;
        else
            return false;
    }
}

public enum GamepadInputSystemButtons { none, yButton, aButton, bButton, xButton, leftBumper, rightBumper, leftTrigger, rightTrigger, dpadUp, dpadDown, dpadLeft, dpadRight, leftStickButton, rightStickButton, select, start }
public enum GamepadLayout { Auto, Xbox, PlayStation, Switch }
public enum InputSource { Mouse, Keyboard, Gamepad, TouchScreen }

public static class UniversalFunctions
{
    public static List<T> FindInterface<T>(bool includeInactive = true)
    {
        List<T> interfaces = new List<T>();
        GameObject[] rootGameObjects = SceneManager.GetActiveScene().GetRootGameObjects();

        foreach (var rootGameObject in rootGameObjects)
        {
            T[] childrenInterfaces = rootGameObject.GetComponentsInChildren<T>(includeInactive);

            foreach (var childInterface in childrenInterfaces)
                interfaces.Add(childInterface);
        }

        return interfaces;
    }

    public static T GetPropertyValue<T>(object obj, string propName) { return (T)obj.GetType().GetProperty(propName).GetValue(obj, null); }

    public static GameObject[] FindObjectsInLayer(int layerIndex)
    {
        var goArray = GameObject.FindObjectsOfType<GameObject>();
        var goList = new List<GameObject>();

        for (var i = 0; i < goArray.Length; i++)
            if (goArray[i].layer == layerIndex)
                goList.Add(goArray[i]);

        return goList.ToArray();
    }

    public static int RandomNumberExcludingCurrent(int currentNumber, int maxExclusive)
    {
        return (currentNumber + UnityEngine.Random.Range(1, maxExclusive - 1)) % maxExclusive;
    }

    public static void CloneClass(this object dst, object src)
    {
        var srcT = src.GetType();
        var dstT = dst.GetType();

        foreach (var f in srcT.GetFields())
        {
            var dstF = dstT.GetField(f.Name);

            if (dstF == null || dstF.IsLiteral)
                continue;

            dstF.SetValue(dst, f.GetValue(src));
        }

        foreach (var f in srcT.GetProperties())
        {
            var dstF = dstT.GetProperty(f.Name);

            if (dstF == null)
                continue;

            dstF.SetValue(dst, f.GetValue(src, null), null);
        }
    }

    private static float WrapAngle(float angle)
    {
        angle %= 360;
        if (angle > 180)
            return angle - 360;

        return angle;
    }
    private static float UnwrapAngle(float angle)
    {
        if (angle >= 0)
            return angle;

        angle = -angle % 360;

        return 360 - angle;
    }

    public static bool InputSystemButtonWasPressedThisFrame(GamepadInputSystemButtons button)
    {
        if (Gamepad.current == null)
            return false;

        switch (button)
        {
            case GamepadInputSystemButtons.yButton:
                if (Gamepad.current.buttonNorth.wasPressedThisFrame)
                    return true;
                break;
            case GamepadInputSystemButtons.aButton:
                if (Gamepad.current.buttonSouth.wasPressedThisFrame)
                    return true;
                break;
            case GamepadInputSystemButtons.bButton:
                if (Gamepad.current.buttonEast.wasPressedThisFrame)
                    return true;
                break;
            case GamepadInputSystemButtons.xButton:
                if (Gamepad.current.buttonWest.wasPressedThisFrame)
                    return true;
                break;
            case GamepadInputSystemButtons.leftBumper:
                if (Gamepad.current.leftShoulder.wasPressedThisFrame)
                    return true;
                break;
            case GamepadInputSystemButtons.rightBumper:
                if (Gamepad.current.rightShoulder.wasPressedThisFrame)
                    return true;
                break;
            case GamepadInputSystemButtons.leftTrigger:
                if (Gamepad.current.leftTrigger.wasPressedThisFrame)
                    return true;
                break;
            case GamepadInputSystemButtons.rightTrigger:
                if (Gamepad.current.rightTrigger.wasPressedThisFrame)
                    return true;
                break;
            case GamepadInputSystemButtons.dpadUp:
                if (Gamepad.current.dpad.up.wasPressedThisFrame)
                    return true;
                break;
            case GamepadInputSystemButtons.dpadDown:
                if (Gamepad.current.dpad.down.wasPressedThisFrame)
                    return true;
                break;
            case GamepadInputSystemButtons.dpadLeft:
                if (Gamepad.current.dpad.left.wasPressedThisFrame)
                    return true;
                break;
            case GamepadInputSystemButtons.dpadRight:
                if (Gamepad.current.dpad.right.wasPressedThisFrame)
                    return true;
                break;
            case GamepadInputSystemButtons.leftStickButton:
                if (Gamepad.current.leftStickButton.wasPressedThisFrame)
                    return true;
                break;
            case GamepadInputSystemButtons.rightStickButton:
                if (Gamepad.current.rightStickButton.wasPressedThisFrame)
                    return true;
                break;
            case GamepadInputSystemButtons.select:
                if (Gamepad.current.selectButton.wasPressedThisFrame)
                    return true;
                break;
            case GamepadInputSystemButtons.start:
                if (Gamepad.current.startButton.wasPressedThisFrame)
                    return true;
                break;
        }

        return false;
    }

    public static bool InputSystemGamepadWasPressedThisFrame(GamepadInputSystemButtons button)
    {
        if (Gamepad.all.Count == 0)
        {
            ChampisConsole.LogError("There are no Gamepads plugged-in");
            return false;
        }

        foreach (Gamepad g in Gamepad.all)
        {
            switch (button)
            {
                case GamepadInputSystemButtons.yButton:
                    if (g.buttonNorth.wasPressedThisFrame)
                        return true;
                    break;
                case GamepadInputSystemButtons.aButton:
                    if (g.buttonSouth.wasPressedThisFrame)
                        return true;
                    break;
                case GamepadInputSystemButtons.bButton:
                    if (g.buttonEast.wasPressedThisFrame)
                        return true;
                    break;
                case GamepadInputSystemButtons.xButton:
                    if (g.buttonWest.wasPressedThisFrame)
                        return true;
                    break;
                case GamepadInputSystemButtons.leftBumper:
                    if (g.leftShoulder.wasPressedThisFrame)
                        return true;
                    break;
                case GamepadInputSystemButtons.rightBumper:
                    if (g.rightShoulder.wasPressedThisFrame)
                        return true;
                    break;
                case GamepadInputSystemButtons.leftTrigger:
                    if (g.leftTrigger.wasPressedThisFrame)
                        return true;
                    break;
                case GamepadInputSystemButtons.rightTrigger:
                    if (g.rightTrigger.wasPressedThisFrame)
                        return true;
                    break;
                case GamepadInputSystemButtons.dpadUp:
                    if (g.dpad.up.wasPressedThisFrame)
                        return true;
                    break;
                case GamepadInputSystemButtons.dpadDown:
                    if (g.dpad.down.wasPressedThisFrame)
                        return true;
                    break;
                case GamepadInputSystemButtons.dpadLeft:
                    if (g.dpad.left.wasPressedThisFrame)
                        return true;
                    break;
                case GamepadInputSystemButtons.dpadRight:
                    if (g.dpad.right.wasPressedThisFrame)
                        return true;
                    break;
                case GamepadInputSystemButtons.leftStickButton:
                    if (g.leftStickButton.wasPressedThisFrame)
                        return true;
                    break;
                case GamepadInputSystemButtons.rightStickButton:
                    if (g.rightStickButton.wasPressedThisFrame)
                        return true;
                    break;
                case GamepadInputSystemButtons.select:
                    if (g.selectButton.wasPressedThisFrame)
                        return true;
                    break;
                case GamepadInputSystemButtons.start:
                    if (g.startButton.wasPressedThisFrame)
                        return true;
                    break;
            }
        }

        return false;
    }
    public static Gamepad InputSystemGetGamepadByButton(GamepadInputSystemButtons button)
    {
        if (Gamepad.all.Count == 0)
        {
            ChampisConsole.LogError("There are no Gamepads plugged-in");
            return null;
        }

        foreach(Gamepad g in Gamepad.all)
        {
            switch (button)
            {
                case GamepadInputSystemButtons.yButton:
                    if (g.buttonNorth.wasPressedThisFrame)
                        return g;
                    break;
                case GamepadInputSystemButtons.aButton:
                    if (g.buttonSouth.wasPressedThisFrame)
                        return g;
                    break;
                case GamepadInputSystemButtons.bButton:
                    if (g.buttonEast.wasPressedThisFrame)
                        return g;
                    break;
                case GamepadInputSystemButtons.xButton:
                    if (g.buttonWest.wasPressedThisFrame)
                        return g;
                    break;
                case GamepadInputSystemButtons.leftBumper:
                    if (g.leftShoulder.wasPressedThisFrame)
                        return g;
                    break;
                case GamepadInputSystemButtons.rightBumper:
                    if (g.rightShoulder.wasPressedThisFrame)
                        return g;
                    break;
                case GamepadInputSystemButtons.leftTrigger:
                    if (g.leftTrigger.wasPressedThisFrame)
                        return g;
                    break;
                case GamepadInputSystemButtons.rightTrigger:
                    if (g.rightTrigger.wasPressedThisFrame)
                        return g;
                    break;
                case GamepadInputSystemButtons.dpadUp:
                    if (g.dpad.up.wasPressedThisFrame)
                        return g;
                    break;
                case GamepadInputSystemButtons.dpadDown:
                    if (g.dpad.down.wasPressedThisFrame)
                        return g;
                    break;
                case GamepadInputSystemButtons.dpadLeft:
                    if (g.dpad.left.wasPressedThisFrame)
                        return g;
                    break;
                case GamepadInputSystemButtons.dpadRight:
                    if (g.dpad.right.wasPressedThisFrame)
                        return g;
                    break;
                case GamepadInputSystemButtons.leftStickButton:
                    if (g.leftStickButton.wasPressedThisFrame)
                        return g;
                    break;
                case GamepadInputSystemButtons.rightStickButton:
                    if (g.rightStickButton.wasPressedThisFrame)
                        return g;
                    break;
                case GamepadInputSystemButtons.select:
                    if (g.selectButton.wasPressedThisFrame)
                        return g;
                    break;
                case GamepadInputSystemButtons.start:
                    if (g.startButton.wasPressedThisFrame)
                        return g;
                    break;
            }
        }

        return null;
    }
    public static Gamepad GetGamepadByID(int id)
    {
        foreach (Gamepad g in Gamepad.all)
            if (g.deviceId == id)
                return g;

        return null;
    }

    public static KeyCode CurrentPressedKey()
    {
        foreach (KeyCode kcode in Enum.GetValues(typeof(KeyCode)))
            if (Input.GetKey(kcode))
                return kcode;

        return KeyCode.None;
    }

    public static IEnumerator Blink(SpriteRenderer sprite, int blinks, float blinkThreshold)
    {
        for (int i = 0; i <= blinks; i++)
        {
            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 0);
            yield return new WaitForSeconds(blinkThreshold);
            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 1);
            yield return new WaitForSeconds(blinkThreshold);
        }
    }

    public static string Vector3ToString(Vector3 vector)
    {
        var culture = (System.Globalization.CultureInfo)System.Globalization.CultureInfo.CurrentCulture.Clone();
        culture.NumberFormat.NumberDecimalSeparator = ".";

        string bake = vector.x.ToString(culture) + "," + vector.y.ToString(culture) + "," + vector.z.ToString(culture);
        return bake;
    }
    public static Vector3 ParseVector3(string cont)
    {
        if (cont == string.Empty)
            return Vector3.zero;

        string[] split = null;

        float parsedX = 0f;
        float parsedY = 0f;
        float parsedZ = 0f;

        try
        {
            Vector3 bake = new Vector3();

            split = cont.Split(',');

            if (split.Length != 3)
            {
                ChampisConsole.LogWarning($"The specified string '{cont}' couldn't be parsed. Was it generated with 'UniversalFunctions.Vector3ToString()'?");
                return bake;
            }

            var culture = (System.Globalization.CultureInfo)System.Globalization.CultureInfo.CurrentCulture.Clone();
            culture.NumberFormat.NumberDecimalSeparator = ".";

            parsedX = float.Parse(split[0], culture);
            parsedY = float.Parse(split[1], culture);
            parsedZ = float.Parse(split[2], culture);

            bake = new Vector3(parsedX, parsedY, parsedZ);
            return bake;
        }
        catch (Exception e)
        {
            ChampisConsole.LogError($"Something went wrong while trying to parse '{cont}': " + e);
            return Vector3.zero;
        }
    }
    public static Vector3 ParseVector3(string cont, Vector3 defaultVector3)
    {
        Vector3 result = ParseVector3(cont);

        if (result == Vector3.zero)
            return defaultVector3;

        return result;
    }

    public static bool IntToBool(int i)
    {
        if (i == 0)
            return false;
        else
            return true;
    }
    public static int BoolToInt(bool b)
    {
        if (b == false)
            return 0;
        else
            return 1;
    }

    public static Vector2 BezierLerp(float t, Vector2 a, Vector2 b, Vector2 c)
    {
        var ab = Vector2.Lerp(a, b, t);
        var bc = Vector2.Lerp(b, c, t);

        return Vector2.Lerp(ab, bc, t);
    }

    public static IEnumerator WaitForAnimation(Animator animator, string animationStateName, int animatorLayer = 0)
    {
        animator.Play(animationStateName, 0);
        float t = 0;

        while(t < animator.GetCurrentAnimatorStateInfo(animatorLayer).length)
        {
            if(animator.updateMode == AnimatorUpdateMode.Normal)
                t += Time.deltaTime;
            if (animator.updateMode == AnimatorUpdateMode.UnscaledTime)
                t += Time.unscaledDeltaTime;

            yield return new WaitForEndOfFrame();
        }
    }

    public static void FlipDirection(Transform objectToFlip, bool turnLeft)
    {
        if (turnLeft)
        {
            objectToFlip.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
        }
        else
        {
            objectToFlip.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        }
    }

    public static void AlternateDirection(Transform objectToFlip)
    {
        objectToFlip.rotation = Quaternion.Euler(new Vector3(0, objectToFlip.rotation.eulerAngles.y + 180, 0));
    }

    public static Quaternion LookAt2D(Vector2 forward)
    {
        return Quaternion.Euler(0, 0, Mathf.Atan2(forward.y, forward.x) * Mathf.Rad2Deg);
    }

    public static bool CompareCollisionAngle2D(Collision2D collision, Vector3 expectedDirection, float degreesMargin = 30f)
    {
        for (int i = 0; i < collision.contacts.Length; i++)
            if (Vector3.Angle(collision.contacts[i].normal, expectedDirection) <= degreesMargin)
                return true;

        return false;
    }
    public static bool CompareCollisionAngle3D(Collision collision, Vector3 expectedDirection, float degreesMargin = 30f)
    {
        for (int i = 0; i < collision.contacts.Length; i++)
            if (Vector3.Angle(collision.contacts[i].normal, expectedDirection) <= degreesMargin)
                return true;

        return false;
    }

    /// <summary>
    /// Sets the rumble intensity of the first gamepad. One intensity value is set for both motors and the change happens instantly.
    /// </summary>
    /// <param name="intensity">Intensity goes from 0 to 1 and is applied for both left and right motors.</param>
    public static void SetGamepadRumbleSimple(float intensity)
    {
        if (!SettingsManager.gameSettings.gamepadRumble && intensity != 0)
            return;

        if(Gamepad.current != null)
            Gamepad.current.SetMotorSpeeds(intensity, intensity);
    }

    /// <summary>
    /// Sets the rumble intensity of the first gamepad. One intensity value for each motor. The change happens instantly.
    /// </summary>
    /// <param name="leftIntensity">The intensity for the left gamepad motor.</param>
    /// <param name="rightIntensity">The intensity for the right gamepad motor.</param>
    public static void SetGamepadRumbleAdvanced(float leftIntensity, float rightIntensity)
    {
        if (!SettingsManager.gameSettings.gamepadRumble)
            return;

        if (Gamepad.current != null)
            Gamepad.current.SetMotorSpeeds(leftIntensity, rightIntensity);
        /*
        else if (SystemInfo.deviceType != DeviceType.Desktop)
            XboxOneGamepad.current.SetMotorSpeeds(leftIntensity, rightIntensity, 0, 0);*/
    }

    /// <summary>
    /// Sets the rumble intensity of the first gamepad. One intensity value for each motor. The change happens instantly.
    /// </summary>
    /// <param name="leftIntensity">The intensity for the left gamepad motor.</param>
    /// <param name="rightIntensity">The intensity for the right gamepad motor.</param>
    /// <param name="leftTriggerIntensity">The intensity for the left trigger motor.</param>
    public static void SetGamepadRumbleAdvancedWithTriggers(float leftIntensity, float rightIntensity, float leftTriggerIntensity, float rightTriggerIntensity)
    {
        if (!SettingsManager.gameSettings.gamepadRumble)
            return;

        if (Gamepad.current != null)
            Gamepad.current.SetMotorSpeeds(leftIntensity, rightIntensity);
        /*
        else if (SystemInfo.deviceType != DeviceType.Desktop)
            XboxOneGamepad.current.SetMotorSpeeds(leftIntensity, rightIntensity, leftTriggerIntensity, rightTriggerIntensity);*/
    }

    public static IEnumerator LerpTimeScale(float newTimeScale, float speed = 1)
    {
        float time = 0;

        while(time < 1)
        {
            time += speed * Time.unscaledDeltaTime;
            Time.timeScale = Mathf.Lerp(Time.timeScale, newTimeScale, time);

            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForEndOfFrame();
    }

    /// <summary>
    /// Slowly fades in the rumble intensity, holds still for some seconds, and then slowly fades out. This is a coroutine, so make sure to call this using 'StartCoroutine()'.
    /// </summary>
    /// <param name="leftIntensity">The left gamepad motor intensity.</param>
    /// <param name="rightIntensity">The right gamepad motor intensity.</param>
    /// <param name="duration">Amount of seconds to hold the rumble in its higher value.</param>
    /// <param name="fadeIn">How fast the gamepad reaches its highest point. Don't put 0, that would freeze the coroutine.</param>
    /// <param name="fadeOut">How fast the gamepad goes back to 0. Don't put 0, that would freeze the coroutine.</param>
    public static IEnumerator SetGamepadRumbleSmooth(float leftIntensity, float rightIntensity, float duration, float fadeIn, float fadeOut)
    {
        if(fadeIn <= 0f || fadeOut <= 0f)
        {
            ChampisConsole.Log("'FadeIn' or 'FadeOut' was '0'. The coroutine could freeze. Aborting...");
            yield break;
        }

        if(SettingsManager.gameSettings.gamepadRumble)
        {
            bool fadingIn = true;
            bool fadingOut = false;

            float currentLeft = 0;
            float currentRight = 0;

            while (fadingIn)
            {
                if (currentLeft < leftIntensity)
                {
                    currentLeft += Time.deltaTime * fadeIn;
                }
                if (currentRight < rightIntensity)
                {
                    currentRight += Time.deltaTime * fadeIn;
                }
             
                if (Gamepad.current != null)
                    Gamepad.current.SetMotorSpeeds(currentLeft, currentRight);
                /*
                else if(SystemInfo.deviceType != DeviceType.Desktop)
                    XboxOneGamepad.current.SetMotorSpeeds(currentLeft, currentRight, 0, 0);*/

                if (currentLeft >= leftIntensity && currentRight >= rightIntensity)
                {
                    fadingIn = false;
                }

                yield return new WaitForEndOfFrame();
            }
           
            if (Gamepad.current != null)
                Gamepad.current.SetMotorSpeeds(currentLeft, currentRight);
            /*
            else if (SystemInfo.deviceType != DeviceType.Desktop)
                XboxOneGamepad.current.SetMotorSpeeds(currentLeft, currentRight, 0, 0);*/

            yield return new WaitForSeconds(duration);

            fadingOut = true;

            while (fadingOut)
            {
                if (currentLeft > 0)
                {
                    currentLeft -= Time.deltaTime * fadeOut;
                }
                if (currentRight > 0)
                {
                    currentRight -= Time.deltaTime * fadeOut;
                }

                if (Gamepad.current != null)
                    Gamepad.current.SetMotorSpeeds(currentLeft, currentRight);
                /*
                else if (SystemInfo.deviceType != DeviceType.Desktop)
                    XboxOneGamepad.current.SetMotorSpeeds(currentLeft, currentRight, 0, 0);*/

                if (currentLeft <= 0 && currentRight <= 0)
                {
                    fadingOut = false;
                }

                yield return new WaitForEndOfFrame();
            }

            if (Gamepad.current != null)
                Gamepad.current.SetMotorSpeeds(0, 0);
            /*
            else if (SystemInfo.deviceType != DeviceType.Desktop)
                XboxOneGamepad.current.SetMotorSpeeds(0, 0, 0, 0);*/

            yield return new WaitForEndOfFrame();
        }
        else
        {
            yield return new WaitForEndOfFrame();
        }
    }

    public static IEnumerator SetGamepadRumbleSimpleWithDuration(float leftIntensity, float rightIntensity, float duration)
    {
        if (Gamepad.current != null)
            Gamepad.current.SetMotorSpeeds(leftIntensity, rightIntensity);
        /*
        else if (SystemInfo.deviceType != DeviceType.Desktop)
            XboxOneGamepad.current.SetMotorSpeeds(leftIntensity, rightIntensity, leftIntensity, rightIntensity);*/

        yield return new WaitForSeconds(duration);

        if (Gamepad.current != null)
            Gamepad.current.SetMotorSpeeds(0, 0);
        /*
        else if (SystemInfo.deviceType != DeviceType.Desktop)
            XboxOneGamepad.current.SetMotorSpeeds(0, 0, 0, 0);*/
    }
}
