using UnityEngine;

public class Ladder : MonoBehaviour
{
    [Header("Ladder Positions")]
    public Transform topPosition;
    public Transform bottomPosition;

    [Header("Exit Positions")]
    public Transform topExitPosition;
    public Transform bottomExitPosition;

    [Header("Edge collider")]
    public Collider2D edgeCollider;

}