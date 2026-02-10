Shader "Custom/Water_Gravity"
{
    Properties
    {
        _ShallowColor ("Shallow Tint", Color) = (0.97, 0.99, 1.0, 0.06)
        _DeepColor ("Deep Tint", Color) = (0.85, 0.93, 1.0, 0.20)

        _FillHeight ("Water Level", Float) = 0
        _ContainerPos ("Container Position", Vector) = (0,0,0,0)

        _FresnelPower ("Rim Light Power", Range(0,10)) = 4
        _DepthStrength ("Depth Darkening", Range(0,10)) = 1.5
        _SurfaceSmooth ("Surface Softness", Range(0,0.03)) = 0.012

        _WaveStrength ("Tiny Ripples", Range(0,0.02)) = 0.002
        _WaveSpeed ("Ripple Speed", Range(0,5)) = 0.6
        _WaveScale ("Ripple Scale", Range(0,10)) = 3

        _IntersectionFade ("Glass Fade", Range(0,10)) = 1
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _CameraDepthTexture;

            struct appdata { float4 vertex : POSITION; float3 normal : NORMAL; };
            struct v2f {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD0;
                float3 normal : TEXCOORD1;
                float3 viewDir : TEXCOORD2;
                float4 screenPos : TEXCOORD3;
            };

            float4 _ShallowColor, _DeepColor;
            float _FillHeight, _FresnelPower, _DepthStrength, _SurfaceSmooth;
            float _WaveStrength, _WaveSpeed, _WaveScale, _IntersectionFade;
            float4 _ContainerPos;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.normal = normalize(UnityObjectToWorldNormal(v.normal));
                o.viewDir = normalize(_WorldSpaceCameraPos - o.worldPos);
                o.screenPos = ComputeScreenPos(o.pos);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float3 gravityUp = float3(0,1,0);

                float ripple = sin((i.worldPos.x + i.worldPos.z) * _WaveScale + _Time.y * _WaveSpeed) * _WaveStrength;
                float height = dot(i.worldPos - _ContainerPos.xyz, gravityUp);
                float surface = _FillHeight + ripple;

                if (height > surface) discard;

                float depth = saturate((surface - height) * _DepthStrength);
                float3 waterColor = lerp(_ShallowColor.rgb, _DeepColor.rgb, depth);

                float fresnel = pow(1 - saturate(dot(i.normal, i.viewDir)), _FresnelPower);
                float alpha = lerp(_ShallowColor.a, _DeepColor.a, depth);

                float surfaceFade = smoothstep(surface, surface - _SurfaceSmooth, height);

                float sceneDepth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.screenPos)));
                float waterDepth = i.screenPos.w;
                float intersection = saturate((sceneDepth - waterDepth) * _IntersectionFade);

                alpha *= surfaceFade * intersection;
                float3 finalCol = waterColor + fresnel * 0.08;

                return float4(finalCol, alpha);
            }
            ENDCG
        }
    }
}
