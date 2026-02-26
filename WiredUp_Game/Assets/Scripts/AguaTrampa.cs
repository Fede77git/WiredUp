
using UnityEngine;
using UnityEngine.SceneManagement;
public class AguaTrampa : MonoBehaviour
{
    private void OnTriggerEnter(Collider otro)
    {
        
        if (otro.CompareTag("Player"))
        {
           

            
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
