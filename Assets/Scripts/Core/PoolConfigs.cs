using System;
using UnityEngine;

[Serializable]
public class AudioSourcePoolConfig
{
    public AudioSource audioSource;
    public int initialSize = 10;
}

[Serializable]
public class ParticleSystemPoolConfig
{
    public ParticleSystem particlePrefab;
    public int initialSize = 10;
}

[Serializable]
public class LineRendererConfig
{
    public LineRenderer lineRendererPrefab;
    public int initialSize = 10;
}