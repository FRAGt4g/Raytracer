Shader "Unlit/Wallhack" {
    Properties {
        _Color ("Behind wall Color", Color) = (1, 1, 1, 1)
    }
    SubShader {
        Tags {
            "RenderType"="Transparent" // tag to inform the render pipeline of what type this is
            "Queue"="Transparent" // changes the render order
        }

        Cull Off
        Cull Off
        ZWrite Off
        ZTest Greater

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define TAU 6.283184

            #include "UnityCG.cginc"

            float4 _Color;

            struct MeshData {                   //Per-vertex mesh data
                float4 vertex : POSITION;       //vertex position
                float3 normal : NORMAL;         //Normals
                float2 uv : TEXCOORD0;          //uv coordinates
            };

            struct Interpolators {
                float4 vertex : SV_POSITION;    //Clip space position
                float2 uv : TEXCOORD0;      //Random stream
                float3 normal : TEXCOORD1;      //Random stream
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            Interpolators vert (MeshData v) {
                Interpolators o;
                
                o.vertex = UnityObjectToClipPos(v.vertex); //local to clip space
                o.uv = v.uv;
                o.normal = v.normal;

                return o;
            }

            float4 frag (Interpolators i) : SV_Target {
                return _Color; 
            }
            ENDCG
        }
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            float4 _Color;

            struct MeshData {                   //Per-vertex mesh data
                float4 vertex : POSITION;       //vertex position
                float3 normal : NORMAL;         //Normals
                float2 uv : TEXCOORD0;          //uv coordinates
            };

            struct Interpolators {
                float4 vertex : SV_POSITION;    //Clip space position
                float2 uv : TEXCOORD0;      //Random stream
                float3 normal : TEXCOORD1;      //Random stream
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            Interpolators vert (MeshData v) {
                Interpolators o;
                
                o.vertex = UnityObjectToClipPos(v.vertex); //local to clip space
                o.uv = v.uv;
                o.normal = v.normal;

                return o;
            }

            float4 frag (Interpolators i) : SV_Target {
                return float4(0, 0, 0, 1); 
            }
            ENDCG
        }
    }
}
