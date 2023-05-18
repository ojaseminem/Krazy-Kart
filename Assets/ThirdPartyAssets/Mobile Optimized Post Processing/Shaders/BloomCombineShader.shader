Shader "Hidden/BloomCombineShader" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
        _FilteredTex("Filtered scene", 2D) = "white" {}
	}
	SubShader 
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest

			#include "UnityCG.cginc"

			sampler2D _MainTex;
            sampler2D _FilteredTex;

			float4 frag(v2f_img i) : COLOR
			{
				float4 color = tex2D(_MainTex, i.uv);
				float4 filteredColor = tex2D(_FilteredTex, i.uv);

                return color + filteredColor; //additive blend normal scene image with blurred one
			}

			ENDCG
		} 
	}
}    