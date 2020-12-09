Shader "Custom/HeightDependentTint" 
 {
   Properties 
   {
     _MainTex ("Base (RGB)", 2D) = "white" {}
	 _HeightLv0 ("Height Lv0", Float) = -2
     _HeightLv1 ("Height Lv1", Float) = -1
     _HeightLv2 ("Height lv2", Float) = 1
	 _Color0 ("Tint Color At 0", Color) = (1,1,1,1)
     _Color1 ("Tint Color At 1", Color) = (0,0,0,1)
     _Color2 ("Tint Color At 2", Color) = (1,1,1,1)
	 _Color3 ("Tint Color At 3", Color) = (1,1,1,1)

   }
  
   SubShader
   {
     Tags { "RenderType"="Opaque" }
  
     CGPROGRAM
     #pragma surface surf Lambert
  
     sampler2D _MainTex;
     fixed4 _Color1;
     fixed4 _Color2;
	 fixed4 _Color3;
	 fixed4 _Color0;
	 float _HeightLv0;
     float _HeightLv1;
     float _HeightLv2;
  
     struct Input
     {
       float2 uv_MainTex;
       float3 worldPos;
     };
  
     void surf (Input IN, inout SurfaceOutput o) 
     {
     
		if(IN.worldPos.y <= _HeightLv0){
		o.Albedo = _Color0;
		}

		if((IN.worldPos.y > _HeightLv0)&(IN.worldPos.y <=_HeightLv1)){
		o.Albedo = _Color1;
		}

		if((IN.worldPos.y > _HeightLv1)&(IN.worldPos.y <=_HeightLv2)){
		o.Albedo = _Color2;
		}
		
		if(IN.worldPos.y >= _HeightLv2){
		o.Albedo = _Color3;
		}
		o.Alpha=1;
	 }
     ENDCG
   } 
   Fallback "Diffuse"
 }