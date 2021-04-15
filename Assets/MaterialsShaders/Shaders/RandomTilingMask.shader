Shader "Bronz/RandomTilingMask" {
	Properties{
		_Color("Color",COLOR) = (0.5,0.5,0.5,1.0)
		_Tex1("Texture", 2D) = "white" {}
		[Toggle] _UseRandMask("Use Random Mask", Int) = 0
	}
		SubShader{
			Tags { "RenderType" = "Opaque" }
			LOD 200

			CGPROGRAM
			#pragma surface surf Lambert

			sampler2D _Tex1;
			int _UseRandMask;
			fixed4 _Color;

			struct Input {
				half2 uv_Tex1;
			};

			// generic pseudo-random function
			half rand2(half2 coords) {
				return frac(sin(dot(coords, half2(12.9898,78.233))) * 43758.5453);
			}

			void surf(Input IN, inout SurfaceOutput o) {

				// declare the rotation matrixes in array
				const int2x2 rotMx[4] = {
					int2x2(1, 0, 0, 1),
					int2x2(0, 1, -1, 0),
					int2x2(-1, 0, 0, -1),
					int2x2(0, -1, 1, 0)
				};

				// calculate random values from 0 to 3 to select random rotation from the matrix array
				int x = (round(rand2(floor(IN.uv_Tex1)) * 3)) * _UseRandMask; // switch random mask on/off

				// rotate texture UVs based on random element of the rotation matrix array
				half2 uvTex1 = mul(IN.uv_Tex1 - 0.5, rotMx[x]);
				uvTex1.xy += 0.5;

				// use input texture with calculated UVs
				// and using texture derivates ddx and ddy to resolve mip mapping issues
				half4 tex1 = tex2D(_Tex1, uvTex1, ddx(IN.uv_Tex1.xy), ddy(IN.uv_Tex1.xy)) * _Color;

				// send texture data to shader output
				//fixed4 c = tex2D(_Tex1, IN.uv_Tex1) * _Color;
				o.Albedo = tex1.rgb;
				o.Alpha = tex1.a;
			}
			ENDCG
		}
			FallBack "Diffuse"
}
