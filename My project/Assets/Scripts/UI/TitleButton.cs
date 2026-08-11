using Photon.Pun;
using Photon.Pun.Demo.Cockpit;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TitleButton : MonoBehaviour
{
    [SerializeField] private GameObject nameWindow;
    [SerializeField] private GameObject roomWindow;
    [SerializeField] private TMP_InputField _playerName;
    [SerializeField] private TMP_InputField _roomName;

    public void NameWindowAppear()
    {
        nameWindow.SetActive(true);
    }

    public void RoomWindowAppear()
    {
        roomWindow.SetActive(true);
    }

    public void DecisionButtonClick()
    {
        UIManager.Instance.playerName = _playerName.text;
        UIManager.Instance.roomName = _roomName.text;


        RoomOptions options = new RoomOptions { MaxPlayers = 4, IsVisible = true, IsOpen = true };
        PhotonNetwork.CreateRoom(_roomName.text, options);

        nameWindow.SetActive(false);
    }
}
