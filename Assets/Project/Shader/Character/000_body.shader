Shader "Unlit/000_body"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _LambertThreshold("Threshold", float) = 0.5
    }
    SubShader
    {
        Tags{   
                "RenderType"="Opaque" 
                "RenderPipeline"="UniversalPipeline"
            }
        LOD 100
        Cull off

        Pass
        {
            Name "Body"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl" 
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
                float4 vertex : SV_POSITION;
            };

            TEXTURE2D(_MainTex);  
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
            float4 _MainTex_ST;
            float _LambertThreshold;
            CBUFFER_END

            v2f vert (appdata v)
            {
                v2f o = (v2f)0;
                VertexPositionInputs vertexInput = GetVertexPositionInputs(v.vertex.xyz);
                o.vertex = vertexInput.positionCS;
                o.normal = normalize(TransformObjectToWorldNormal(v.normal));
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            half4 toon(float4 vertex, float3 normal, half4 col)
            {
                Light mainLight;
                mainLight = GetMainLight();
                half dotV = saturate(dot(mainLight.direction.xyz, normal) * 0.5f + 0.5f);
                half ramp = step(dotV, _LambertThreshold);
                col.rgb = lerp(col.rgb, col.rgb * mainLight.color.rgb, ramp);
                return col;
            }

            half4 frag (v2f i) : SV_Target
            {
                half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                col = toon(i.vertex, i.normal, col);
                return col;
            }
            ENDHLSL
        }
    }
}
