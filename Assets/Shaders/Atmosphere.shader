Shader "Hidden/Atmosphere"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "/Includes/Math.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 viewVector : TEXCOORD1;
            };

            v2f vert(appdata v) 
            {
                v2f output;
                output.pos = UnityObjectToClipPos(v.vertex);
                output.uv = v.uv;
                float3 view = mul(unity_CameraInvProjection, float4(v.uv.xy * 2 - 1, 0, -1));
                output.viewVector = mul(unity_CameraToWorld, float4(view, 0));
                return output;
            }

            sampler2D _MainTex;
            sampler2D _CameraDepthTexture;

            float4 coefficientsForScatter;
            float density;
            int inScatterPointCount;
            int opticalDepthPointCount;

            float3 sunDir;
            float planetRadius;
            float3 centre;
            float atmosphereRadius;

            float DensityAtGivenPoint(float3 samplePoint) 
            {
                float heightFromSurface = length(samplePoint - centre) - planetRadius;
                float normalizedHeight = heightFromSurface / (atmosphereRadius - planetRadius);
                return (1 - normalizedHeight) * exp(density * -normalizedHeight);
            }

            float OpticalDepth(float3 rayOrigin, float3 rayDir, float rayLength) 
            {
                float3 samplePoint = rayOrigin;
                float stepLength = rayLength / (opticalDepthPointCount - 1);
                float depth = 0;

                for (int i = 0; i < opticalDepthPointCount; i++)
                {
                    depth += DensityAtGivenPoint(samplePoint) * stepLength;
                    samplePoint += rayDir * stepLength;
                }

                return depth;
            }

            float3 CustomLight(float3 rayOrigin, float3 rayDir, float rayLength, float3 originalCol) 
            {
                float3 inScatteredLight = 0;
                float viewRayOpticalDepth = 0;
                float stepLength = rayLength / (inScatterPointCount - 1);
                float3 inScatterPoint = rayOrigin;

                for (int i = 0; i < inScatterPointCount; i++)
                {
                    viewRayOpticalDepth = OpticalDepth(inScatterPoint, -rayDir, i * stepLength);
                    float sunRayLength = raySphere(centre, atmosphereRadius, inScatterPoint, sunDir).y;
                    float sunRayOpticalDepth = OpticalDepth(inScatterPoint, sunDir, sunRayLength);
                    float3 transmittance = exp(coefficientsForScatter * -(viewRayOpticalDepth + sunRayOpticalDepth));

                    inScatteredLight += DensityAtGivenPoint(inScatterPoint) * transmittance * coefficientsForScatter * stepLength;
                    inScatterPoint += rayDir * stepLength;
                }
                return originalCol * exp(-viewRayOpticalDepth) + inScatteredLight;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float4 originalCol = tex2D(_MainTex, i.uv);
                float sceneDepthNonLinear = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv);
                float sceneDepth = LinearEyeDepth(sceneDepthNonLinear) * length(i.viewVector);
                float3 rayOrigin = _WorldSpaceCameraPos;
                float3 rayDir = normalize(i.viewVector);
                float2 hitInfo = raySphere(centre, atmosphereRadius, rayOrigin, rayDir);
                float distToAtmosphere = hitInfo.x;
                float distThroughAtmosphere = min(hitInfo.y, sceneDepth - distToAtmosphere);

                if (distThroughAtmosphere > 0)
                {
                    float3 atmosphericPoint = rayOrigin + rayDir * (distToAtmosphere + 0.0001);
                    float3 calculatedLight = CustomLight(atmosphericPoint, rayDir, distThroughAtmosphere - 0.0002, originalCol);
                    return float4(calculatedLight, 0);
                }

                return originalCol;
            }
            ENDCG
        }
    }
}
