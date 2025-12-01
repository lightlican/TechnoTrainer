using UnityEngine;
using UnityEngine.SceneManagement;

public class Scenes : MonoBehaviour  // ÏÅĞÅÈÌÅÍÎÂÀËÈ ÊËÀÑÑ
{
    public void ChangeScenes(int numberScenes)
    {
        SceneManager.LoadScene(numberScenes);
    }


    public void Exit()
    {
        Application.Quit();
    }

}