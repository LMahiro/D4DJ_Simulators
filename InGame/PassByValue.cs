using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PassByValue : MonoBehaviour
{
    private static PassByValue instance;
    public static PassByValue Instance => instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
            Destroy(this.gameObject);
    }

    public Chart chart;

    public void SwitchingScenes(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
