using Managers;
using TMPro;
using UnityEngine;

namespace Bootstrap
{
    public class MainSceneObjects : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI screenMessage;
        [SerializeField] private GameObject playerObject;

    

        private void Start()
        {
            // Once the scene starts, pass references to the GameManager
            GameManager.Instance.SetMainSceneReferences(scoreText, screenMessage, playerObject);
        }
    }
}
