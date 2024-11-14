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
            dustParticleSystem.Stop(); //�ߺ� ���� �ߴ��ڵ�
            dustParticleSystem.Play(); // �߰��� ����Ʈ �۵�
        }
    }
}
