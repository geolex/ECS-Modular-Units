using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;

public partial class RotationSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var RotationJob = new RotationJob();
        RotationJob.deltaTime = Time.DeltaTime;

        RotationJob.ScheduleParallel();
    }

	// Free turning entities
	struct TurningJob : IJobEntity
    {
        public float deltaTime;
        public void Execute(ref Rotation _rotation, ref TurretComponent _turret)
        {
            _turret.m_currentAngle += _turret.m_maxAngularSpeed * deltaTime;

            _rotation.Value = quaternion.RotateY(_turret.m_currentAngle);
        }
    }

    // Constrained Rotation
    partial struct RotationJob : IJobEntity
    {
        public float deltaTime;
        public void Execute(ref Rotation _rotation, ref TurretComponent _turret, ref SlotComponent _slot)
        {
            _turret.m_currentAngle += (_turret.m_maxAngularSpeed * deltaTime);

            _turret.m_currentAngle = math.clamp(_turret.m_currentAngle, _slot.m_maxAngleLeft, _slot.m_maxAngleRight);

            _rotation.Value = quaternion.RotateY(_turret.m_currentAngle);
        }
    }

}
