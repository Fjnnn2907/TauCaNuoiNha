using UnityEngine;
using UnityEngine.SceneManagement;

public class BorderCollide : MonoBehaviour
{
    [SerializeField] private int sceneId;
    void OnTriggerEnter2D(Collider2D collision)
    {
        SceneManager.LoadScene(sceneId);
    }
}