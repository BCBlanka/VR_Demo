using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blade : MonoBehaviour
{
    private AudioSource audioSource;
    private Animator animator;
    public AudioClip on;
    public AudioClip off;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
    }


    public void playOn()
    {
        audioSource.PlayOneShot(on);
        animator.SetBool("on", true);
    }
    public void playOff()
    {
        audioSource.PlayOneShot(off);
        animator.SetBool("on", false);
    }
}
