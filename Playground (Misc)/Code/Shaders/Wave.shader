Shader "Unlit/Wave" {
    Properties {
        _Amplitude ("Wave Amplitude", float) = 0.1
        _Period ("Wave Period", float) = 1

        _PeakColor ("Color of peaks of the waves", Color) = (1, 1, 1, 1)
        _ValleyColor ("Color of valleys of the waves", Color) = (0, 0, 0, 1)
    }
    SubShader {
        Tags { "RenderType"="Opaque" }

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define TAO 6.283184

            #include "UnityCG.cginc"


            float _Amplitude;
            float _Period;

            float4 _ValleyColor;
            float4 _PeakColor;

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

            float GetHieghtOnWave(float input) {
                return cos( (input - _Time.y * _Period) * TAO * 4) * _Amplitude;
            }

            float GetHeightOnRadialRipple(float2 coord) {
                float2 centeredCoords = coord * 2 - 1;
                float distance = length(centeredCoords);
                float height = GetHieghtOnWave(distance);
                return height;
            }


            Interpolators vert (MeshData v) {
                Interpolators o;
                
                float wave = GetHeightOnRadialRipple(v.uv);
                v.vertex.y = wave;
                o.vertex = UnityObjectToClipPos(v.vertex); //local to clip space
                
                o.normal = v.normal;
                o.uv = v.uv;
                return o;
            }


            float4 frag (Interpolators i) : SV_Target {
                return lerp(_ValleyColor, _PeakColor, (GetHeightOnRadialRipple(i.uv) / _Amplitude)); 
            }
            ENDCG
        }
    }
}
