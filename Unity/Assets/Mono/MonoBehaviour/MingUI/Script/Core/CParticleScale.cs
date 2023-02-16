using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class CParticleScale : MonoBehaviour {
    private ParticleSystem _particleSystem;
    private TrailRenderer trail;
    public void ResetParticleSystem() {
        _particleSystem = GetComponent<ParticleSystem>();
        if (_particleSystem != null) {
            ScaleShurikenSystems(UIRoot.StandardScale);
        }

         trail = GetComponent<TrailRenderer>();
         if (trail != null) { 
            ScaleTrailRenderers(UIRoot.StandardScale);
        }
    }

    void ScaleShurikenSystems(float scaleFactor) {
        //_particleSystem.startSpeed *= scaleFactor;
        //_particleSystem.startSize *= scaleFactor;
        //_particleSystem.gravityModifier *= scaleFactor;
        var main = _particleSystem.main;
        var startSize =main.startSize;
        startSize.curveMultiplier *= scaleFactor;
        startSize.constant *= scaleFactor;
        startSize.constantMin *= scaleFactor;

        main.startSize = startSize;


        var startSpeed = main.startSpeed;
        startSpeed.curveMultiplier *= scaleFactor;
        startSpeed.constant *= scaleFactor;
        startSpeed.constantMax *= scaleFactor;
        startSpeed.constantMin *= scaleFactor;

        main.startSpeed = startSpeed;

        var shape = _particleSystem.shape;
        shape.scale *= scaleFactor;
        //so.FindProperty("VelocityModule.x.scalar").floatValue *= scaleFactor;
        //so.FindProperty("VelocityModule.y.scalar").floatValue *= scaleFactor;
        //so.FindProperty("VelocityModule.z.scalar").floatValue *= scaleFactor;
        //so.FindProperty("ClampVelocityModule.magnitude.scalar").floatValue *= scaleFactor;
        //so.FindProperty("ClampVelocityModule.x.scalar").floatValue *= scaleFactor;
        //so.FindProperty("ClampVelocityModule.y.scalar").floatValue *= scaleFactor;
        //so.FindProperty("ClampVelocityModule.z.scalar").floatValue *= scaleFactor;
        //so.FindProperty("ForceModule.x.scalar").floatValue *= scaleFactor;
        //so.FindProperty("ForceModule.y.scalar").floatValue *= scaleFactor;
        //so.FindProperty("ForceModule.z.scalar").floatValue *= scaleFactor;
        //so.FindProperty("ColorBySpeedModule.range").vector2Value *= scaleFactor;
        //so.FindProperty("SizeBySpeedModule.range").vector2Value *= scaleFactor;
        //so.FindProperty("RotationBySpeedModule.range").vector2Value *= scaleFactor;

        //so.ApplyModifiedProperties();
    }

    void ScaleTrailRenderers(float scaleFactor) {
            trail.startWidth *= scaleFactor;
            trail.endWidth *= scaleFactor;
    }  
}
