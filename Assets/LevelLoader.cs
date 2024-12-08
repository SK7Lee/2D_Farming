//using UnityEngine;

//public class LevelLoader : MonoBehaviour
//{
//   public void LoadLevel (int sceneIndex)
//    {
//        StartCoroutine(LoadAsynchronously(sceneIndex));
//    }

//    IEnumerator LoadAsynchronously (int sceneIndex)
//    {
//        AsyncOperation operation = SceneManager.LoadSceneAsyunc(sceneIndex);

//        while (!operation.isDone) 
//        {
//            Debug.Log(operation.progress);

//            yield return null;
//        }

    
//}
