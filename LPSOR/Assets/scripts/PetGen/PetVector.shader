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


				float3 Hue(float H)
				{
					float R = abs(H * 6 - 3) - 1;
					float G = 2 - abs(H * 6 - 2);
					float B = 2 - abs(H * 6 - 4);
					return saturate(float3(R,G,B));
				}
				
				float4 HSVtoRGB(in float3 HSV)
				{
					return fixed4(((Hue(HSV.x) - 1) * HSV.y + 1) * HSV.z,1);
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
					// if operations might be expensive, if there's a better way to implement it, please do
	
					// exit if r=g=b
					if (xcolor.r == xcolor.g && xcolor.g == xcolor.b) return xcolor;
					// exits if multiplying r,g, or b does not equal to zero (since one of the two values HAVE to be equal to zero)
					if (xcolor.r*xcolor.g != 0 || xcolor.r*xcolor.b !=0  || xcolor.b*xcolor.g != 0 ) return xcolor;
					// exits if the shader isnt meant to be colored
					if ( _IsColorable == 0) return xcolor;

					int index;
					float base = 0.74f; // base color AKA saturation cant go any lower
					
					// sets index based on whether or not rgb values are equal to 0
					if (xcolor.r != 0) index = 0;
					else if (xcolor.g != 0) index = 1;
					else if (xcolor.b != 0) index = 2;

					float multiplier = xcolor[index]; // color multiplier for value

					// multiplies the saturation if its darker than base
					float satmultiplier = multiplier < base? 3.00f+(-3.00f*multiplier) : 1;

					float4 storedcolor = _ColorArray[index];
					float f = storedcolor[0];

					// converts the hsv values into rgb values
					float3 hsvvalue = float3(storedcolor.x,storedcolor.y*satmultiplier,storedcolor.z*multiplier);
					float4 color = HSVtoRGB(hsvvalue);
					
					// remnants from old codeee
					//int index = int(floor(xcolor.r*10));
					//float4 c = _ColorArray[index];
					
					color.a = 255;
					return color;
	
				}


			ENDCG
			}
		}
}
