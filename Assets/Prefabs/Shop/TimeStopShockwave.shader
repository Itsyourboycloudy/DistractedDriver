Shader "Custom/TimeStopShockwave"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (0.7, 0.9, 1, 0.15)
        _EdgeColor ("Edge Color", Color) = (1, 1, 1, 0.9)
        _FresnelPower ("Fresnel Power", Float) = 4
        _AlphaMultiplier ("Alpha Multiplier", Float) = 1
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Back

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            fixed4 _BaseColor;
            fixed4 _EdgeColor;
            float _FresnelPower;
            float _AlphaMultiplier;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 normalWS : TEXCOORD0;
                float3 viewDirWS : TEXCOORD1;
            };

            v2f vert(appdata v)
            {
                v2f o;
                float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.normalWS = UnityObjectToWorldNormal(v.normal);
                o.viewDirWS = normalize(_WorldSpaceCameraPos.xyz - worldPos.xyz);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float3 n = normalize(i.normalWS);
                float3 v = normalize(i.viewDirWS);

                float fresnel = pow(1.0 - saturate(dot(n, v)), _FresnelPower);

                fixed4 col = lerp(_BaseColor, _EdgeColor, fresnel);
                col.a *= _AlphaMultiplier;

                return col;
            }
            ENDCG
        }
    }
}