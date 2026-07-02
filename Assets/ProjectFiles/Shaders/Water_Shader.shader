Shader "Custom/Water_Gravity_Unity6_MR_Local"
{
    Properties
    {
        _ShallowColor ("Shallow Tint", Color) = (0.97, 0.99, 1.0, 1)
        _DeepColor ("Deep Tint", Color) = (0.85, 0.93, 1.0, 1)

        _FillHeight ("Water Level", Float) = 0

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
            "RenderPipeline"="UniversalPipeline"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }

        Pass
        {
            Name "ForwardLit"

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #pragma multi_compile_instancing

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;

                float3 localPos : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float3 viewDir : TEXCOORD2;

                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            float4 _ShallowColor;
            float4 _DeepColor;

            float _FillHeight;

            float _FresnelPower;
            float _DepthStrength;

            float _WaveStrength;
            float _WaveSpeed;
            float _WaveScale;

            Varyings vert (Attributes IN)
            {
                Varyings OUT;

                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

                VertexPositionInputs posInput =
                    GetVertexPositionInputs(IN.positionOS.xyz);

                VertexNormalInputs normInput =
                    GetVertexNormalInputs(IN.normalOS);

                OUT.positionHCS =
                    posInput.positionCS;

                // LOCAL POSITION
                OUT.localPos =
                    IN.positionOS.xyz;

                OUT.normalWS =
                    normInput.normalWS;

                OUT.viewDir =
                    GetWorldSpaceViewDir(
                        posInput.positionWS);

                return OUT;
            }

            half4 frag (Varyings IN)
                : SV_Target
            {
                // LOCAL SPACE WATER
                half ripple =
                    sin((IN.localPos.x +
                    IN.localPos.z)
                    * _WaveScale
                    + _Time.y *
                    _WaveSpeed)
                    * _WaveStrength;

                // LOCAL HEIGHT
                half surface =
                    _FillHeight + ripple;

                // USE LOCAL Y
                clip(surface -
                    IN.localPos.y);

                half depth =
                    saturate(
                    (surface -
                    IN.localPos.y)
                    * _DepthStrength);

                half3 waterColor =
                    lerp(
                        _ShallowColor.rgb,
                        _DeepColor.rgb,
                        depth);

                half fresnel =
                    pow(
                        1 -
                        saturate(
                            dot(
                                normalize(
                                    IN.normalWS),
                                normalize(
                                    IN.viewDir)
                            )
                        ),
                        _FresnelPower);

                half3 finalCol =
                    waterColor +
                    fresnel * 0.08;

                return half4(
                    finalCol,
                    0.75);
            }

            ENDHLSL
        }
    }
} 