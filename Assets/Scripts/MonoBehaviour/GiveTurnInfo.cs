using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GiveTurnInfo : MonoBehaviour
{
  private Text info;

    private void Awake()
    {
        info = gameObject.GetComponentInChildren<Text>();
    }

    private void Update()
    {
        info.text = GameManager.isWhiteTurn == true ? "WHITE" : "BLACK";
    }
}
