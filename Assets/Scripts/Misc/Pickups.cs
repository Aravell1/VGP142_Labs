using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickups : MonoBehaviour
{

    enum CollectibleType
    {
        JUMP_POWERUP,
        SPEED_POWERUP,
        SCORE_COLLECTIBLE,
        HEAL_POWERUP
    }

    [SerializeField] CollectibleType curCollectible;
    [SerializeField] GameObject particleEmitter;
    [SerializeField] AudioClip pickupSound;
    [SerializeField] int scoreValue;

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Player curPlayerScript = collision.gameObject.GetComponent<Player>();
            switch (curCollectible)
            {
                case CollectibleType.JUMP_POWERUP:
                    curPlayerScript.StartCoroutine("JumpMode");
                    Debug.Log("Jump Height increased");
                    break;
                case CollectibleType.SPEED_POWERUP:
                    curPlayerScript.StartCoroutine("SpeedMode");
                    Debug.Log("Player Speed increased");
                    break;
                case CollectibleType.SCORE_COLLECTIBLE:
                    Debug.Log("You Win!");
                    break;
                case CollectibleType.HEAL_POWERUP:
                    GameManager.Instance.health++;
                    Debug.Log("Healed 1 HP");
                    break;
            }
            GameManager.Instance.score += scoreValue;
            SoundManager.Instance.Play(pickupSound);
            particleEmitter.GetComponent<ParticleSystem>().Play();

            Destroy(gameObject, 0.2f);
        }
    }
}