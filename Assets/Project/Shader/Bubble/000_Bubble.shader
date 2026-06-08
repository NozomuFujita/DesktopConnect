Shader "Unlit/000_Bubble"
{
    Properties
    {
        
        [Header(Bubble Shape)]
        // 吹き出しのwidthとheight, エッジのradius
        _X ("X", Float) = 1
        _Y ("Y", Float) = 1
        _R ("r", Float) = 0.2

        [Header(Tail)]
        // 吹き出しの尾の方向, 長さ(の強さ), 範囲
        _TailDirection ("Direction", Vector) = (1, 0, 0, 0)
        _TailStrength ("Strength", Float) = 1
        _TailRange ("Range", Float) = 0.99

        [Header(Header)]
        _Color ("Color", Color) = (1, 1, 1, 1)
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
            Name "Bubble"
            Tags { "LightMode"="UniversalForward" }
            Blend SrcAlpha OneMinusSrcAlpha

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
            };

            CBUFFER_START(UnityPerMaterial)
            float _X;
            float _Y;
            float _R;
            vector _TailDirection;
            float _TailStrength;
            float _TailRange;
            half4 _Color;
            CBUFFER_END

            float4 RoundedRect(float4 vertex, float r)
            {
                float halfPI = 1.57;
                float lengthX = max(0, _X - _R);
                float lengthY = max(0, _Y - _R);
                float lengthArc = _R * halfPI;

                // 第一象限の長さ
                float totalLength = lengthX + lengthY + lengthArc;
                // 中心からの角度
                float angle = atan2(abs(vertex.y), abs(vertex.x));
                // 第一象限の長さから線形変換
                float targetLength = (angle / halfPI) * totalLength;

                // x,yの符号
                int signX = vertex.x >= 0 ? 1 : -1;
                int signY = vertex.y >= 0 ? 1 : -1;

                if(targetLength < lengthY)
                {
                    // 縦部分
                    vertex.x = signX * _X;
                    vertex.y = signY * targetLength;
                }
                else if(targetLength < lengthY + lengthArc)
                {
                    // 弧の部分
                    float currentArcLength = targetLength - lengthY;
                    float arcAngle = currentArcLength / _R;
                    vertex.x = signX * (lengthX + _R * cos(arcAngle));
                    vertex.y = signY * (lengthY + _R * sin(arcAngle));
                }
                else
                {
                    // 横部分
                    float currentLength = targetLength - lengthY - lengthArc;
                    vertex.x = signX * (lengthX - currentLength);
                    vertex.y = signY * _Y;
                }

                return vertex;
            }

            float4 TailB(float4 vertex, float r)
            {
                // 吹き出しのターゲット
                float2 dirTarget = float2(_TailDirection.x, _TailDirection.y);
                dirTarget = normalize(dirTarget);
                // 各頂点のベクトル((0, 0)原点)
                float2 dirVertex = float2(vertex.x, vertex.y);
                dirVertex = normalize(dirVertex);

                // 各頂点に応じて押し出し距離を変える
                float dotV = dot(dirTarget, dirVertex);
                if(dotV > _TailRange)
                {
                    float2 dirVertexReflect = -reflect(dirVertex, dirTarget);
                    float extrusion = (dotV - _TailRange) / (1.0 - _TailRange);
                    vertex.x += dirVertexReflect.x * extrusion * _TailStrength;
                    vertex.y += dirVertexReflect.y * extrusion * _TailStrength;;
                }

                return vertex;
            }

            

            v2f vert (appdata v)
            {
                v2f o = (v2f)0;

                // 中心(0,0)からの距離rを計算
                float r = length(v.vertex.xy);
                // 頂点が原点にある場合（ゼロ除算やatan2(0,0)を防止）
                if (r > 0.00001) 
                {
                    // 頂点はローカル座標で計算
                    v.vertex = RoundedRect(v.vertex, r);
                    v.vertex = TailB(v.vertex, r);
                }

                VertexPositionInputs vertexInput = GetVertexPositionInputs(v.vertex.xyz);
                o.vertex = vertexInput.positionCS;
                o.uv = v.uv;
                return o;

            }

            half4 frag (v2f i) : SV_Target
            {
                return _Color;
            }
            ENDHLSL
        }
    }
}
