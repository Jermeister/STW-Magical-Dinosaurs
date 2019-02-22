using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public static class ConsoleScript
{

	public static bool isConsoleActive;

	static TextMeshProUGUI textRefernce;
	static List<string> linesOfText = new List<string>();

	public static void SetUp(GameObject _textRefernceObject)
	{
		textRefernce = _textRefernceObject.GetComponent<TextMeshProUGUI>();
		ShowText();
	}

	public static void Print(string connectionType, string text)
	{
		if (!isConsoleActive)
		{
			linesOfText = new List<string>();
			textRefernce.text = "";
			return;
		}

		if (linesOfText.Count > 40)
			linesOfText.RemoveAt(0);

		string msg = "[" + connectionType + "] " + text;
		Debug.Log(msg);
		linesOfText.Add(msg);
		ShowText();
	}

	static void ShowText()
	{
		if (!isConsoleActive)
		{
			textRefernce.text = "";
			return;
		}

		if (textRefernce == null)
			return;

		textRefernce.text = "";
		for (int i = 0; i < linesOfText.Count; i++)
		{
			textRefernce.text +=  linesOfText[i] + "\n";
		}
	}

}
