Shader "Custom/Squircle"
{
    Properties
    {
        _Color ("Color", Color) = (1,0,0,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        [Toggle] _UsePixelRadius ("Use Pixel Radius", Float) = 0
        _Radius ("Radius %", Range(0.0, 0.5)) = 0.15
        _PixelRadius ("Radius in Pixels", Float) = 10.0
        _Power ("Power", Float) = 2.0
        _ScreenWidth ("Screen Width", Float) = 1920
        _ScreenHeight ("Screen Height", Float) = 1080
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100

        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        CGPROGRAM
        #pragma surface surf Standard alpha:fade
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
            float4 screenPos;
        };

        fixed4 _Color;
        float _Power;
        float _Radius;
        float _PixelRadius;
        float _UsePixelRadius;
        float _ScreenWidth;
        float _ScreenHeight;

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Center the UV coordinates in [-0.5, 0.5] range
            float2 uv = IN.uv_MainTex - 0.5;
            
            // Calculate max distance to corner in this coordinate system
            float maxDist = pow(0.5, _Power) + pow(0.5, _Power);
            
            // Determine radius based on mode
            float effectiveRadius;
            
            if (_UsePixelRadius > 0.5) {
                // Calculate radius in normalized space based on pixels
                float2 screenSize = float2(_ScreenWidth, _ScreenHeight);
                float2 pixelSize = 1.0 / screenSize;
                
                // Convert pixel radius to UV space
                float2 objectSize = float2(
                    unity_ObjectToWorld[0].x, 
                    unity_ObjectToWorld[1].y
                ) * 2.0; // Assuming the quad is scaled by objectToWorld
                
                float pixelRadiusNormalized = _PixelRadius / min(objectSize.x * screenSize.x, objectSize.y * screenSize.y);
                effectiveRadius = 1.0 - (pixelRadiusNormalized / 0.5);
                effectiveRadius = clamp(effectiveRadius, 0.0, 1.0);
            } else {
                // Use the percentage-based radius
                effectiveRadius = 1.0 - _Radius;
            }
            
            // Calculate the threshold
            float threshold = maxDist * effectiveRadius;
            
            // Calculate the distance using the power value
            float dist = pow(abs(uv.x), _Power) + pow(abs(uv.y), _Power);
            
            // Create the shape with anti-aliasing to fix pixelation
            float edge = 0.01; // Anti-aliasing edge width
            float shape = smoothstep(threshold + edge, threshold - edge, dist);
            
            // Apply the color and alpha
            o.Albedo = _Color.rgb;
            o.Alpha = shape;
        }
        ENDCG
    }
    FallBack "Transparent"
}