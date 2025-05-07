Shader "Unlit/SimpleGradient"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _TopColor("Top Gradient Color: ", Color) = (1, 1, 1,1)
        _BottomColor("Bottom Gradient Color: ", Color) = (0, 0, 0, 0)
        _GradientOffset("Gradient Offset: ", Range(-1, 1)) = 0
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
            };

            struct v2f
            {
                float4 color : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            fixed4 _TopColor;
            fixed4 _BottomColor;
            float _GradientOffset;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                //apply offset to shift the blend.
                float t = saturate(v.uv.y + _GradientOffset);
                //interpolate the color between each vertex.
                o.color = lerp(_BottomColor,_TopColor, t);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                //color will be interpolated from the vertex colors.
                return i.color;
            }
            ENDCG
        }
    }
}
