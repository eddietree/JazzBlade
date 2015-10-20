Shader "JazzBlade/BladeTrail"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		//Tags { "RenderType"="Opaque" }
		Tags { "Queue" = "Transparent" }
		LOD 100
		
		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha // Alpha blending
			Cull Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
				float3 normal : TEXCOORD1;
				
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.normal = v.normal;
				return o;
			}

			float3 contrast( float3 rgb, float contrast) 
			{
				return rgb = ((rgb - 0.5f) * max(contrast, 0)) + 0.5f;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				float2 uv = i.uv;

				float perlin = tex2D(_MainTex, uv * float2(0.1f,1.0)).x;
				float3 perlinContrast = contrast( float3(perlin,perlin,perlin), 2.5f );
			
				fixed4 finalColor = fixed4(1.0,1.0,1.0,1.0);
				finalColor.xyz = perlinContrast;

				// alpha
				float fadeOutYThresX = 0.1f;
				float fadeOutYThresY = 0.1f;

				finalColor.w = perlinContrast.x;
				finalColor.w *= smoothstep( uv.x, 0.0f, fadeOutYThresX) * smoothstep( uv.x, 1.0f, 1.0f-fadeOutYThresX);
				finalColor.w *= smoothstep( uv.y, 0.0f, fadeOutYThresY) * smoothstep( uv.y, 1.0f, 1.0f-fadeOutYThresY);

				fixed4 col = finalColor;

				//col.xy = i.uv.xy;
				//col.z = 0.0f;

				return col;
			}
			ENDCG
		}
	}
}
