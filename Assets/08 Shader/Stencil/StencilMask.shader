
Shader "Custom/StencilMask"
{
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry-1" }

        // 색상 버퍼에는 아무것도 쓰지 않음 (투명하게)
        ColorMask 0
        ZWrite Off

        Stencil
        {
            Ref 1           // 기준값 1
            Comp Always     // 항상 통과
            Pass Replace    // 스텐실 버퍼에 Ref(1)을 기록
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata { float4 vertex : POSITION; };
            struct v2f    { float4 pos : SV_POSITION; };

            v2f vert(appdata v) { v2f o; o.pos = UnityObjectToClipPos(v.vertex); return o; }
            fixed4 frag(v2f i) : SV_Target { return fixed4(0,0,0,0); }
            ENDCG
        }
    }
}