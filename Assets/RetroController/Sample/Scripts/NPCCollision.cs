using UnityEngine;

namespace epiplon.Samples
{
    public class NPCCollision : MonoBehaviour
    {
        public RetroController retroController;
        public Collider collidingNpcCollider;
        public Collider ignoredNpcCollider;

        private void Start()
        {
            if (retroController)
            {
                retroController.AddIgnoredCollider(ignoredNpcCollider);
            }
        }
    }
}
