using UnityEngine;

namespace vnc
{
    /// <summary>
    /// Base class for implementing custom movement for the Retro Controller
    /// </summary>
    public abstract class RetroMovement : MonoBehaviour
    {
        /// <summary>
        /// Executes the movement on the controller.
        /// Check if it the controller has the necessary conditions to perform the movement and execute it.
        /// When a movement is executed, it skips all the others in the list. Check out the Manual to see
        /// how it works in depth.
        /// </summary>
        /// <param name="retroController">
        /// The RetroController being used to perform the custom movement
        /// </param>
        /// <returns>
        /// Return a boolean value as to if the movement passed the check with success.
        /// </returns>
        public abstract bool DoMovement(RetroController retroController);
    }
}
