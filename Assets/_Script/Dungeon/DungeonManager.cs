using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class DungeonManager : MonoBehaviour
{
    private void Start()
    {
    }

    public void successDungeon()
    {

        SceneManager.LoadSceneAsync("Main");
    }
}
