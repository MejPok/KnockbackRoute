using UnityEngine;

public class ParticleHolder : MonoBehaviour
{
    public GameObject[] particles;

    public void useParticle(Transform transPos, int num)
    {
        var particle = Instantiate(particles[num], transPos.position, transPos.rotation);
        var particleSystem = particle.GetComponent<ParticleSystem>();
        var main = particleSystem.main;
        main.stopAction = ParticleSystemStopAction.Destroy;
        particleSystem.Play();
        
    }
}
