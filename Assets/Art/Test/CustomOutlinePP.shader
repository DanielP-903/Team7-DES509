Shader "Hidden/Shader/CustomOutlinePP"
{
    HLSLINCLUDE

    #pragma target 4.5
    #pragma only_renderers d3d11 playstation xboxone vulkan metal switch

    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/PostProcessing/Shaders/FXAA.hlsl"
    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/PostProcessing/Shaders/RTUpscale.hlsl"

    struct Attributes
    {
        uint vertexID : SV_VertexID;
        UNITY_VERTEX_INPUT_INSTANCE_ID
    };

    struct Varyings
    {
        float4 positionCS : SV_POSITION;
        float2 texcoord   : TEXCOORD0;
        UNITY_VERTEX_OUTPUT_STEREO
    };

    Varyings Vert(Attributes input)
    {
        Varyings output;
        UNITY_SETUP_INSTANCE_ID(input);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
        output.positionCS = GetFullScreenTriangleVertexPosition(input.vertexID);
        output.texcoord = GetFullScreenTriangleTexCoord(input.vertexID);
        return output;
    }

    // List of properties to control your post process effect

    float _Intensity;
    //#define _Distance _Params.w
    TEXTURE2D(_DepthTexture);
    SamplerState sampler_DepthTexture;
    TEXTURE2D_X(_InputTexture);

    TEXTURE2D(_MainTex);
    SamplerState sampler_MainTex;
    TEXTURE2D(_CameraGBufferTexture2);
    SamplerState sampler_CameraGBuggerTexture2;

    // Set paramateres
    float _Thickness;
    float _Edge;
    float _Transition;
    float4 _Colour;
    float4 _Params;

    /*float sobelSampleDepth(Texture2D t, SamplerState s, float2 uv, float3 offset)
    {
        float Center = LinearEyeDepth(t.Sample(s, uv).r);
        float left = LinearEyeDepth(t.Sample(s, uv - offset.xz).r);
        float right = LinearEyeDepth(t.Sample(s, uv + offset.xz).r);
        float up = LinearEyeDepth(t.Sample(s, uv + offset.xy).r);
        float down = LinearEyeDepth(t.Sample(s, uv - offset.xy).r);

        return SobelDepth(lCenter, left, right, up, down);
    }*/

    float4 CustomPostProcess(Varyings input) : SV_Target
    {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

        uint2 positionSS = input.texcoord * _ScreenSize.xy;
        float3 outColor = LOAD_TEXTURE2D_X(_InputTexture, positionSS).xyz;

        //Test Sobel effect

        //float3 offset = float3((1.0 / _ScreenParams.x), (1.0 / _ScreenParams.y), 0.0) * _Thickness;
        //float SobelDepth = sobelSampleDepth(_DepthTexture, sampler_DepthTexture, input.texcoord.xy, offset)
       

        //return float4(SobelDepth, SobelDepth, SobelDepth, 1.0);

        float2 offset = _Thickness / _ScreenParams;
       
        /*float left = LinearEyeDepth(SAMPLE_TEXTURE2D(_DepthTexture, _DepthTexture, input.texcoord + float2(-offset.x, 0)).x);
        float right = LinearEyeDepth( SAMPLE_TEXTURE2D(_DepthTexture, _DepthTexture, input.texcoord + float2(offset.x, 0)).x);
        float up = LinearEyeDepth( SAMPLE_TEXTURE2D(_DepthTexture, _DepthTexture, input.texcoord + float2(0,offset.y, 0)).x);
        float down = LinearEyeDepth( SAMPLE_TEXTURE2D(_DepthTexture, _DepthTexture, input.texcoord + float2(0,-offset.y, 0)).x);*/

        float left = LinearEyeDepth(_DepthTexture.Sample(sampler_DepthTexture, input.texcoord.xy).r);


        float delta = sqrt( pow(right - left, 2) + pow(up - down, 2));
        float t = smoothstep(_StartPoint, _StartPoint + _Transition, delta);

        float MainTex = SAMPLE_TEXTURE2D(_InputTexture, Sampler_InputTexture, input.texcoord);
        float Colour = lerp(MainTex, _Colour, _Colour.a);

        float4 output = lerp(MainTex, Colour, t);

        return output;


        // Set depth mask
        //float Depth = LoadCameraDepth(positionSS);
        //float EyeDepth = LinearEyeDepth(Depth, _ZBufferParams);
        //float output = saturate(_Distance / EyeDepth);

        //return output;
        
        //return float4(outColor, 1);
    }

    ENDHLSL

    SubShader
    {
        Pass
        {
            Name "CustomOutlinePP"

            ZWrite Off
            ZTest Always
            Blend Off
            Cull Off

            HLSLPROGRAM
                #pragma fragment CustomPostProcess
                #pragma vertex Vert
            ENDHLSL
        }
    }
    Fallback Off
}
