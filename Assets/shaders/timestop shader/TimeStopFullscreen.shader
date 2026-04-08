Shader "UI/TimeStopFullscreen"
{
    Properties
    {
        [PerRendererData] _MainTex ("Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        _EffectCenterUV ("Effect Center UV", Vector) = (0.5, 0.5, 0, 0)
        _WaveRadius ("Wave Radius", Float) = 0
        _WaveWidth ("Wave Width", Float) = 0.12
        _InvertStrength ("Invert Strength", Float) = 0
        _BWBlend ("Black White Blend", Float) = 0
        _Contrast ("Contrast", Float) = 1
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            Name "Default"
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            fixed4 _Color;

            float4 _EffectCenterUV;
            float _WaveRadius;
            float _WaveWidth;
            float _InvertStrength;
            float _BWBlend;
            float _Contrast;

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = v.texcoord;
                o.color = v.color * _Color;
                return o;
            }

            float3 ApplyContrast(float3 color, float contrast)
            {
                return ((color - 0.5) * contrast) + 0.5;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 tex = tex2D(_MainTex, i.texcoord) * i.color;
                float3 baseCol = tex.rgb;

                float2 center = _EffectCenterUV.xy;
                float dist = distance(i.texcoord, center);

                float halfWidth = _WaveWidth * 0.5;

                float ringMask = 1.0 - smoothstep(halfWidth, halfWidth + 0.01, abs(dist - _WaveRadius));
                float insideMask = step(dist, _WaveRadius);

                float luminance = dot(baseCol, float3(0.299, 0.587, 0.114));
                float3 bwCol = float3(luminance, luminance, luminance);

                float3 finalCol = lerp(baseCol, bwCol, insideMask * _BWBlend);

                float3 inverted = 1.0 - finalCol;
                finalCol = lerp(finalCol, inverted, ringMask * _InvertStrength);

                finalCol = ApplyContrast(finalCol, _Contrast);

                return fixed4(saturate(finalCol), tex.a);
            }
            ENDCG
        }
    }
}