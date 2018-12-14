// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/OverlayMask"
{
    Properties {
        _MainTex ("Texture", 2D) = "" {}
        _Color ("Blend Color", Color) = (0.2, 0.3, 1 ,1)
    }
 
    SubShader {
 
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }

        Lighting Off
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite On
        Fog { Mode Off }
       
        Pass {  
            Offset -2,-2
            Stencil
            {
                Ref 1
                Comp always
                Pass replace
                WriteMask 1
            }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma fragmentoption ARB_precision_hint_fastest
 
            #include "UnityCG.cginc"
 
            struct appdata_t {
                float4 vertex : POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
            };
 
            struct v2f {
                float4 vertex : POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
            };
 
            sampler2D _MainTex;      
            half _MinVisDistance;
            half _MaxVisDistance;
            uniform float4 _MainTex_ST;
            uniform float4 _Color;
           
            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.color = v.color;
                o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
                 half3 viewDirW =WorldSpaceViewDir(v.vertex);
                 half viewDist = length(viewDirW);
                 half falloff = saturate((viewDist - _MaxVisDistance) / (_MaxVisDistance - _MinVisDistance));
                 o.color.a *= ( falloff);
                return o;
            }
 
            fixed4 frag (v2f i) : COLOR
            {
                // Get the raw texture value
                float4 texColor = tex2D(_MainTex, i.texcoord);
               
                fixed4 output = 0;               
                //output = texColor * i.color;
                return output;
            }
            ENDCG
        }
    }  
 
    Fallback off
}