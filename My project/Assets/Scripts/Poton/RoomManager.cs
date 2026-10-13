using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;

public class RoomManager : MonoBehaviourPunCallbacks
{
    [Header("==== UI References ====")]
    [SerializeField] private Transform contentTransform; //ScrollView의 Content
    [SerializeField] private GameObject playerInfoPrefab;

    // 생성된 아이템들을 관리할 리스트
    private Dictionary<int, GameObject> spawnedPlayerItems = new Dictionary<int, GameObject>();

    private void OnEnable()
    {
        RefreshPlayerList();
    }

    // 1. 내가 방에 입장했을 때 (전체 목록 셋팅)
    public override void OnJoinedRoom()
    {
        RefreshPlayerList();
    }

    //새로운 플레이어가 들어왔을 때
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"[방장 화면] 새로운 유저 입장 감지: {newPlayer.NickName}");
        AddPlayerItem(newPlayer);
    }

    //누군가 나갔을 때
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        RemovePlayerItem(otherPlayer);
    }

    public void RefreshPlayerList()
    {
        //기존에 남아있는 아이템 정리
        foreach(var pair in spawnedPlayerItems)
        {
            if(pair.Value != null)
                Destroy(pair.Value);
        }
        spawnedPlayerItems.Clear();

        //현재 방에 없으면 return
        if (PhotonNetwork.CurrentRoom == null) return;

        //현재 플레이어 확인해서 추가
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            AddPlayerItem(player);
        }
    }

    private void AddPlayerItem(Player player)
    {
        //중복 방지
        if (spawnedPlayerItems.ContainsKey(player.ActorNumber)) return;

        GameObject itemObj = Instantiate(playerInfoPrefab, contentTransform);
        PlayerItem itemScript = itemObj.GetComponent<PlayerItem>();

        if(itemObj != null )
        {
            itemScript.SetPlayerInfo(player);
        }

        spawnedPlayerItems.Add(player.ActorNumber, itemObj);
    }

    private void RemovePlayerItem(Player player)
    {
        if(spawnedPlayerItems.ContainsKey(player.ActorNumber))
        {
            Destroy(spawnedPlayerItems[player.ActorNumber]); // UI 오브젝트 삭제
            spawnedPlayerItems.Remove(player.ActorNumber);   // 딕셔너리에서 키 제거
        }
    }

    public void OnClickLeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }
    public override void OnLeftRoom()
    {
        Debug.Log("방을 나갔습니다.");
    }
}
