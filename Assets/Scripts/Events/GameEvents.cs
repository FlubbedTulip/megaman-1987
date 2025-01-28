
using System;
using UnityEngine;

namespace Events
{
    public static class GameEvents
    {
        public static Action<Vector3> OnEnemyDied;
        public static Action PlayerDeath;
        public static Action BossDeath;
    }
}
