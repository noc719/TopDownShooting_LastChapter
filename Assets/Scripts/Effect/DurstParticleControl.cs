using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DurstParticleControl : MonoBehaviour
{
    [SerializeField] private bool createDustOnWalk = true;
    [SerializeField] private ParticleSystem dustParticleSystem;
    
    public void CreateDurstParticles()
    {
        if (createDustOnWalk) 
        {
            dustParticleSystem.Stop(); //중복 방지 중단코드
            dustParticleSystem.Play(); // 발걸음 이펙트 작동
        }
    }
}
