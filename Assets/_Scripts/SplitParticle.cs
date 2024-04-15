using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplitParticle : MonoBehaviour
{
    private ParticleSystem particles;

    private void Awake()
    {
        this.particles = GetComponent<ParticleSystem>();
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(this.PlayAndDestroyParticles());
    }

    private IEnumerator PlayAndDestroyParticles()
    {
        this.particles.Play();

        yield return new WaitForSeconds(2.0f);

        Destroy(this.gameObject);
    }    
}
