using UnityEngine;
using UnityEngine.SceneManagement;


namespace HeatingSolutionsInaTestTube
{
    public class SceneManagers : MonoBehaviour
    {
        public void LoadSceneByName(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
    }
    
    
}