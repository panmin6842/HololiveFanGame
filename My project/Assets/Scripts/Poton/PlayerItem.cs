using UnityEngine;
using TMPro;
using Photon.Realtime;

public class PlayerItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;

    public void SetPlayerInfo(Player player)
    {
        string nickName = player.NickName;

        if(player.IsMasterClient)
        {
            nickName += " Master";
        }
        nameText.text = nickName;
    }
}
