Shader "UI/TimeStopBubbleOverlay"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _Center ("Bubble Center", Vector) = (0.5, 0.5, 0, 0)
        _Radius ("Bubble Radius", Float) = 0
        _EdgeSoftness ("Edge Softness", Float) = 0.08

        _InnerTint ("Inner Tint", Color) = (0.45, 0.45, 0.45, 0.55)
        _OuterRingColor ("Outer Ring Color", Color) = (1, 1, 1, 0.9)

        _RingSize ("Ring Size", Float) = 0.04
        _RingStrength ("Ring Strength", Range(0,1)) = 1
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "IgnoreProjector"="True"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float2 texcoord : TEXCOORD0;
                float4 color    : COLOR;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                float2 uv       : TEXCOORD0;
                float4 color    : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float4 _Center;
            float _Radius;
            float _EdgeSoftness;

            float4 _InnerTint;
            float4 _OuterRingColor;

            float _RingSize;
            float _RingStrength;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.color = v.color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float2 center = _Center.xy;
                float dist = distance(uv, center);

                float bubbleMask = 1.0 - smoothstep(_Radius - _EdgeSoftness, _Radius + _EdgeSoftness, dist);

                float ringOuter = 1.0 - smoothstep(_Radius, _Radius + _EdgeSoftness, dist);
                float ringInner = 1.0 - smoothstep(_Radius - _RingSize, _Radius - _RingSize + _EdgeSoftness, dist);
                float ringMask = saturate(ringOuter - ringInner) * _RingStrength;

                float4 fillCol = _InnerTint;
                float4 ringCol = _OuterRingColor;

                float4 col = fillCol * bubbleMask;
                col = lerp(col, ringCol, ringMask);

                col.a = saturate(max(fillCol.a * bubbleMask, ringCol.a * ringMask));

                return col * i.color;
            }
            ENDHLSL
        }
    }
}