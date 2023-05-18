Shader "Hidden/BlurShader"
{
    Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
        _Dir("Direction", Vector) = (1.0, 0.0, 0.0, 0.0)
	}
	SubShader 
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 _MainTex_TexelSize;
			float2 _Dir;

			struct vertToFrag {
				float4 pos : POSITION;
				half2 uv : TEXCOORD0;
				float2 blurCoordinates[5] : TEXCOORD1;
			};
			
			vertToFrag vert(appdata_img v)
			{
				vertToFrag o;
				o.pos = UnityObjectToClipPos (v.vertex);
				o.uv = v.texcoord;

				o.blurCoordinates[0] = o.uv.xy;
				o.blurCoordinates[1] = o.uv.xy + _Dir * 1.407333;
				o.blurCoordinates[2] = o.uv.xy - _Dir * 1.407333;
				o.blurCoordinates[3] = o.uv.xy + _Dir * 3.294215;
				o.blurCoordinates[4] = o.uv.xy - _Dir * 3.294215;

				return o;
			}

			float4 frag(vertToFrag i) : COLOR
			{
				float4 sum = float4(0.0, 0.0, 0.0, 1.0);
				sum += tex2D(_MainTex, i.blurCoordinates[0]) * 0.204164;
				sum += tex2D(_MainTex, i.blurCoordinates[1]) * 0.304005;
				sum += tex2D(_MainTex, i.blurCoordinates[2]) * 0.304005;
				sum += tex2D(_MainTex, i.blurCoordinates[3]) * 0.093913;
				sum += tex2D(_MainTex, i.blurCoordinates[4]) * 0.093913;
				return sum; 
			}

			ENDCG
		} 
	}
}    