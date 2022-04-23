Shader "Unlit/TerrainGrassShader"
{
    Properties
    {
    }
    
//    HLSLINCLUDE
//    // #include "Packages/com.unity.render-pipelines.high-definition/ShaderLibrary/Core.hlsl"
    
    CGINCLUDE
    #include "UnityCG.cginc"
    #include "Lighting.cginc"
    #include "AutoLight.cginc"
    #pragma multi_compile _SHADOWS_SCREEN
    #pragma multi_compile_fwdbase_fullforwardshadows
    #pragma multi_compile_fog
    #pragma shader_feature FADE

    struct DrawVertex
    {
        float3 positionWS;
        float2 uv;
        float3 diffuseColor;
    };

    struct DrawTriangle
    {
        float3 normalOS;
        DrawVertex verts[3];
    };

    StructuredBuffer<DrawTriangle> _DrawTriangles;

    struct v2f
    {
        float4 pos : SV_POSITION;
        float2 uv : TEXCOORD0;
        float3 positionWS : TEXCOORD1;
        float3 normalWS : TEXCOORD2;
        float3 diffuseColor : COLOR;
        LIGHTING_COORDS(3, 4)
        UNITY_FOG_COORDS(5)
    };

    float4 _TopTint;
    float4 _BottomTint;
    float _AmbientStrength;
    float _Fade;

    // struct unityTransferVertexToFragmentSucksHack
    // {
        // float3 vertex : POSITION;
    // };

    v2f vert(uint vertexID : SV_VertexID)
    {
        v2f output = (v2f)0;

        DrawTriangle tri = _DrawTriangles[vertexID / 3];
        DrawVertex input = tri.verts[vertexID % 3];

        output.pos = UnityObjectToClipPos(input.positionWS);
        output.positionWS = input.positionWS;

        float3 faceNormal = tri.normalOS;
        output.normalWS = faceNormal;

        // unityTransferVertexToFragmentSucksHack v;
        // v.vertex = output.pos;

        TRANSFER_VERTEX_TO_FRAGMENT(output);
        UNITY_TRANSFER_FOG(output, output.pos);

        return output;
    }

    ENDCG
    
    SubShader
    {
        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha
        Pass
        {
            Tags {
                "LightMode" = "ForwardBase"
            }
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            float4 frag(v2f i) : SV_Target
            {
                float shadow = 1;
                #if defined(SHADOWS_SCREEN)
                shadow = (SAMPLE_DEPTH_TEXTURE_PROJ(_ShadowMapTexture, UNITY_PROJ_COORD(i._ShadowCoord)).r);
                #endif

                return float4(0, 0, 0, 0);
            }

            ENDCG
        }
    }
}
