Shader "Custom/LiquidShader"
{
    Properties
    {
        _Color ("Water Color", Color) = (0,0.5,1,0.5)
        _Fill ("Fill Level", Range(-1,1)) = 0
        _WaveHeight ("Wave Height", Range(0,0.1)) = 0.02
        _WaveSpeed ("Wave Speed", Range(0,10)) = 2
        _WaveFrequency ("Wave Frequency", Range(0,10)) = 3
        _Gravity ("Gravity Direction", Vector) = (0,1,0,0)
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD0;
            };

            float4 _Color;
            float _Fill;
            float _WaveHeight;
            float _WaveSpeed;
            float _WaveFrequency;
            float4 _Gravity;

            v2f vert(appdata v)
            {
                v2f o;

                float3 world = mul(unity_ObjectToWorld, v.vertex).xyz;

                // Gravity direction (object space)
                float3 gravityDir = normalize(_Gravity.xyz);

                // Height along gravity axis
                float height = dot(v.vertex.xyz, gravityDir);

                // Wave animation
                float wave =
                    sin(height * _WaveFrequency + _Time.y * _WaveSpeed) * _WaveHeight;

                world += gravityDir * wave;

                o.worldPos = world;
                o.pos = UnityObjectToClipPos(v.vertex);

                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                float3 localPos = i.worldPos;

                // Fill mask based on gravity direction (Y-like clipping but rotated)
                float fillMask = localPos.y + _Fill;

                clip(fillMask);

                return _Color;
            }

            ENDHLSL
        }
    }
}