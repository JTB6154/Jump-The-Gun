Shader "Universal Render Pipeline/2D/Custom/CrystalRefraction"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }

    SubShader
    {
        // SubShader Tags define when and under which conditions a SubShader block or
        // a pass is executed.
        Tags 
        { 
            "Queue" = "Transparent"
            "PreviewType" = "Plane"
            "RenderPipeline" = "UniversalRenderPipeline" 
        }

        Pass
        {
            ZWrite off
            Cull off
            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM
            // This line defines the name of the vertex shader.
            #pragma vertex vert
            // This line defines the name of the fragment shader.
            #pragma fragment frag

            // The Core.hlsl file contains definitions of frequently used HLSL
            // macros and functions, and also contains #include references to other
            // HLSL files (for example, Common.hlsl, SpaceTransforms.hlsl, etc.).
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl" 
            
            // The structure definition defines which variables it contains.
            // This example uses the Attributes structure as an input structure in
            // the vertex shader.
            struct Attributes
            {
                // The positionOS variable contains the vertex positions in object
                // space.
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                half4 color : COLOR;
            };

            struct Varyings
            {
                // The positions in this struct must have the SV_POSITION semantic.
                float4 positionCS : SV_POSITION;

                float2 uv : TEXCOORD0;
                
                // The position of the fragment on the screen.
                float2 screenuv : TEXCOORD1;
                half4 color : COLOR;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            half4 _MainTex_ST;


            uniform TEXTURE2D(_GlobalRefractionTex);
            uniform SAMPLER(sampler_GlobalRefractionTex);
            uniform half4 _GlobalRefractionTex_ST;

            Varyings vert (Attributes v)
            {
                Varyings o = (Varyings)0;

                // The TransformObjectToHClip function transforms vertex positions
                // from object space to homogenous space
                o.positionCS = TransformObjectToHClip(v.positionOS.xyz);

                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                // w = 1 for orthographic cameras
                o.screenuv = ((o.positionCS.xy / o.positionCS.w) + 1) * 0.5;
                o.color = v.color;

                return o;
            }

            half4 frag(Varyings i) : SV_Target
            {
                // sample the texture
                half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);

                // Reflection
                half4 refl = SAMPLE_TEXTURE2D(_GlobalRefractionTex, sampler_GlobalRefractionTex, i.screenuv);
                col.rgb = col.rgb * (1.0 - refl.a) + refl.rgb * refl.a;

                return col;
            }
            ENDHLSL
        }
    }
}
