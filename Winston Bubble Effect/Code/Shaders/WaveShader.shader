Shader "Unlit/WaveShader" {
    Properties {
        _Color ("Color", Color) = (1, 1, 1, 1)
        _UseRadialWave ("Should waves be radial", int) = 0
    }
    SubShader {
        Tags {
            "RenderType"="Transparent" // tag to inform the render pipeline of what type this is
            "Queue"="Transparent" // changes the render order
        }

        Cull Off
        Cull Off
        ZWrite Off
        Blend One One // additive

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define TAU 6.283184

            #include "UnityCG.cginc"

            float4 _Color;
            int _UseRadialWave;

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

            float GetWaveHieght( float value, float count = 5, float amp = 0.06, float speed = 0.2 ) {
                return cos((value - _Time.y * speed) * TAU * count) * amp;
            }
            
            float RadialDamp( float2 coord, float maxDistance = 1) {
                float2 centered = coord * 2 - 1;
                float distance = length(centered);
                return clamp(-1/maxDistance * distance + 1, 0, 1);
            }

            float GetWaveHieghtRadial( float2 coord, float count = 5, float amp = 0.06, float speed = 0.08, float maxDistance = 0.2 ) {
                float2 centered = coord * 2 - 1;
                float radialDistance = length(centered);
                float undampened = cos((radialDistance - _Time.y * speed) * TAU * count) * amp;
                return undampened * RadialDamp(coord);
            }


            Interpolators vert (MeshData v) {
                Interpolators o;
                
                if (_UseRadialWave == 0) v.vertex.y = GetWaveHieght(v.uv.x);
                else v.vertex.y = GetWaveHieghtRadial(v.uv);

                o.vertex = UnityObjectToClipPos(v.vertex); //local to clip space
                o.uv = v.uv;
                o.normal = v.normal;

                return o;
            }

            float4 frag (Interpolators i) : SV_Target {
                // return RadialDamp(i.uv);

                if (_UseRadialWave == 0) return GetWaveHieght(i.uv.x);
                else {
                    float height = GetWaveHieghtRadial(i.uv) / 0.5; 
                    return float4(height, height, height, 0);
                } 
            }
            ENDCG
        }
    }
}
