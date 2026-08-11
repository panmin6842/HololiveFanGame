using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class RoomItem : MonoBehaviour
{
    [SerializeField] private TMP_Text roomInfoText;
    private string _roomName;

    //방 정보를 UI텍스트에
    public void SetRoomInfo(RoomInfo info)
    {
        _roomName = info.Name;
        roomInfoText.text = $"{info.Name} ({info.PlayerCount} / {info.MaxPlayers})";
    }

    //방 이동
    public void OnClickRoomItem()
    {
        PhotonNetwork.JoinRoom(_roomName);
    }
}
