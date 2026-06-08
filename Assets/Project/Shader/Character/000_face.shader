Shader "Unlit/000_face"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _EyeMaskTex ("Mask Texture", 2D) = "white" {}
        _CenterPos ("Center Pos (Object)", float) = (0, 0, 0, 1)
        _FaceVector ("Face Vector (Object)", float) = (0, 0, 1, 1) //Camera Must not be Rotate ( => (-, 0, -))
        _LambertThreshold("Threshold", float) = 0.5
        [Header(Threshold)]
        [Space]
        _NearEyeHorizontalRangeMin ("Near Eye Horizontal Range (Min)", Range(0, 1)) = 0.3
        _NearEyeHorizontalRangeMax ("Near Eye Horizontal Range (Max)", Range(0, 1)) = 0.8
        _FarEyeHorizontalRangeMin ("Far Eye Horizontal Range (Min)", Range(0, 1)) = 0.7
        _FarEyeHorizontalRangeMax ("Far Eye Horizontal Range (Max)", Range(0, 1)) = 1
        _EyeVerticalRangeMin ("Eye Vertical Range (Min)", Range(0, 1)) = 0.7
        _EyeVerticalRangeMax ("Eye Vertical Range (Max)", Range(0, 1)) = 1

        [Header(Face Stencil)]
        [Space]
        _StencilRef("Stencil Ref", Int) = 1
        [Enum(UnityEngine.Rendering.CompareFunction)]_StencilComp("Stencil Comp", Int) = 0
        [Enum(UnityEngine.Rendering.StencilOp)]_StencilPassOp("Stencil Pass", Int) = 0
        [Enum(UnityEngine.Rendering.StencilOp)]_StencilFailOp("Stencil Fail", Int) = 0
        [Enum(UnityEngine.Rendering.StencilOp)]_StencilZFailOp("Stenci ZFail", Int) = 0
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
            Name "Face"
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
            TEXTURE2D(_EyeMaskTex);  
            SAMPLER(sampler_MainTex);
            SAMPLER(sampler_EyeMaskTex);

            CBUFFER_START(UnityPerMaterial)
            float4 _MainTex_ST;
            float4 _EyeMaskTex_ST;
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

        Pass
        {
            Name "Eye"
            Tags { "LightMode"="Eye" }
            Blend SrcAlpha OneMinusSrcAlpha
            ZTest Off

            Stencil
            {
                Ref [_StencilRef]
                Comp [_StencilComp]
                Pass [_StencilPassOp]
                Fail [_StencilFailOp]
                ZFail [_StencilZFailOp]
            }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl" 
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 positionWS : TEXCOORD1;
                float eyeMask : TEXCOORD2;
            };

            TEXTURE2D(_MainTex);  
            TEXTURE2D(_EyeMaskTex);  
            SAMPLER(sampler_MainTex);
            SAMPLER(sampler_EyeMaskTex);

            CBUFFER_START(UnityPerMaterial)
            float4 _MainTex_ST;
            float4 _EyeMaskTex_ST;
            float4 _CenterPos;
            float4 _FaceVector;
            float _NearEyeHorizontalRangeMin;
            float _NearEyeHorizontalRangeMax;
            float _FarEyeHorizontalRangeMin;
            float _FarEyeHorizontalRangeMax;
            float _EyeVerticalRangeMin;
            float _EyeVerticalRangeMax;
            CBUFFER_END

            float cross2d(float2 a, float2 b)
            {
                return (a.x * b.y - a.y * b.x);
            }

            float remap(float x, float a, float b, float c, float d)
            {
                return saturate(((b - a) / (d - c)) * (x - c));
            }

            float CalculateEyeMask(float vertex)
            {
                float3 cameraPosition = TransformWorldToObject(GetCameraPositionWS());
                float3 centerPosition = TransformWorldToObject(_CenterPos.xyz);
                float3 vectorFace2Camera = cameraPosition - centerPosition;

                //Normalize
                _FaceVector = normalize(_FaceVector);
                vectorFace2Camera = normalize(vectorFace2Camera);

                //Judgement - is Camera Right?, is Vertex Right? (Object Space)
                float isCameraRight = step(0.0, cross2d(_FaceVector.xz, vectorFace2Camera.xz));
                float isVertexRight = step(vertex, 0);

                //Calculate Cosine Value (Horizontal, Vertical)
                float horizontalAngleFactor = saturate(dot(normalize(_FaceVector.xz), normalize(vectorFace2Camera.xz)));
                float verticalAngleFactor = saturate(dot(normalize(_FaceVector.yz), normalize(vectorFace2Camera.yz)));

                //Caluculate Alpha Value (Horizontal)
                float thresholdHorizontalMin = (isCameraRight == isVertexRight) ? _NearEyeHorizontalRangeMin : _FarEyeHorizontalRangeMin;
                float thresholdHorizontalMax = (isCameraRight == isVertexRight) ? _NearEyeHorizontalRangeMax : _FarEyeHorizontalRangeMax;
                float eyeMask = remap(horizontalAngleFactor, 0.0, 1.0, thresholdHorizontalMin, thresholdHorizontalMax);

                //Multiplication Alpha Value (Vertical)
                eyeMask *= remap(verticalAngleFactor, 0.0, 1.0, _EyeVerticalRangeMin, _EyeVerticalRangeMax);

                return eyeMask;
            }

            v2f vert (appdata v)
            {
                v2f o = (v2f)0;
                VertexPositionInputs vertexInput = GetVertexPositionInputs(v.vertex.xyz);
                o.vertex = vertexInput.positionCS;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.eyeMask = CalculateEyeMask(v.vertex.x);
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                half4 mask = SAMPLE_TEXTURE2D(_EyeMaskTex, sampler_EyeMaskTex, i.uv);
                clip(mask.r - 0.1); //cliping by mask texture (Black is cliping area)
                col.a *= i.eyeMask; 
                return col;
            }
            ENDHLSL
        }
    }
}
