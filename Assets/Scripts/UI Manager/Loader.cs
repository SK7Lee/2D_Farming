using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Loader : MonoBehaviour
{
    public void LoadScene(int sceneIndex)
    {
        StartCoroutine(LoadAsynchronously(sceneIndex));

    }

    IEnumerator LoadAsynchronously (int sceneIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);

        while (!operation.isDone) 
        {
            
            Debug.Log(operation.progress);
            yield return null;

        }
    }

}
