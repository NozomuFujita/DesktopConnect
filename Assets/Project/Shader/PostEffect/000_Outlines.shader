Shader "Unlit/000_Outlines"
{
    Properties
    {
        [Header(outline)]
        _OutlineColor("OutlineColor", Color) = (0, 0, 0, 1)
        _OutlineThickness("OutlineThickness", Range(0.0, 1.0)) = 1.0
        _OutlineZ("Outline(Z-Axis Move)", Range(0.0, 1.0)) = 0.6
    }
    SubShader
    {
        Tags 
        { 
            "RenderType"="Opaque" 
            "RenderPipeline"="UniversalPipeline"
        }
        LOD 100

        Pass
        {
            Name "Outline"
            Tags { "LightMode"="Outline"}
            Cull Front
            ZWrite On

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl" 
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct appdata
            {
                float4 positionOS : POSITION;
                float4 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
            };

            struct v2f
            {
                float4 positionOS : SV_POSITION;
            };

            CBUFFER_START(UnityPerMaterial)
            float _OutlineThickness;
            float _OutlineZ;
            half4 _OutlineColor;
            CBUFFER_END

            v2f vert (appdata v)
            {
                v2f o = (v2f)0;
                VertexNormalInputs vertexNormalInput = GetVertexNormalInputs(v.normalOS.xyz, v.tangentOS);               
                float3 normalWS = vertexNormalInput.normalWS;
                float3 normalCS = TransformWorldToHClipDir(normalWS);          
                VertexPositionInputs positionInputs = GetVertexPositionInputs(v.positionOS.xyz);
                o.positionOS = positionInputs.positionCS + float4(normalCS.xy * 0.05 * _OutlineThickness, -_OutlineZ, 0);     
                o.positionOS.z = clamp(o.positionOS.z, 0, 1);
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                return _OutlineColor;
            }
            ENDHLSL
        }
    }
}
