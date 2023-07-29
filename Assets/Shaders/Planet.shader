Shader "Custom/Planet"
{
    Properties
    {
        _MainTex("Main Texture", 2D) = "white" {}
    }

        SubShader
    {
        Tags { "RenderType" = "Opaque" }

        CGPROGRAM
        #pragma surface surf Lambert vertex:vert

        #include "/Includes/Math.cginc"

        float2 _ElevationMinMax;
        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
            float3 localPos; // Vertex position in object space
        };

        void vert(inout appdata_full v, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input, o);
            // Calculate the distance from the object's center using the vertex's local position
            float distance = length(v.vertex);

            // Map the distance from _ElevationMinMax.x to _ElevationMinMax.y to a range between 0 and 1
            float t = saturate((distance - _ElevationMinMax.x) / (_ElevationMinMax.y - _ElevationMinMax.x));

            // Interpolate the position based on the distance
            o.localPos = v.vertex;
            o.localPos.xyz *= t;
        }

        void surf(Input IN, inout SurfaceOutput o)
        {
            // Sample the texture using the UV coordinates from the main texture
            fixed4 texColor = tex2D(_MainTex, IN.uv_MainTex);

            // Calculate the distance from the object's center using the interpolated vertex position
            float distance = length(IN.localPos);

            float yes = invLerp(_ElevationMinMax.x, _ElevationMinMax.y, distance);

            // Manually calculate the UV coordinates for sampling the gradient texture
            // The gradient texture is sampled horizontally, so the U coordinate is 't', and the V coordinate is 0.5 (or any fixed value in [0, 1])
            fixed2 gradientUV = fixed2(yes, 0.5);

            // Sample the color from the gradient texture using the calculated UV coordinates
            fixed3 gradientColor = tex2D(_MainTex, gradientUV).rgb;

            // Apply the color from the gradient texture
            o.Albedo = gradientColor;
            o.Alpha = texColor.a;
        }

        ENDCG
    }
        FallBack "Diffuse"
}
