using Unity.Entities;
using Unity.Burst;
using Unity.Transforms;
using Unity.Mathematics;

public partial struct InstantiateSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Instantiator>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EndSimulationEntityCommandBufferSystem.Singleton commandBufferSystem =
            SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();

        // Schedule job
        InstantiateJob instantiateJob = new()
        {
            commandBuffer = commandBufferSystem.CreateCommandBuffer(state.WorldUnmanaged),
            minPos = -10.0f,
            maxPos = 10.0f
        };
        state.Dependency = instantiateJob.Schedule(state.Dependency);
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }

    [BurstCompile]
    private partial struct InstantiateJob : IJobEntity
    {
        public EntityCommandBuffer commandBuffer;
        public float minPos;
        public float maxPos;

        public void Execute(ref Instantiator instantiator)
        {
            if (instantiator.instantiated)
            {
                // Already instantiated
                return;
            }

            // Prepare random generator
            Random random = new(123456);

            for (int i = 0; i < instantiator.instanceCount; i++)
            {
                Entity instance = this.commandBuffer.Instantiate(instantiator.entityPrefab);

                // Random position at x and z
                float3 position = new()
                {
                    x = random.NextFloat(this.minPos, this.maxPos),
                    z = random.NextFloat(this.minPos, this.maxPos)
                };

                // Random euler rotation but only at y
                float3 euler = new()
                {
                    y = random.NextFloat(0.0f, 360.0f)
                };
                quaternion rotation = quaternion.Euler(euler);

                // Set LocalTransform
                this.commandBuffer.SetComponent(instance,
                    LocalTransform.FromPositionRotation(position, rotation));
            }

            // We set this to true so it will no longer be processed on the next frame
            instantiator.instantiated = true;
        }
    }
}