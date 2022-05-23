using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickups : MonoBehaviour
{
    enum CollectibleType
    {
        JUMP_POWERUP,
        SPEED_POWERUP,
        SCORE_COLLECTIBLE
    }

    [SerializeField] CollectibleType curCollectible;
    [SerializeField] GameObject particleEmitter;
    [SerializeField] int scoreValue;
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
                    curPlayerScript.StartCoroutine("JumpMode");
                    GameManager.Instance.score += scoreValue;
                    break;
                case CollectibleType.SPEED_POWERUP:
                    curPlayerScript.StartCoroutine("SpeedMode");
                    GameManager.Instance.score += scoreValue;
                    break;
                case CollectibleType.SCORE_COLLECTIBLE:
                    GameManager.Instance.score += scoreValue;
                    break;
            }
            particleEmitter.SetActive(true);
            particleEmitter.GetComponent<ParticleSystem>().Play();
            Destroy(gameObject, 0.2f);
        }
    }
}