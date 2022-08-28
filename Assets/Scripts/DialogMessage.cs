using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogMessage : MonoBehaviour
{
    [SerializeField] private GameObject dialog;

    public void OpenDialog(){
        Time.timeScale = 0f;
        dialog.SetActive(true);
    }

    public void CloseDialog(){
        Time.timeScale = 1f;
        dialog.SetActive(false);
    }
}
