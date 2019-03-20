using UnityEngine;
using vnc;

public class IgnoreCollisionTest : MonoBehaviour
{
    public RetroController retroController;
    public Collider playerCollider;
    public Collider npcCollider;
    public bool ignoring;

    private void Start()
    {
        retroController = GetComponent<RetroController>();
        playerCollider = retroController.GetComponent<Collider>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            ignoring = !ignoring;
            if (ignoring)
            {
                retroController.AddIgnoredCollider(npcCollider);
            }
            else
            {
                retroController.RemoveIgnoredCollider(npcCollider);
            }
        }
    }
}
