using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

[CreateAssetMenu(fileName = "Scene Management", menuName = "Build-index Matrix", order = 0)]
public class BuildIndexMatrix : ScriptableObject
{
    [System.Serializable]
    struct BuildIndexMapper
    {
        public string SceneName;
        public int SceneBuildIndex;
    }
    private static BuildIndexMatrix instance;

    [Tooltip("You must set the buld indexes of the scenes here. All scenes will be switched to by the name specified here")]
    [SerializeField] List<BuildIndexMapper> indexes = new List<BuildIndexMapper>();

   

    private static Dictionary<string, int> buildIndexMatrix = new Dictionary<string, int>();

    /// <summary>
    /// Constructs the build index matrix that we use to pull build indexes from scene names
    ///<see cref="buildIndexMatrix"/>
    /// </summary>
    private static void ConstructBuildIndexMatrix()
    {
        buildIndexMatrix.Clear();
        foreach (BuildIndexMapper indexMapper in instance.indexes)
        {
           if (!buildIndexMatrix.ContainsKey(indexMapper.SceneName))
            {
                buildIndexMatrix.Add(indexMapper.SceneName, indexMapper.SceneBuildIndex);
            }
            else
            {
                Debug.LogError(indexMapper.SceneName + " has already been declared as a scene with a build index. We cannot specify it twice");
            }
        }


        //make sure every scene has a name
        foreach (string sceneName in arrayOfNames)
        {
            if (!buildIndexMatrix.ContainsKey(sceneName))
            {
                Debug.LogWarning(sceneName + " was not given an index in " + instance);
            }
        }
    }

    private void OnEnable()
    {
        //set the singleton instance
        instance = this;
        GetOpenScenes();
        ConstructBuildIndexMatrix();
    }


    /// <summary>
    /// Try's to give the build index of the scene with the specified scene name
    /// </summary>
    /// <param name="sceneName"></param>
    /// <param name="buildIndex"></param>
    /// <returns>true if the index exists, false otherwise</returns>
    public static bool TryGetSceneBuildIndex(string sceneName, out int buildIndex)
    {
        buildIndex = -1;
        if (buildIndexMatrix.ContainsKey(sceneName))
        {
            buildIndex = buildIndexMatrix[sceneName];
        }


        return (buildIndex == -1);
    }


    private static string[] arrayOfNames;
    /// <summary>
    /// Gets an array of all the current scene names
    /// </summary>
    private static void GetOpenScenes()
    {
        instance.indexes.Clear();


        var sceneNumber = SceneManager.sceneCountInBuildSettings;
        arrayOfNames = new string[sceneNumber]; ;
        for (int i = 0; i < sceneNumber; i++)
        {
            arrayOfNames[i] = Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i));
            instance.indexes.Add(new BuildIndexMapper
            {
                SceneBuildIndex = i,
                SceneName = arrayOfNames[i]
            });
        }
    }




}
