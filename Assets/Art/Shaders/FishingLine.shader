Shader "Custom/FishingLine"
{
    Properties
    {
        _Color ("Line Color", Color) = (1,1,1,1)
        _EdgeOpacity ("Edge Opacity", Range(0, 1)) = 1
        _CenterOpacity ("Center Opacity", Range(0, 1)) = 0.1
        _Width ("Edge Fade Width", Range(0, 1)) = 0.25
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType"="Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Lighting Off
        Cull Off

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
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            fixed4 _Color;
            float _EdgeOpacity;
            float _CenterOpacity;
            float _Width;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float v = i.uv.y; // Vertical axis across rope thickness
                float fade = saturate(abs(v - 0.5) / _Width); // 0 at center, 1 at edges
                float alpha = lerp(_CenterOpacity, _EdgeOpacity, fade);

                return fixed4(_Color.rgb, alpha * _Color.a);
            }
            ENDCG
        }
    }
}
