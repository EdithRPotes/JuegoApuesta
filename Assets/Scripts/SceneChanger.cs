using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public void ChangeScene(string sceneName)
    {
        Debug.Log("Changing to scene: " + sceneName);
        SceneManager.LoadScene(sceneName);
    }
}
