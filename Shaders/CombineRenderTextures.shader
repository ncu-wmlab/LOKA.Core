Shader "Custom/CombineRenderTextures"
{
    Properties
    {
        _LeftTex ("Left Eye Texture", 2D) = "white" {}
        _RightTex ("Right Eye Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

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

            sampler2D _LeftTex;
            sampler2D _RightTex;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;
                fixed4 col;

                // Combine the two textures
                if (uv.x < 0.5)
                {
                    col = tex2D(_LeftTex, uv * 2);
                }
                else
                {
                    col = tex2D(_RightTex, float2((uv.x - 0.5) * 2, uv.y));
                }
                return col;
            }
            ENDCG
        }
    }
}
