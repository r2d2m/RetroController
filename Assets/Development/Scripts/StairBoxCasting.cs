using UnityEngine;
using vnc;

public class StairBoxCasting : MonoBehaviour
{
    public RetroControllerProfile Profile;
    BoxCollider _boxCollider;

    void Start()
    {
        _boxCollider = GetComponent<BoxCollider>();
    }

    private void FixedUpdate()
    {
                
    }
}
