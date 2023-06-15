using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.IO;
using UnityEngine.Events;

public enum JSONKeyType { String, Int, Float, Bool, All}
public interface IJSONPlayerPrefs
{
    public void OnJSONPlayerPrefsLoad();
}

[Serializable]
public class JSONPlayerPrefsEvent<T>
{
    public T condition;
    [Space]
    public UnityEvent instruction;

    public void TryInvoke(T value)
    {
        if (value.Equals(condition))
            instruction.Invoke();
    }
    public void ForceInvoke()
    {
        instruction?.Invoke();
    }
}

[Serializable]
public class JSONPlayerPrefsEventsListWrapper<T>
{
    public JSONPlayerPrefsEvent<T>[] events;

    public void TryInvokeAll(T value)
    {
        foreach (JSONPlayerPrefsEvent<T> e in events)
            e.TryInvoke(value);
    }
    public void ForceInvokeAll()
    {
        foreach (JSONPlayerPrefsEvent<T> e in events)
            e.ForceInvoke();
    }

    public void TryInvoke(T value, int eventIndex)
    {
        events[eventIndex].TryInvoke(value);
    }
    public void ForceInvoke(int eventIndex)
    {
        events[eventIndex].ForceInvoke();
    }
}

public class JSONPlayerPrefs : MonoBehaviour
{
    #region CLASSES
    [Serializable]
    class prefKey<T>
    {
        [SerializeField] string key;
        [SerializeField] T value;

        public string GetKey() { return key; }
        public T GetValue() { return value; }

        public prefKey(string _key, T type)
        {
            key = _key;
            value = type;
        }
    }

    [Serializable]
    class prefList
    {
        public List<prefKey<string>> strings = new List<prefKey<string>>();
        public List<prefKey<int>> ints = new List<prefKey<int>>();
        public List<prefKey<float>> floats = new List<prefKey<float>>();
        public List<prefKey<bool>> bools = new List<prefKey<bool>>();
    }

    enum JSONLoadEvent { OnStart, OnAwake, Never}
    #endregion

    [SerializeField] JSONLoadEvent loadEvent;

    static prefList keysList = new prefList();

    public static bool alreadyLoaded { get; private set; }
    public static Action onJSONLoad;
    public static string filePath;
    public static JSONPlayerPrefs current;

    private void Awake()
    {
        current = this;

        //SceneManager.activeSceneChanged += LoadOnSceneChange;

        if (loadEvent == JSONLoadEvent.OnAwake)
            Load();
    }
    private void Start()
    {
        if (loadEvent == JSONLoadEvent.OnStart)
            Load();
    }

    public static bool HasKey(string key, JSONKeyType keyType = JSONKeyType.All)
    {
        int i = 0;

        if (keyType == JSONKeyType.String || keyType == JSONKeyType.All)
            for (i = 0; i < keysList.strings.Count; i++)
                if (keysList.strings[i].GetKey() == key)
                    return true;

        if (keyType == JSONKeyType.Int || keyType == JSONKeyType.All)
            for (i = 0; i < keysList.ints.Count; i++)
            if (keysList.ints[i].GetKey() == key)
                return true;

        if (keyType == JSONKeyType.Float || keyType == JSONKeyType.All)
            for (i = 0; i < keysList.floats.Count; i++)
            if (keysList.floats[i].GetKey() == key)
                return true;

        if (keyType == JSONKeyType.Bool || keyType == JSONKeyType.All)
            for (i = 0; i < keysList.bools.Count; i++)
            if (keysList.bools[i].GetKey() == key)
                return true;

        return false;
    }

    #region Setters
    public static void SetString(string key, string value)
    {
        if (!HasKey(key))
            keysList.strings.Add(new prefKey<string>(key, value));
        else
        {
            DeleteKey(key, JSONKeyType.String);
            keysList.strings.Add(new prefKey<string>(key, value));
        }
    }
    public static void SetInt(string key, int value)
    {
        if (!HasKey(key))
            keysList.ints.Add(new prefKey<int>(key, value));
        else
        {
            DeleteKey(key, JSONKeyType.Int);
            keysList.ints.Add(new prefKey<int>(key, value));
        }
    }
    public static void SetFloat(string key, float value)
    {
        if (!HasKey(key))
            keysList.floats.Add(new prefKey<float>(key, value));
        else
        {
            DeleteKey(key, JSONKeyType.Float);
            keysList.floats.Add(new prefKey<float>(key, value));
        }
    }
    public static void SetBool(string key, bool value)
    {
        if (!HasKey(key))
            keysList.bools.Add(new prefKey<bool>(key, value));
        else
        {
            DeleteKey(key, JSONKeyType.Bool);
            keysList.bools.Add(new prefKey<bool>(key, value));
        }
    }
    #endregion
    #region Getters
    public static string GetString(string key, string defaultValue = "")
    {
        foreach (prefKey<string> sk in keysList.strings)
            if (sk.GetKey() == key)
                return sk.GetValue();

        return defaultValue;
    }
    public static int GetInt(string key, int defaultValue = 0)
    {
        foreach (prefKey<int> sk in keysList.ints)
            if (sk.GetKey() == key)
                return sk.GetValue();

        return defaultValue;
    }
    public static float GetFloat(string key, float defaultValue = 0f)
    {
        foreach (prefKey<float> sk in keysList.floats)
            if (sk.GetKey() == key)
                return sk.GetValue();

        return defaultValue;
    }
    public static bool GetBool(string key, bool defaultValue = false)
    {
        foreach (prefKey<bool> sk in keysList.bools)
            if (sk.GetKey() == key)
                return sk.GetValue();

        return defaultValue;
    }

    public static string GetFilePath() => filePath;

    public static bool IsEmpty()
    {
        return keysList.strings.Count == 0 && keysList.ints.Count == 0 && keysList.floats.Count == 0 && keysList.bools.Count == 0;
    }
    #endregion

    #region Delete
    public static void DeleteKey(string key, JSONKeyType type)
    {
        int i = 0;

        if (type == JSONKeyType.String || type == JSONKeyType.All)
            for (i = 0; i < keysList.strings.Count; i++)
                if (keysList.strings[i].GetKey() == key)
                    keysList.strings.RemoveAt(i);

        if (type == JSONKeyType.Int || type == JSONKeyType.All)
            for (i = 0; i < keysList.ints.Count; i++)
            if (keysList.ints[i].GetKey() == key)
                keysList.ints.RemoveAt(i);

        if (type == JSONKeyType.Float || type == JSONKeyType.All)
            for (i = 0; i < keysList.floats.Count; i++)
            if (keysList.floats[i].GetKey() == key)
                keysList.floats.RemoveAt(i);

        if (type == JSONKeyType.Bool || type == JSONKeyType.All)
            for (i = 0; i < keysList.bools.Count; i++)
            if (keysList.bools[i].GetKey() == key)
                keysList.bools.RemoveAt(i);
    }
    public static void DeleteAllKeys()
    {
        int i = 0;

        for (i = 0; i < keysList.strings.Count; i++)
            keysList.strings.RemoveAt(i);

        for (i = 0; i < keysList.ints.Count; i++)
            keysList.ints.RemoveAt(i);

        for (i = 0; i < keysList.floats.Count; i++)
            keysList.floats.RemoveAt(i);

        for (i = 0; i < keysList.bools.Count; i++)
            keysList.bools.RemoveAt(i);
    }
    public static void DeleteAllKeysWithPattern(string pattern, JSONKeyType type)
    {
        int i = 0;

        if (type == JSONKeyType.String || type == JSONKeyType.All)
            for (i = 0; i < keysList.strings.Count; i++)
                if (keysList.strings[i].GetKey().Contains(pattern))
                    keysList.strings.RemoveAt(i);

        if (type == JSONKeyType.Int || type == JSONKeyType.All)
            for (i = 0; i < keysList.ints.Count; i++)
                if (keysList.ints[i].GetKey().Contains(pattern))
                    keysList.ints.RemoveAt(i);

        if (type == JSONKeyType.Float || type == JSONKeyType.All)
            for (i = 0; i < keysList.floats.Count; i++)
                if (keysList.floats[i].GetKey().Contains(pattern))
                    keysList.floats.RemoveAt(i);

        if (type == JSONKeyType.Bool || type == JSONKeyType.All)
            for (i = 0; i < keysList.bools.Count; i++)
                if (keysList.bools[i].GetKey().Contains(pattern))
                    keysList.bools.RemoveAt(i);
    }
    #endregion
    #region Save/Load
    public static void Load(Action<bool> loadCallback = null)
    {
        if (string.IsNullOrEmpty(filePath))
            filePath = Application.persistentDataPath + "/JSONSaves.json";

        keysList = new prefList();

        bool failed = false;

        try
        {
            keysList = JsonUtility.FromJson<prefList>(File.ReadAllText(filePath));
            CallJSONLoad();
        }
        catch { keysList = new prefList(); CallJSONLoad(); failed = true; }

        alreadyLoaded = true;
        try { loadCallback.Invoke(failed); } catch { }
    }
    static void LoadOnSceneChange(Scene o, Scene n) => CallJSONLoad();

    public static void InjectJSON(string toInject, Action<bool> callback = null)
    {
        bool failed = false;

        try
        {
            keysList = JsonUtility.FromJson<prefList>(toInject);
        }
        catch { ChampisConsole.LogError("[JSONPlayerPrefs] Unable to parse Injected JSON"); failed = true;}

        try { callback.Invoke(failed); } catch { }

        if (!failed)
            CallJSONLoad();
    }

    public static void Save(Action<bool> callback = null)
    {
        if (string.IsNullOrEmpty(filePath))
            filePath = Application.persistentDataPath + "/JSONSaves.json";

        bool failed = false;

        try { File.WriteAllText(filePath, JsonUtility.ToJson(keysList, true)); } catch { failed = true; }
        try { callback.Invoke(failed); } catch { }
    }

    static void CallJSONLoad()
    {
        List<IJSONPlayerPrefs> saveListeners = UniversalFunctions.FindInterface<IJSONPlayerPrefs>(false);

        foreach (IJSONPlayerPrefs s in saveListeners)
            try { s.OnJSONPlayerPrefsLoad(); } catch { }

        try { onJSONLoad?.Invoke(); } catch { }
    }
    #endregion
}
