Shader "Custom/ChessboardShader"
{
    Properties
    {
        _CellSize("Cell Size", Float) = 1.5
        _BoardSize("Board Size", Int) = 15
        _Color1("Color 1", Color) = (1, 1, 1, 1)
        _Color2("Color 2", Color) = (0, 0, 0, 1)
        _SideColor("Side Color", Color) = (0.5, 0.5, 0.5, 1) // Цвет боковых сторон
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
            };

            float _CellSize;
            int _BoardSize;
            fixed4 _Color1;
            fixed4 _Color2;
            fixed4 _SideColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv * _BoardSize;
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Проверяем, что это верхняя грань (нормаль смотрит вверх)
                float isTopFace = step(0.9, dot(i.worldNormal, float3(0, 1, 0)));
                
                if (isTopFace > 0.5)
                {
                    // Рассчитываем координаты клетки только для верхней грани
                    float2 cellCoord = floor(i.uv);
                    
                    // Проверяем границы доски
                    if (cellCoord.x >= 0 && cellCoord.x < _BoardSize && 
                        cellCoord.y >= 0 && cellCoord.y < _BoardSize)
                    {
                        // Шахматный порядок
                        bool isColor1 = (cellCoord.x + cellCoord.y) % 2 == 0;
                        return isColor1 ? _Color1 : _Color2;
                    }
                }
                
                // Для всех остальных граней - возвращаем цвет боковых сторон
                return _SideColor;
            }
            ENDCG
        }
    }
}
