using Photon.Realtime;
using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [Header("===UI 연결===")]
    [SerializeField] private Transform roomListContent;
    [SerializeField] private GameObject roomItemPrefab;

    //생성된 방 버튼들을 관리하는 딕셔너리
    private Dictionary<string, GameObject> _roomListDict = new Dictionary<string, GameObject>();

    //포톤 로비에서 방 목록 변경 사항이 생길 때마다 자동으로 호출되는 콜백
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log($"방 목록 업데이트 수신! 변동된 방 개수: {roomList.Count}");

        foreach (RoomInfo room in roomList)
        {
            //룸이 없어졌거나 숨김일 경우
            if(room.RemovedFromList || !room.IsVisible)
            {
                if(_roomListDict.ContainsKey(room.Name))
                {
                    Destroy(_roomListDict[room.Name]);
                    _roomListDict.Remove(room.Name);
                }
                continue;
            }

            //이미 룸이 있으면 텍스트만 변경
            if(_roomListDict.ContainsKey(room.Name))
            {
                _roomListDict[room.Name].GetComponent<RoomItem>().SetRoomInfo(room);
            }
            else
            {
                GameObject newRoomItem = Instantiate(roomItemPrefab, roomListContent);
                newRoomItem.GetComponent<RoomItem>().SetRoomInfo(room);

                _roomListDict.Add(room.Name, newRoomItem);
            }
        }
    }

}
