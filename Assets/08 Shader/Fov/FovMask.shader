// 시야 메시(부채꼴)가 있는 픽셀을 스텐실 버퍼에 마킹
// 색상 출력 없음 → 오직 스텐실 마스크 역할만 수행
Shader "Custom/FovMask"
{
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }

        // 시야 영역 픽셀에 스텐실 값 1을 기록
        Stencil
        {
            Ref     2
            Comp    Always
            Pass    Replace
        }

        ColorMask 0   // 색상 버퍼에는 아무것도 쓰지 않음
        ZWrite    Off
        Cull      Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata { float4 vertex : POSITION; };
            struct v2f    { float4 pos    : SV_POSITION; };

            v2f    vert(appdata v) { v2f o; o.pos = UnityObjectToClipPos(v.vertex); return o; }
            fixed4 frag(v2f i)    : SV_Target { return 0; }
            ENDCG
        }
    }
}