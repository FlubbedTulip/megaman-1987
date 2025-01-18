using TMPro;
using UnityEngine;

public class debegTest : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] Rigidbody2D rg;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        text.text = $"time scale:{Time.timeScale}\n velocity: {rg.linearVelocity}";

        
    }
}
