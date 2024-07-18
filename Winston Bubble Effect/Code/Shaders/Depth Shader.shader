Shader "Unlit/Depth Shader"
{
    SubShader {
        Tags { "RenderType"="Opaque" }
        Pass {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t {
                float4 vertex : POSITION;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float depth : TEXCOORD0;
            };

            v2f vert (appdata_t v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.depth = o.pos.z / o.pos.w;
                return o;
            }

            half4 frag(v2f i) : SV_Target {
                float depth = Linear01Depth(i.depth);
                return half4(depth, depth, depth, 1.0);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
