using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

public class MovementSystem : ComponentSystem
{

    protected override void OnUpdate()
    {
        // foreach
        Entities.ForEach((ref Translation translation, ref MovementSpeedComponent movementSpeedComponent) => {
            translation.Value.y += movementSpeedComponent.moveSpeed * Time.DeltaTime;

            if(translation.Value.y > 5f) {
                movementSpeedComponent.moveSpeed = -Mathf.Abs(movementSpeedComponent.moveSpeed);
            }

            if (translation.Value.y < -5f){
                movementSpeedComponent.moveSpeed = +Mathf.Abs(movementSpeedComponent.moveSpeed);
            }



        });


    }
}
