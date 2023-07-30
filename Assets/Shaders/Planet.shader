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
        #pragma surface surf Standard vertex:vert
        #pragma target 4.0

        #include "/Includes/Math.cginc"

        float2 _ElevationMinMax;
        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
            float3 localPos; // Vertex position in object space
            float3 normal : NORMAL;
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

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            // Sample the texture using the UV coordinates from the main texture
            fixed4 texColor = tex2D(_MainTex, IN.uv_MainTex);

            float yes = invLerp(_ElevationMinMax.x, 0, IN.uv_MainTex.y);

            float no = invLerp(0, _ElevationMinMax.y, IN.uv_MainTex.y);

            float yes2 = lerp(0, 0.5, yes);

            float no2 = lerp(0.5, 1, no);

            float yes3 = floor(yes);

            float slightlyAgreed = yes3 * no2;

            float maybe = 1 - yes3;

            float idk = yes2 * maybe;

            float agreed = idk + slightlyAgreed;

            float2 myVector = float2(agreed, IN.uv_MainTex.x);

            // Manually calculate the UV coordinates for sampling the gradient texture
            // The gradient texture is sampled horizontally, so the U coordinate is 't', and the V coordinate is 0.5 (or any fixed value in [0, 1])
            fixed2 gradientUV = myVector;

            // Sample the color from the gradient texture using the calculated UV coordinates
            fixed3 gradientColor = tex2D(_MainTex, gradientUV).rgb;

            // Apply the color from the gradient texture
            o.Albedo = gradientColor;
            o.Alpha = texColor.a;

            // Zero for now as it doesn't look good
            o.Smoothness = maybe * 0;
        }

        ENDCG
    }
        FallBack "Diffuse"
}
