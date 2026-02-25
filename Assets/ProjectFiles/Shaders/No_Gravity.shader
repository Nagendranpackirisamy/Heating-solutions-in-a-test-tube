Shader "Custom/Water_NoGravity_Unity6"
{
    Properties
    {
        _ShallowColor ("Shallow Tint", Color) = (0.97, 0.99, 1.0, 1)
        _DeepColor ("Deep Tint", Color) = (0.85, 0.93, 1.0, 1)

        _FillHeight ("Water Level (Local Y)", Float) = 0

        _FresnelPower ("Rim Light Power", Range(0,10)) = 4
        _DepthStrength ("Depth Darkening", Range(0,10)) = 1.5

        _WaveStrength ("Tiny Ripples", Range(0,0.02)) = 0.002
        _WaveSpeed ("Ripple Speed", Range(0,5)) = 0.6
        _WaveScale ("Ripple Scale", Range(0,10)) = 3
    }

    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
            "Queue"="Geometry"
            "RenderPipeline"="UniversalPipeline"
        }

        Pass
        {
            Tags { "LightMode"="UniversalForward" }

            ZWrite On
            Cull Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD0;
                float3 localPos : TEXCOORD1;
                float3 normalWS : TEXCOORD2;
                float3 viewDir : TEXCOORD3;
            };

            float4 _ShallowColor, _DeepColor;
            float _FillHeight, _FresnelPower, _DepthStrength;
            float _WaveStrength, _WaveSpeed, _WaveScale;

            v2f vert (appdata v)
            {
                v2f o;

                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.localPos = v.vertex.xyz;
                o.normalWS = UnityObjectToWorldNormal(v.normal);
                o.viewDir = normalize(_WorldSpaceCameraPos - o.worldPos);

                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                half ripple = sin((i.localPos.x + i.localPos.z) * _WaveScale
                                  + _Time.y * _WaveSpeed) * _WaveStrength;

                half surface = _FillHeight + ripple;

                clip(surface - i.localPos.y);

                half depth = saturate((surface - i.localPos.y) * _DepthStrength);
                half3 waterColor = lerp(_ShallowColor.rgb, _DeepColor.rgb, depth);

                half fresnel = pow(1 - saturate(dot(normalize(i.normalWS), normalize(i.viewDir))), _FresnelPower);

                half3 finalCol = waterColor + fresnel * 0.08;

                return half4(finalCol, 1.0);
            }

            ENDHLSL
        }
    }
}