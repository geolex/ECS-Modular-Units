using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;

public class VelocitySystem : JobComponentSystem
{
    struct VelocityJob : IJobForEach<Translation, VelocityComponent>
    {
        public float deltaTime;
        public void Execute(ref Translation _translation, ref VelocityComponent _velocity)
        {
            _translation.Value += _velocity.m_value * deltaTime;
        }
    }
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var velocityJob = new VelocityJob();
        velocityJob.deltaTime = Time.deltaTime;
        return velocityJob.Schedule(this, inputDeps);

    }
}