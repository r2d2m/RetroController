// Upgrade NOTE: upgraded instancing buffer 'Props' to new syntax.

Shader "Custom/Water" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_BumpMap("Normal (RGB)", 2D) = "white" {}
		_Smoothness("Smoothness", 2D) = "black" {}
		_Glossiness ("Glossiness", 2D) = "white" {}
		_Occlusion ("Occlusion Map", 2D) = "white" {}
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_Intensity("Intensity", Range(0.0, 360.0)) = 20.0
		_CutoutThresh("Cutout Threshold", Range(0.0,1.0)) = 0.2
		_Transparency("Transparency", Range(0.0,0.5)) = 0.25
	}
	SubShader {
		Tags {"Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100

        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
		Cull off
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard alpha:fade

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _BumpMap;
		sampler2D _Smoothness;
		sampler2D _Occlusion;
		float _Intensity;

		struct Input {
			float2 uv_MainTex;
			float2 uv_BumpMap;
			float2 uv_Glossiness;
		};

		sampler2D _Glossiness;
		half _Metallic;
		fixed4 _Color;
		float _Transparency;
		float _CutoutThresh;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		
		void surf (Input IN, inout SurfaceOutputStandard o) {
			float sine = sin((IN.uv_MainTex.y + abs(_Time[1]))* _Intensity);
			IN.uv_MainTex.x += sine * 0.01;
			
			fixed4 oc = tex2D(_Occlusion, IN.uv_MainTex);
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb + (1 - oc.rgb);
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			fixed4 s = tex2D(_Smoothness, IN.uv_MainTex);
			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));
			o.Smoothness = tex2D(_Glossiness, IN.uv_Glossiness);
			o.Alpha = _Transparency;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
