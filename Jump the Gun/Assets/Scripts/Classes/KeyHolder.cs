using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyHolder
{
	private KeyCode keyCode;
	public KeyCode Code{ get { return keyCode; } }
	private string location;

	public KeyHolder(string _location, string _default)
	{
		location = _location;
		keyCode = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(_location, _default));
	}

	public void SetKey(KeyCode newCode)
	{
		keyCode = newCode;
		PlayerPrefs.SetString(location, keyCode.ToString());
	}
}
