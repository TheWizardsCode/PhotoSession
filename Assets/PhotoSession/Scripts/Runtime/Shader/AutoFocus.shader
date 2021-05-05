// Simulate the focus rectangles of a camera
// Paints rectangles with borders at the position that is handed over in the _PositionX, _PositionY and Distance arrays
// The fill color depends on the distance
Shader "PhotoSession/AutoFocus"
{
    Properties
    {
        _RectangleSize("Rectangle Size", Float) = 0.008
        _RectangleBorder("Rectangle Border", Float) = 1

        _GridColor("Grid Color", Color) = (1,1,1,0)
        _HitColor("Hit Color", Color) = (0,1,0,0)
        _MaxHitDistance( "Max Hit Distance", Float) = 3

    }
    SubShader
    { 
         Tags{
             "Queue" = "Overlay"
             "IgnoreProjector" = "True"
             "RenderType" = "Transparent"
             "PreviewType" = "Plane"
         }
         ZWrite Off
         Lighting Off
         Cull Off
         Fog { Mode Off }
         Blend One Zero

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float2 uv : TEXCOORD0;
                float4 vertex : POSITION;

            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            float _RectangleSize;
            float _RectangleBorder;
            fixed4 _GridColor;
            fixed4 _HitColor;
            float _MaxHitDistance;

            uniform int _ArrayLength = 0;
            uniform float _PositionX[144];
            uniform float _PositionY[144];
            uniform float _Distance[144]; 

            v2f vert (appdata IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.uv = IN.uv;
                return OUT;
            }

            fixed4 rectangle(v2f IN, fixed4 col, float x, float y, float ratio, fixed4 outerColor, fixed4 innerColor)
            {
                // fwidth gets you exactly 1 pixel width
                // note float2, not just a float
                float2 border = fwidth(IN.uv);

                // actual rectangle
                float minXOuter = x - _RectangleSize / ratio;
                float maxXOuter = x + _RectangleSize / ratio;
                float minYOuter = y - _RectangleSize;
                float maxYOuter = y + _RectangleSize;

                float minXInner = x - (_RectangleSize - border.x * ratio * _RectangleBorder) / ratio;
                float maxXInner = x + (_RectangleSize - border.x * ratio * _RectangleBorder) / ratio;
                float minYInner = y - (_RectangleSize - border.y * _RectangleBorder);
                float maxYInner = y + (_RectangleSize - border.y * _RectangleBorder);

                float uvx = IN.uv.x;
                float uvy = IN.uv.y;

                // inner (excluding border)
                if (uvx >= minXInner && uvx <= maxXInner && uvy >= minYInner && uvy <= maxYInner)
                {
                    col = innerColor;
                }
                // outer (including border)
                else if (uvx >= minXOuter && uvx <= maxXOuter && uvy >= minYOuter && uvy <= maxYOuter)
                {
                    col = outerColor;
                }

                return col;
            }

            fixed4 frag (v2f IN) : SV_Target
            {
                // Apply aspect ratio correction
                float ratio = _ScreenParams.x / _ScreenParams.y;
                

                // use the color of the texture
                // fixed4 col = tex2D(_MainTex, i.uv);

                // use transparent color
                fixed4 col = fixed4(0, 0, 0, 0);

                // debug rectangle
                //col = rectangle( i, col, 0.54, 0.5, ratio, fixed4(1,0,0,1), fixed4(0,0,1,0.2));

                for (int j = 0; j < _ArrayLength; j++)
                {
                    float x = _PositionX[j] / _ScreenParams.x;
                    float y = _PositionY[j] / _ScreenParams.y;

                    // focus hit rectangle
                    if (_Distance[j] >= 0) 
                    {
                        // outer color
                        fixed4 outerColor = _HitColor;

                        // inner color depends on distance
                        fixed4 innerColor = _HitColor;

                        innerColor.a = 1 - clamp(_Distance[j], 0, _MaxHitDistance) / _MaxHitDistance;

                        // hit color's alpha may have been reduced, consider that for the maximum alpha value
                        innerColor.a *= _HitColor.a;
                        
                        col = rectangle(IN, col, x, y, ratio, outerColor, innerColor);
                    }
                    // default focus rectangle
                    else
                    {
                        // only a rectangle with a border with no inner color
                        fixed4 outerColor = _GridColor;
                        fixed4 innerColor = _GridColor;
                        innerColor.a = 0;

                        col = rectangle(IN, col, x, y, ratio, outerColor, innerColor);
                    }
                }

                return col;
            }
            ENDCG
        }
    }
}
