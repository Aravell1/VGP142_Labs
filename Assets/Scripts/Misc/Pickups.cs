using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickups : MonoBehaviour
{
    enum CollectibleType
    {
        JUMP_POWERUP,
        SPEED_POWERUP
    }

    [SerializeField] CollectibleType curCollectible;

    private void Start()
    {

    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Player curPlayerScript = collision.gameObject.GetComponent<Player>();
            switch (curCollectible)
            {
                case CollectibleType.JUMP_POWERUP:
                    curPlayerScript.StartJumpForceChange();
                    break;
                case CollectibleType.SPEED_POWERUP:
                    curPlayerScript.StartSpeedForceChange();
                    break;
            }
            Destroy(gameObject);
        }
    }
}