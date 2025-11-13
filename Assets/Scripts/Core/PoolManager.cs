using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    [SerializeField] private ParticleSystemPoolConfig particleSystemPoolConfig;
    [SerializeField] private AudioSourcePoolConfig audioSourceConfig;
    [SerializeField] private LineRendererConfig lineRendererConfig;

    private ObjectPool<ParticleSystem> particlesPool;
    private ObjectPool<AudioSource> audioSources;
    private ObjectPool<LineRenderer> lineRendererPool;

    public void InitializePools()
    {
        audioSources = InitializePool(audioSourceConfig.audioSource, audioSourceConfig.initialSize);
        particlesPool = InitializePool(particleSystemPoolConfig.particlePrefab, particleSystemPoolConfig.initialSize);
        lineRendererPool = InitializePool(lineRendererConfig.lineRendererPrefab, lineRendererConfig.initialSize);
    }

    public ParticleSystem GetParticlesFromPool()
    {
        return particlesPool.Get();
    }

    public ParticleSystem GetParticlesFromPool(Vector3 position)
    {
        return particlesPool.Get(position);
    }

    public void ReturnParticleToPool(ParticleSystem particle)
    {
        particlesPool.Return(particle);
    }

    public void ReturnParticleToPool(ParticleSystem particle, float delayTime)
    {
        StartCoroutine(ReturnToPoolCoroutine(particlesPool, particle, delayTime));
    }

    public AudioSource GetAudioSource()
    {
        return audioSources.Get();
    }
    
    public void ReturnAudioSourceToPool(AudioSource audioSource)
    {
        audioSources.Return(audioSource);
    }

    public void ReturnAudioSourceToPool(AudioSource audioSource, float delayTime)
    {
        StartCoroutine(ReturnToPoolCoroutine(audioSources, audioSource, delayTime));
    }

    public LineRenderer GetLineRendererFromPool()
    {
        return lineRendererPool.Get();
    }

    public void ReturnLineRendererToPool(LineRenderer lineRenderer)
    {
        lineRendererPool.Return(lineRenderer);
    }

    private ObjectPool<T> InitializePool<T>(T prefab, int initialSize) where T : Component
    {
        var parent = new GameObject(prefab.name + "Pool");
        parent.transform.SetParent(transform);
        return new ObjectPool<T>(prefab, parent.transform, initialSize);
    }
    private IEnumerator ReturnToPoolCoroutine<T>(ObjectPool<T> pool, T obj, float delay)  where T : Component
    {
        yield return new WaitForSeconds(delay);
        pool.Return(obj);
    }
}