Shader "Custom/GPULineShader"
{
    Properties
    {
        _LineWidth("Line Width", Float) = 1
        _Color("Color", Color) = (1,1,1,1)
    }
    
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma geometry geom
            #pragma fragment frag
            #pragma target 4.5
            
            #include "UnityCG.cginc"
            
            struct LineData
            {
                float3 start;
                float3 end;
            };
            
            StructuredBuffer<LineData> _Lines;
            float _LineWidth;
            fixed4 _Color;
            
            struct v2g
            {
                float3 pos : TEXCOORD0;
            };
            
            struct g2f
            {
                float4 pos : SV_POSITION;
                fixed4 color : COLOR;
            };
            
            v2g vert(uint vertex_id : SV_VertexID, uint instance_id : SV_InstanceID)
            {
                v2g o;
                LineData currentLine = _Lines[instance_id];
                o.pos = (vertex_id == 0) ? currentLine.start : currentLine.end;
                return o;
            }
            
            [maxvertexcount(4)]
            void geom(line v2g input[2], inout TriangleStream<g2f> triStream)
            {
                float3 start = input[0].pos;
                float3 end = input[1].pos;
                
                float3 dir = normalize(end - start);
                float3 right = normalize(cross(dir, UNITY_MATRIX_IT_MV[1].xyz));
                float3 up = normalize(cross(dir, right));
                
                float halfWidth = _LineWidth; // Масштабируем ширину
                
                g2f o;
                o.color = _Color;
                
                // Создаем четыре вершины для квада, представляющего линию
                o.pos = UnityWorldToClipPos(float4(start - right * halfWidth - up * halfWidth, 1.0));
                triStream.Append(o);
                
                o.pos = UnityWorldToClipPos(float4(start + right * halfWidth - up * halfWidth, 1.0));
                triStream.Append(o);
                
                o.pos = UnityWorldToClipPos(float4(end - right * halfWidth + up * halfWidth, 1.0));
                triStream.Append(o);
                
                o.pos = UnityWorldToClipPos(float4(end + right * halfWidth + up * halfWidth, 1.0));
                triStream.Append(o);
                
                triStream.RestartStrip();
            }
            
            fixed4 frag(g2f i) : SV_Target
            {
                return i.color;
            }
            ENDCG
        }
    }
}
