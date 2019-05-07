using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Setting_Menu : MonoBehaviour
{
    public void ChangeScene (string scene)
    {
        Application.LoadLevel(scene);
    }
}
