Shader "Custom/FillQuestProgress"
{
    Properties{
    _Color("Color", Color) = (1, 1, 1, 1) // 커스텀 속성으로 색상 값을 받습니다.
    _MainTex("Sprite Texture", 2D) = "white" // 스프라이트 텍스처를 받습니다.
    }

        SubShader{
            Tags { "Queue" = "Transparent" "RenderType" = "Transparent" } // 투명 렌더링 설정
            Blend One OneMinusSrcAlpha // 블렌딩 모드 설정
            Lighting Off // 조명 끄기
            ZWrite Off // Z 버퍼 쓰기 끄기
            Cull Off // 양면 렌더링 설정
            AlphaTest Greater 0 // 알파 테스트 설정
            ColorMask RGB // 컬러 마스크 설정

            Pass {
                CGPROGRAM
                #pragma vertex vert // 버텍스 함수 선언
                #pragma fragment frag // 프래그먼트 함수 선언
                #pragma multi_compile _ SRGB_ALPHA // 컴파일 옵션 선언
                #include "UnityCG.cginc" // 유니티 셰이더 라이브러리 인클루드

                struct appdata {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                };

                struct v2f {
                    float2 uv : TEXCOORD0;
                    float4 vertex : SV_POSITION;
                };

                sampler2D _MainTex; // 텍스처 변수 선언
                float4 _Color; // 컬러 변수 선언

                v2f vert(appdata v) {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex); // 버텍스 위치 변환
                    o.uv = v.uv; // 텍스처 좌표 전달
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target {
                    fixed4 col = tex2D(_MainTex, i.uv); // 텍스처에서 색상 읽어오기
                    col.rgb = col.a * _Color.rgb; // 수정된 부분: 텍스처 RGB에 알파 값을 곱하여 검정색에 대한 올바른 색상 적용
                    col.a = col.a * _Color.a; // 수정된 부분: 텍스처 알파 값에 알파 값을 곱하여 투명도 조절
                    return col; // 최종 색상 반환
                }
                ENDCG
            }
    }
}
