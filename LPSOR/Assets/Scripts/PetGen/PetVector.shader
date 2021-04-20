Shader "Unlit/PetVector"
{
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		
		_ColorIndex("Color Index",Float) = 3 // 0,1,2 => Coat,Patch,Eyecolor
		// if bigger than 2 then its non applicable and exits
		
		_Saturation("Saturation Multiplier",Float) = 2 // saturation is multiplied
	
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
				
				float _ColorIndex;
				float _Saturation;
			
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

					OUT.color = color *_RendererColor;

					#ifdef PIXELSNAP_ON
					OUT.vertex = UnityPixelSnap(OUT.vertex);
					#endif

					return OUT;
				}

				int GetMin(fixed4 color)
				{
					// compare r, g and b values to see which is the highest one
					int min1 = color.r < color.g? 0 : 1;
					int min2 = color[min1] < color.b ? min1 : 2;

					// returns the index of the color
					return min2;		
				}

				int GetMax(fixed4 color)
				{
					// compare r, g and b values to see which is the biggest value
					int max1 = color.r > color.g? 0 : 1;
					int max2 = color[max1] > color.b ? max1 : 2;

					// returns index of the color
					return max2;
				}
				 
				fixed4 SwapFragment(v2f IN) : SV_Target // Swaps the colors when frag is called.
				{
					half4 xcolor = SampleSpriteTexture(IN.texcoord) * IN.color;
					// if operations might be expensive, if there's a better way to implement it, please do
	
					// exit if r,g and b are not equal
					if (xcolor.r != xcolor.g && xcolor.g != xcolor.b) return xcolor;
					// exit if white or black
					if(xcolor.r == 0 || xcolor.r == 1) return xcolor;
					// exits if colorIndex isn't valid
					if ( _ColorIndex > 2) return xcolor;
					xcolor += 0.25f; // adds 0.25 to the current color, making the 0.75 pure white

					// multiply stored color
					half4 storedColor = _Color;
					half4 color = xcolor*storedColor;
					color.a = 255;

					// increase saturation
					int minIndex = GetMin(color);
					int maxIndex = GetMax(color);
					float satMultiplier = (1-xcolor.r)*_Saturation;

					/* Not getting into the math, but it basically calculates how you modify the color if you want to
						increase the saturation. It finds the delta, adds the multiplier, then sets a new minColor based on delta
						since maxColor doesn't change (maxColor = value)
					*/
					float delta2 = color[maxIndex] + color[minIndex]*(satMultiplier-1);
					float minColor2 = color[maxIndex] - delta2;
					
					// anyway since i cant write to a float4 i have to use if else statements
					// im so sorry
					if(minIndex==0)
						color.r = minColor2;
					else if(minIndex==1)
						color.g = minColor2;
					else
						color.b = minColor2;
					
					return color;
	
				}
			ENDCG
			}
		}
}
