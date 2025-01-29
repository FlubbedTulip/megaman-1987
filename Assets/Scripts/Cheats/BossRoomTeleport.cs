using Unity.Cinemachine;
using UnityEngine;

namespace Cheats
{
    public class BossRoomTeleport : MonoBehaviour
    {
        [SerializeField] private CinemachineCamera startCamera;
        [SerializeField] private CinemachineCamera targetCamera;
        [SerializeField] private Transform bossRoom;
        [SerializeField] private Transform player;
        

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                startCamera.Priority = 0;
                targetCamera.Priority = 1;
                player.position = bossRoom.position;
            }
        }
    }
}
