Shader "Custom/FoodOutlineURP"
{
    Properties
    {
        _OutlineColor("Outline Color", Color) = (0, 1, 0, 1)
        _OutlineWidth("Outline Width", Range(0.001, 0.05)) = 0.01
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType" = "Opaque"
            "Queue" = "Geometry+1"
        }

        Pass
        {
            Name "Food Outline"

            Tags
            {
                "LightMode" = "SRPDefaultUnlit"
            }

            Cull Front
            ZWrite Off
            ZTest LEqual

            HLSLPROGRAM

            #pragma target 2.0
            #pragma vertex Vertex
            #pragma fragment Fragment

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
            };

            CBUFFER_START(UnityPerMaterial)
                half4 _OutlineColor;
                float _OutlineWidth;
            CBUFFER_END

            Varyings Vertex(Attributes input)
            {
                Varyings output;

                // Erst in Weltkoordinaten umwandeln.
                // Dadurch beeinflusst die Mesh-Skalierung nicht mehr
                // die Dicke der Outline.
                float3 positionWS =
                    TransformObjectToWorld(input.positionOS.xyz);

                float3 normalWS =
                    TransformObjectToWorldNormal(input.normalOS);

                positionWS +=
                    normalize(normalWS) * _OutlineWidth;

                output.positionCS =
                    TransformWorldToHClip(positionWS);

                return output;
            }

            half4 Fragment(Varyings input) : SV_Target
            {
                return _OutlineColor;
            }

            ENDHLSL
        }
    }

    Fallback Off
}