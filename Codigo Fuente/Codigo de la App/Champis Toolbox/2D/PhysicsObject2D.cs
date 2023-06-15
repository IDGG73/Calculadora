using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsObject2D : MonoBehaviour
{
    [SerializeField] AudioSource draggedAudioSource;
    [SerializeField] AudioSource collisionsAudioSource;

    [Foldout("Audio")]
    [SerializeField] AudioManagerClips hitClips;
    [SerializeField] float hitSoundMagnitude = 1.5f;

    public int Collisions { get; private set; }

    new Rigidbody2D rigidbody;

    private void FixedUpdate()
    {
        if(rigidbody == null)
        {
            rigidbody = GetComponent<Rigidbody2D>();
            return;
        }

        if (Collisions > 0)
            draggedAudioSource.volume = Mathf.Clamp(rigidbody.velocity.magnitude - 0.5f, 0f, 1f);
        else
            draggedAudioSource.volume = 0f;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Collisions++;

        if (collision.relativeVelocity.magnitude >= hitSoundMagnitude)
            hitClips.PlayRandomClip(collisionsAudioSource);
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        Collisions--;
    }
}
