Shader "Unlit/Skybox Shader" {
    Properties {
        
        _ColorA ("Start Color", Color) = (1, 1, 1, 1)
        _ColorB ("End Color", Color) = (1, 1, 1, 1)

        _ColorStart ("Start Gradient", Range(0, 1)) = 0
        _ColorEnd ("End Gradient", Range(0, 1)) = 1

        _Scale ("UV Scale", Float) = 1
        _Offset ("Offset", Float) = 0
    }
    SubShader {
        Tags { "RenderType"="Opaque" }

        Cull Front

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"


            float4 _ColorA;
            float4 _ColorB;

            float _ColorStart;
            float _ColorEnd;

            float _Offset;
            float _Scale;

            struct MeshData {                   //Per-vertex mesh data
                float4 vertex : POSITION;       //vertex position
                float3 normal : NORMAL;         //Normals
                float2 uv : TEXCOORD0;          //uv coordinates
            };

            struct Interpolators {
                float4 vertex : SV_POSITION;    //Clip space position
                float3 normal : TEXCOORD0;      //Random stream
                float2 uv : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float InverseLerp(float a, float b, float current) {
                return (current - a) / (b - a);
            }

            Interpolators vert (MeshData v) {
                Interpolators o;
                o.vertex = UnityObjectToClipPos(v.vertex); //local to clip space
                o.normal = v.normal;
                o.uv = (v.uv + _Offset) * _Scale;

                return o;
            }

            float4 frag (Interpolators i) : SV_Target {
                return float4(i.normal, 1);
            }
            ENDCG
        }
    }
}
