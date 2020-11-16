// Ref: https://www.vertexfragment.com/ramblings/unity-postprocessing-sobel-outline/
// The shader uses the Sobel operator/filter, which is a classic edge detection convolution
// filter used in for decades in computer graphics.
Shader "Hidden/Custom/ContourEdgeOutline"
{
    HLSLINCLUDE

        #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"

        TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
        TEXTURE2D_SAMPLER2D(_CameraDepthTexture, sampler_CameraDepthTexture);

        float _OutlineThickness;
        float _OutlineMultiplier;
        float _OutlineBias;

        float4 _OutlineColor;

        float Persp2OrthoDepth(float rawDepth)
        {
            float persp = LinearEyeDepth(rawDepth);
            float ortho = (_ProjectionParams.z - _ProjectionParams.y) * (1 - rawDepth) + _ProjectionParams.y;
            return lerp(persp, ortho, unity_OrthoParams.w);
        }

        float Sobel(float sc, float st, float sb, float sl, float sr)
        {
            float sobel = abs(st - sc) + abs(sb - sc) + abs(sl - sc) + abs(sr - sc);
            return pow(sobel * _OutlineMultiplier, _OutlineBias);
        }

        float Sobel3(float3 sc, float3 st, float3 sb, float3 sl, float3 sr)
        {
            float3 sobelVec = abs(st - sc) + abs(sb - sc) + abs(sl - sc) + abs(sr - sc);
            float sobel = saturate(sobelVec.x + sobelVec.y + sobelVec.z);
            return pow(sobel * _OutlineMultiplier, _OutlineBias);
        }

        float SobelSampleColor(Texture2D t, SamplerState s, float2 uv, float3 offset)
        {
            float3 pixelCenter = t.Sample(s, uv).rgb;
            float3 pixelLeft = t.Sample(s, uv - offset.xz).rgb;
            float3 pixelRight = t.Sample(s, uv + offset.xz).rgb;
            float3 pixelTop = t.Sample(s, uv + offset.zy).rgb;
            float3 pixelBottom = t.Sample(s, uv - offset.zy).rgb;

            return Sobel3(pixelCenter, pixelTop, pixelBottom, pixelLeft, pixelRight);
        }

        float SobelSampleDepth(Texture2D t, SamplerState s, float2 uv, float3 offset)
        {
            float pixelCenter = Persp2OrthoDepth(t.Sample(s, uv).r);
            float pixelLeft = Persp2OrthoDepth(t.Sample(s, uv - offset.xz).r);
            float pixelRight = Persp2OrthoDepth(t.Sample(s, uv + offset.xz).r);
            float pixelTop = Persp2OrthoDepth(t.Sample(s, uv + offset.zy).r);
            float pixelBottom = Persp2OrthoDepth(t.Sample(s, uv - offset.zy).r);

            return Sobel(pixelCenter, pixelTop, pixelBottom, pixelLeft, pixelRight);
        }

        float4 Frag(VaryingsDefault i) : SV_Target
        {
            // Sample the scene and main texture
            float3 offset = float3((1.0 / _ScreenParams.x), (1.0 / _ScreenParams.y), 0.0) * _OutlineThickness;
            float3 sceneColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord).rgb;
            float sobelColor = SobelSampleColor(_MainTex, sampler_MainTex, i.texcoord.xy, offset);
            //float sobelDepth = SobelSampleDepth(_CameraDepthTexture, sampler_CameraDepthTexture, i.texcoord.xy, offset);

            float sobelOutline = saturate(sobelColor);

            // Modulate the outline color based on its transparency
            float3 outlineColor = lerp(sceneColor, _OutlineColor.rgb, _OutlineColor.a);

            // Calculate the final scene color
            float3 color = lerp(sceneColor, outlineColor, sobelOutline);
            return float4(color, 1.0);
        }

    ENDHLSL

    SubShader
    {
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            HLSLPROGRAM

                #pragma vertex VertDefault
                #pragma fragment Frag

            ENDHLSL
        }
    }
}