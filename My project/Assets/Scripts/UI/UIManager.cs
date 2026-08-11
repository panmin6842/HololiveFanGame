using UnityEngine;

public class UIManager : MonoBehaviour
{
    private static UIManager instance = null;
    public static UIManager Instance { get => instance; }

    public string playerName;
    public string roomName;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(instance);

        DontDestroyOnLoad(instance);
    }
}
