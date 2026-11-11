Shader "Custom/WhiteOutline2D"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _OutlineColor ("Outline Color", Color) = (1,1,1,1)
        _OutlineSize ("Outline Thickness (Pixels)", Range(1, 10)) = 2
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
            "IgnoreProjector" = "True"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }

        Cull Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _MainTex_TexelSize;

            fixed4 _OutlineColor;
            float _OutlineSize;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 original = tex2D(_MainTex, i.uv) * i.color;

                float2 offset = _MainTex_TexelSize.xy * _OutlineSize;

                float outlineAlpha = 0.0;

                // Horizontal and vertical
                outlineAlpha = max(outlineAlpha,
                    tex2D(_MainTex, i.uv + float2(offset.x, 0)).a);

                outlineAlpha = max(outlineAlpha,
                    tex2D(_MainTex, i.uv - float2(offset.x, 0)).a);

                outlineAlpha = max(outlineAlpha,
                    tex2D(_MainTex, i.uv + float2(0, offset.y)).a);

                outlineAlpha = max(outlineAlpha,
                    tex2D(_MainTex, i.uv - float2(0, offset.y)).a);

                // Diagonal
                outlineAlpha = max(outlineAlpha,
                    tex2D(_MainTex, i.uv + float2(offset.x, offset.y)).a);

                outlineAlpha = max(outlineAlpha,
                    tex2D(_MainTex, i.uv + float2(-offset.x, offset.y)).a);

                outlineAlpha = max(outlineAlpha,
                    tex2D(_MainTex, i.uv + float2(offset.x, -offset.y)).a);

                outlineAlpha = max(outlineAlpha,
                    tex2D(_MainTex, i.uv - float2(offset.x, offset.y)).a);

                // Keep original sprite visible
                if (original.a > 0.01)
                    return original;

                // Draw white outline
                return fixed4(
                    _OutlineColor.rgb,
                    outlineAlpha * _OutlineColor.a * i.color.a
                );
            }
            ENDCG
        }
    }
}