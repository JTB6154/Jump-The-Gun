using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyAssignmentHelper : MonoBehaviour
{
	Button[] children;

	private void Start()
	{
		children = GetComponentsInChildren<Button>();
		updateButtonText();
	}
	public void AssignKey(int index)
	{
		Options.Instance.UpdateKeyOfIndex(index);
	}

	private void Update()
	{
		updateButtonText();
	}

	private void updateButtonText()
	{
		children[0].GetComponentInChildren<Text>().text = Options.Instance.Left.ToString();
		children[1].GetComponentInChildren<Text>().text = Options.Instance.Right.ToString();
		children[2].GetComponentInChildren<Text>().text = Options.Instance.Fire1.ToString();
		children[3].GetComponentInChildren<Text>().text = Options.Instance.Fire2.ToString();
		children[4].GetComponentInChildren<Text>().text = Options.Instance.Escape.ToString();
	}
}
