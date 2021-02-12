using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;



public class Testing : MonoBehaviour
{
    [SerializeField] private int entityCount = 5;

    [SerializeField] private Mesh sharedMesh;
    [SerializeField] private Material sharedMaterial;

    private void Start()
    {

        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        // here, we're building an 'Archetype', which is a set of components we want together
        var entityArchetype = entityManager.CreateArchetype(
            typeof(EntropyComponent),
            typeof(Translation),
            typeof(MovementSpeedComponent),


            // for rendering
            typeof(RenderMesh),
            typeof(RenderBounds),
            typeof(WorldRenderBounds),
            typeof(LocalToWorld)
            );

        NativeArray<Entity> entityArray = new NativeArray<Entity>(entityCount, Allocator.Temp);
        entityManager.CreateEntity(entityArchetype, entityArray);

        for(int i = 0; i < entityArray.Length; i++)
        {
            var entity = entityArray[i];

            // hunger
            entityManager.SetComponentData(entity
                , new EntropyComponent {
                    value = UnityEngine.Random.Range(10, 20)
                });

            entityManager.SetComponentData(entity
                , new MovementSpeedComponent {
                    moveSpeed = UnityEngine.Random.Range(0.1f, 2)
            });

            entityManager.SetComponentData(entity
                , new Translation {
                    Value = new float3(
                        UnityEngine.Random.Range(-8f, 8f), 
                        UnityEngine.Random.Range(-4f, 4f), 
                        0f                
                    )
            });

            entityManager.SetSharedComponentData(entity, new RenderMesh
            {
                mesh = sharedMesh,
                material = sharedMaterial
            });
        }

        //entityArray.Dispose();
    }


    private void Demo1()
    {
        // Entities hold components
        // components are structs
        // systems operate on components

        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        // The hypothetical system here, is something that will degrade over time        
        var entity = entityManager.CreateEntity(
            typeof(EntropyComponent)            
            );

        // start at 100%
        entityManager.SetComponentData(entity, new EntropyComponent { value = 1 });
    }




}
