// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/empowered"
{
    Properties
    {
        _Color ("Base Color", Color) = (1, 1, 1, 1)
        _BlinkColor ("Blink Color", Color) = (1, 0, 0, 1)
        _BlinkIntensity ("Blink Intensity", Range(0, 1)) = 0
        _MainTex ("Base Texture", 2D) = "white" { }
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 color : COLOR;
            };

            struct v2f
            {
                float4 pos : POSITION;
                float4 color : COLOR;
            };

            uniform float _BlinkIntensity;
            uniform float4 _Color;
            uniform float4 _BlinkColor;
            uniform float4 _MainTex_ST;
            sampler2D _MainTex;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.color = v.color;
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                // Get the base color from texture or material color
                half4 baseColor = tex2D(_MainTex, i.pos.xy) * _Color;

                // Mix the base color and blink color based on _BlinkIntensity
                half4 blinkColor = lerp(baseColor, _BlinkColor, _BlinkIntensity);

                return blinkColor;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}

