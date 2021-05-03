using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoomListItem : MonoBehaviour
{
	[SerializeField] TMP_Text text;
	[SerializeField] TMP_Text countText;

	public RoomInfo info;

	public void SetUp(RoomInfo _info)
	{
		info = _info;
		text.text = _info.Name;
		countText.text = _info.PlayerCount.ToString() + "/12";
	}

	public void OnClick()
	{
		Launcher.Instance.JoinRoom(info);
	}
}