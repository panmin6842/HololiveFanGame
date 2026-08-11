using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    //싱글톤
    private static PhotonManager instance = null;
    public static PhotonManager Instance { get => instance; }

    [Header("====Info====")]
    [SerializeField] string _roomVersion = "1.0.0";
    [SerializeField] string _roomName = "Room";

    [Header("====동기화할 Player====")]
    [SerializeField] private GameObject _player;

    //플레이어 오브젝트
    public GameObject photonPlayer => _player;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(instance);

        DontDestroyOnLoad(instance);

        F_MasterClient();
    }

    private void F_MasterClient()
    {
        PhotonNetwork.AutomaticallySyncScene = true; //마스터 클라이어언트가 다른 씬으로 이동 시 다른 멤버들도 자동 이동

        //게임 버전 할당
        PhotonNetwork.GameVersion = _roomVersion;
        Debug.Log("포톤 서버 초당 데이터 통신 수 : " + PhotonNetwork.SendRate);

        //포튼 사용자들을 포튼 서버에 연결
        if(!PhotonNetwork.IsConnected)
            PhotonNetwork.ConnectUsingSettings();
    }

    //서버의 마스터(나)가 서버에 접속 성공 시
    public override void OnConnectedToMaster()
    {
        //서버 마스터가 접속하면 메서드 실행
        Debug.Log("포톤에 접속 성공 유무 : " + PhotonNetwork.IsConnected);

        //로비 입장 함수 실행
        PhotonNetwork.JoinLobby();
    }

    //로비 접속 시
    public override void OnJoinedLobby()
    {
        Debug.Log("로비 입장 완료!");
    }

    //방 입장
    public void OnClickCreatOrJoinRoom()
    {
        JointRoom();
    }

    private void JointRoom()
    {
        //방 정보 생성
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 3;
        roomOptions.IsOpen = true;
        roomOptions.IsVisible = true;

        //생성 후 바로 입장
        PhotonNetwork.JoinOrCreateRoom(_roomName, roomOptions, TypedLobby.Default);
    }

    public override void OnCreatedRoom()
    {
        //방 생성 시
        Debug.Log("방 생성 완료 : " + PhotonNetwork.CurrentRoom.Name);
    }

    public override void OnJoinedRoom()
    {
        //방 입장 시
        Debug.Log($"{PhotonNetwork.CurrentRoom.Name} 에 입장 / 방에 있는지 여부 : {PhotonNetwork.InRoom} " +
            $"인원 수 : {PhotonNetwork.CurrentRoom.PlayerCount}");

        //StartCoroutine(CreatePlayer());
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        //방 접속 실패 시
        Debug.Log($"JoinRoom Failed {returnCode} {message} / 방 없음 -> 생성시도중....");
        JointRoom();
    }

    IEnumerator CreatePlayer()
    {
        yield return new WaitForSeconds(3);

        _player = PhotonNetwork.Instantiate("Mio", new Vector3(0, 0, 0), Quaternion.identity);

        if(_player == null )
        {
            Debug.Log("플레이어가 없습니다");
            yield break;
        }
    }
}
