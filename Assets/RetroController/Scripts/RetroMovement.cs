using UnityEngine;

namespace vnc
{
    /// <summary>
    /// Base class for implementing custom movement for the Retro Controller
    /// </summary>
    public abstract class RetroMovement : MonoBehaviour
    {
        [SerializeField]
        bool isActive = true;
        public bool IsActive
        {
            get { return isActive; }
            set { isActive = value; }
        }

        protected RetroController retroController;

        public virtual void OnAwake(RetroController retroController)
        {
            this.retroController = retroController;
        }

        /// <summary>
        /// <para>Executes the movement on the controller, if it meets the necessary conditions.</para>
        /// <para>
        /// When a movement is executed, it skips all the others in the list. Check out the Manual to see
        /// how it works in depth.
        /// </para>
        /// </summary>
        /// <param name="retroController">
        /// The RetroController being used to perform the custom movement
        /// </param>
        /// <returns>
        /// Return a boolean value as to if the movement passed the check with success.
        /// </returns>
        public abstract bool DoMovement();

        public abstract void OnCharacterMove();
    }
}
