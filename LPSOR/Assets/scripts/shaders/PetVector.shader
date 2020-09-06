Shader "Unlit/PetVector"
{
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		
		_IsColorable("IsColorable",Float) = 1 // 1 is true, 0 is false

		_Color("Tint", Color) = (1,1,1,1)

		[MaterialToggle] PixelSnap("Pixel snap", Float) = 0
		[HideInInspector] _RendererColor("RendererColor", Color) = (1,1,1,1)
		[HideInInspector] _Flip("Flip", Vector) = (1,1,1,1)
		[PerRendererData] _AlphaTex("External Alpha", 2D) = "white" {}
		[PerRendererData] _EnableExternalAlpha("Enable External Alpha", Float) = 0
	}

		SubShader
		{
			Tags
			{
				"Queue" = "Transparent"
				"IgnoreProjector" = "True"
				"RenderType" = "Transparent"
				"PreviewType" = "Plane"
				"CanUseSpriteAtlas" = "True"
			}

			Cull Off
			Lighting Off
			ZWrite Off
			Blend One OneMinusSrcAlpha

			Pass
			{
			CGPROGRAM


				#pragma vertex VectorVert
				#pragma fragment SwapFragment
				
				#pragma target 2.0
				#pragma multi_compile_instancing
				#pragma multi_compile _ PIXELSNAP_ON
				#pragma multi_compile _ ETC1_EXTERNAL_ALPHA
				#include "UnitySprites.cginc"
				
				
				float _IsColorable;
				float4 _ColorArray[3];

				//sampler2D _MainTex;


				fixed4 hsv_to_rgb(float3 HSV) // god save us
				{
						fixed4 RGB = HSV.z;
				
						fixed var_h = HSV.x * 6;
						fixed var_i = floor(var_h);   // Or ... var_i = floor( var_h )
						fixed var_1 = HSV.z * (1.0 - HSV.y);
						fixed var_2 = HSV.z * (1.0 - HSV.y * (var_h-var_i));
						fixed var_3 = HSV.z * (1.0 - HSV.y * (1-(var_h-var_i)));
						if      (var_i == 0) { RGB = (HSV.z, var_3, var_1,1); }
						else if (var_i == 1) { RGB = (var_2, HSV.z, var_1,1); }
						else if (var_i == 2) { RGB = (var_1, HSV.z, var_3,1); }
						else if (var_i == 3) { RGB = (var_1, var_2, HSV.z,1); }
						else if (var_i == 4) { RGB = (var_3, var_1, HSV.z,1); }
						else                 { RGB = (HSV.z, var_1, var_2,1); }
				
					return (RGB);
				}

				v2f VectorVert(appdata_t IN) // The original VectorVertex portion of the shader. 
				{
					
					v2f OUT;

					UNITY_SETUP_INSTANCE_ID(IN);
					UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

					OUT.vertex = UnityFlipSprite(IN.vertex, _Flip);
					OUT.vertex = UnityObjectToClipPos(OUT.vertex);
					OUT.texcoord = IN.texcoord;

					#ifdef UNITY_COLORSPACE_GAMMA
					fixed4 color = IN.color;
					#else
					fixed4 color = fixed4(GammaToLinearSpace(IN.color.rgb), IN.color.a);
					#endif

					OUT.color = color * _Color*_RendererColor;

					#ifdef PIXELSNAP_ON
					OUT.vertex = UnityPixelSnap(OUT.vertex);
					#endif

					return OUT;
				}
				 
				fixed4 SwapFragment(v2f IN) : SV_Target // Swaps the colors when frag is called.
				{
					fixed4 xcolor = SampleSpriteTexture(IN.texcoord) * IN.color;
					fixed4 mcolor = _RendererColor-xcolor;
					float4 c = (1,1,1,1);

					
					if (mcolor.r*mcolor.g == 0 || mcolor.r*mcolor.b ==0  || mcolor.b*mcolor.g == 0 ){
						return c;
					}
					if (mcolor.r*mcolor.g*mcolor.b == 0 || _IsColorable == 0){
						// if operations might be expensive on the gpu, if there's a better way to implement it, don't hesitate to change
						// exits the operation if r isnt equal to g, or if the shader isnt meant to be colored
						return xcolor;
					}
					int index;
					float multiplier;
					if(mcolor.g==mcolor.b)
					{
						index = 0;
						multiplier = xcolor.r;
					}
					if(mcolor.r==mcolor.b)
					{
						index = 1;
						multiplier = xcolor.g;
					} 
					if(mcolor.r==mcolor.g)
					{
						index = 2;
						multiplier = xcolor.b;
					} 
					float3 storedcolor = _ColorArray[index];
					float3 hsvvalue = (storedcolor.r,storedcolor.g,storedcolor.b*multiplier);
					fixed4 color = hsv_to_rgb(hsvvalue);
					
						//int index = int(floor(xcolor.r*10));
					
						//float4 c = _ColorArray[index];
					

					color.a = 255;
					return color;
					//return xcolor;*/
	
				}


			ENDCG
			}
		}
}
