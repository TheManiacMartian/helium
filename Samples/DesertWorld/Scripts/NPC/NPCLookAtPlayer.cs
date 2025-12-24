using UnityEngine;

namespace Martian.Helium.Samples
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Collider2D))]
    public class NPCLookAtPlayer : MonoBehaviour
    {
        private SpriteRenderer _sprite;
        private Transform _playerTransform;

        private void Start()
        {
            _sprite = GetComponent<SpriteRenderer>();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            // check player
            if(collision.CompareTag("Player"))
            {
                // activate player lookat
                _playerTransform = collision.transform;
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if(collision.transform == _playerTransform)
            {
                _playerTransform = null;
            }
        }

        private void LateUpdate()
        {
            if(_playerTransform != null)
            {
                // look at player by flipped sprite based on horizontal distance
                float dist = _playerTransform.position.x - transform.position.x;

                _sprite.flipX = (dist < 0);
            }
        }

    }
}
