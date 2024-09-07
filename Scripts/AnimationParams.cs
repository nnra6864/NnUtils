using System;
using UnityEngine;

namespace NnUtils.Scripts
{
    [Serializable]
    public struct AnimationParams
    {
        [Header("Position")]
        public Vector3 Position;
        public bool AdditivePosition;
        public float PositionDuration;
        public Easings.Type PositionEasing;
        
        [Header("Rotation")]
        public Vector3 Rotation;
        public bool AdditiveRotation;
        public float RotationDuration;
        public Easings.Type RotationEasing;
        
        [Header("Scale")]
        public Vector3 Scale;
        public bool AdditiveScale;
        public float ScaleDuration;
        public Easings.Type ScaleEasing;
        public bool Unscaled;

        public AnimationParams(
            Vector3 position,
            Vector3 rotation,
            Vector3 scale,
            float duration,
            Easings.Type easing = Easings.Type.None,
            bool unscaled = false,
            bool additive = false)
        {
            Position = position;
            Rotation = rotation;
            Scale = scale;
            PositionDuration = RotationDuration = ScaleDuration = duration;
            PositionEasing = RotationEasing = ScaleEasing = easing;
            Unscaled = unscaled;
            AdditivePosition = AdditiveRotation = AdditiveScale = additive;
        }

        public AnimationParams(
            Vector3 position,
            bool additivePosition,
            float positionDuration,
            Easings.Type positionEasing,
            Vector3 rotation,
            bool additiveRotation,
            float rotationDuration,
            Easings.Type rotationEasing,
            Vector3 scale,
            bool additiveScale,
            float scaleDuration,
            Easings.Type scaleEasing,
            bool unscaled = false)
        {
            Position = position;
            AdditivePosition = additivePosition;
            PositionDuration = positionDuration;
            PositionEasing = positionEasing;
            
            Rotation = rotation;
            AdditiveRotation = additiveRotation;
            RotationDuration = rotationDuration;
            RotationEasing = rotationEasing;
            
            Scale = scale;
            AdditiveScale = additiveScale;
            ScaleDuration = scaleDuration;
            ScaleEasing = scaleEasing;
            
            Unscaled = unscaled;
        }
    }
}