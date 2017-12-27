// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

#warning Upgrade NOTE: unity_Scale shader variable was removed; replaced 'unity_Scale.w' with '1.0'

Shader "HM/Unlit_Alpha_CustomLight" {
	Properties {
		_Color ("CMain olor", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Ambient ("Ambient Color", Color) = (0.788, 0.819, 1, 1)
		_LightDiffuseColor ("Light Diffuse Color", Color) = (0.561, 0.341, 0.016, 1)
		_HighLightDir ("Hight Light Dir", Vector) = (1, 1, 1, 0)
		
		//_OutlineColor("Outline Color",Color) = (1,1,1,1)
		//_OutlineWidth("Outline Width",Range(0.001,0.1)) = 0.006
		
		//_RimColor ("Rim Color", Color) = (0.26, 0.19, 0.16, 0.0)
	        //_RimPower ("Rim Power", Range(0.5, 8.0)) = 3.0
	        //_LightDir ("World light Dir", Vector) = (-1, -1, 1, 0)

	     //_LineWidth("Burn Line Width", Range(0.0, 0.2)) = 0.1
	    // _BurnFirstColor("Burn First Color", Color) = (0.3, 0, 0, 1)
		//_BurnSecondColor("Burn Second Color", Color) = (1, 0, 0, 1)
		//_BurnAmount ("Burn Amount", Range(0.0, 1.0)) = 0.0		
		//_BurnMap("Burn Map", 2D) = "white"{}
	}
	
    Category
    {
	Cull Off
	Lighting Off

	SubShader {

		Pass {
			Cull Off
			LOD 200
			Tags { "RenderType"="Opaque" }

			CGPROGRAM

			#pragma vertex vert 
	        #pragma fragment frag
	        #include "UnityCG.cginc"
		
	        
	        uniform fixed4 _Color;       
			uniform fixed4 _Ambient;
			uniform fixed4 _HighLightDir;
			uniform fixed4 _LightDiffuseColor;

			//fixed _BurnAmount;			
			//sampler2D _BurnMap;
			//float4 _BurnMap_ST;
			//fixed _LineWidth;
			//fixed4 _BurnFirstColor;
			//fixed4 _BurnSecondColor;

		    //uniform fixed4 _RimColor;
			//uniform fixed _RimPower;
			//uniform fixed4 _Emissive;
			//uniform fixed4 _Diffuse;
		
	        struct v2f {
	        	float4 pos : SV_POSITION;
	        	fixed2 uv : TEXCOORD0;
	            fixed4 colour : TEXCOORD1;
				//float2 uvBurnMap : TEXCOORD2;
	        };
	        
	        uniform sampler2D _MainTex;
	        
	        v2f vert(appdata_base v) 
		{
			v2f o;
			o.pos = UnityObjectToClipPos(v.vertex);
			o.uv = MultiplyUV(UNITY_MATRIX_TEXTURE0, v.texcoord);
			//o.uvBurnMap = TRANSFORM_TEX(v.texcoord, _BurnMap);
										   
			o.colour = _Ambient*_Color+ _Color*saturate(dot(_HighLightDir, v.normal))*_LightDiffuseColor;//+_Emissive;
			o.colour.a = 1;
			return o;
		}
			
		fixed4 frag(v2f i) : Color {					
		
			fixed4 ret;
			ret = tex2D(_MainTex, i.uv)*i.colour;
			clip(ret.a - 0.5);

			//fixed3 burn = tex2D(_BurnMap, i.uvBurnMap).rgb;
			//clip(burn.r - _BurnAmount);

		    //fixed t = 1 - smoothstep(0.0, _LineWidth, burn.r - _BurnAmount);
			//fixed4 burnColor = lerp(_BurnFirstColor, _BurnSecondColor, t);
		    //burnColor = pow(burnColor, 5);
	    
			//return lerp(ret, burnColor, t * step(0.0001, _BurnAmount));;
			return ret;
			//+fixed4(i.diff, 1.0);
			
		}
			
	        ENDCG
		}

	}
    }
	FallBack "Diffuse"
}