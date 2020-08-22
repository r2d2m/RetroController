using System;
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
        protected RetroControllerProfile RetroProfile { get; private set; }

        public virtual void OnAwake(RetroController retroController)
        {
            this.retroController = retroController;
            RetroProfile = retroController.Profile;
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

        /// <summary>
        /// Executes in CharacterMove function after collisions are solved.
        /// </summary>
        public abstract void OnCharacterMove();

        public virtual void OnCollisionSide(Collider collider) { }
    }
}
