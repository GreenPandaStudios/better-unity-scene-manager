using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class BetterSceneManager : MonoBehaviour
{
    /// <summary>
    /// Called when the scene with build index specified is loading
    /// </summary>
    public static Action<int> OnSceneLoading;
    /// <summary>
    /// Called when the scene with build index specified has loaded
    /// </summary>
    public static Action<int> OnSceneLoaded;

    static BetterSceneManager instance;
    private void Awake()
    {
        if (!instance)
        {
            instance = this;
            DontDestroyOnLoad(this);
            curDesiredScene = SceneManager.GetActiveScene().buildIndex;
        }

    }

    /// <summary>
    /// Called when the scene with build index specified is starting
    /// </summary>
    public static Action<int> OnSceneStarting;

    int scene;

    private static Dictionary<int, AsyncOperation> asyncops = new Dictionary<int, AsyncOperation>();
    private static int curDesiredScene = 0;

    
    public void Changescene(string sceneName)
    {
        if (BuildIndexMatrix.TryGetSceneBuildIndex(sceneName, out var index))
        {
            Changescene(index);

        }
        else
        {
            Debug.LogError("No scene called " + sceneName + " is specified in the BuildIndexMatrix");
        }
    }


    /// <summary>
    /// Changes the scene to the provided toom index
    /// </summary>
    /// <param name="scene"></param>
    public static void Changescene(int scene)
    {
        if (instance == null) return;
        curDesiredScene = scene;

        
        instance.StartCoroutine(Load((int)scene));
        StartScene(curDesiredScene);
    }
    private static IEnumerator Load(int scene)
    {
        if (!asyncops.ContainsKey(scene))
        {

            OnSceneLoading?.Invoke(scene);


            //otherwise add to the pending scenes
            AsyncOperation sceneOp = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Single);
            asyncops.Add(scene, sceneOp);
            sceneOp.allowSceneActivation = false;

            while (!sceneOp.isDone) { yield return null; }


            OnSceneLoaded?.Invoke(scene);

            asyncops.Remove(scene);
        }
    }


    private static void StartScene(int scene)
    {
        //make sure we deactivate all other scenes
        asyncops[scene].completed += (_) =>
        {
            OnSceneStarting?.Invoke(scene);
            SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(scene));
        };
        //remove from dictionary and enable the scene
        asyncops[scene].allowSceneActivation = true;



    }
}
