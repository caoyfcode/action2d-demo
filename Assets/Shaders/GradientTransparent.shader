Shader "Custom/UI/GradientTransparent"
{
    Properties
    {
        _TransparentStart("Transparent Start", Range(0,1)) = 0.8
        _ColorStart("Color Start", Color) = (0,0,0,1)
        _TransparentEnd("Transparent End", Range(0,1)) = 1
        _ColorEnd("Color End", Color) = (0,0,0,0)
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "PreviewType"="Plane" }
        LOD 100
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            float _TransparentStart;
            float4 _ColorStart;
            float _TransparentEnd;
            float4 _ColorEnd;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col;
                col.r = _ColorStart.r - smoothstep(_TransparentStart, _TransparentEnd, i.uv.x) * (_ColorStart.r - _ColorEnd.r);
                col.g = _ColorStart.g - smoothstep(_TransparentStart, _TransparentEnd, i.uv.x) * (_ColorStart.g - _ColorEnd.g);
                col.b = _ColorStart.b - smoothstep(_TransparentStart, _TransparentEnd, i.uv.x) * (_ColorStart.b - _ColorEnd.b);
                col.a = _ColorStart.a - smoothstep(_TransparentStart, _TransparentEnd, i.uv.x) * (_ColorStart.a - _ColorEnd.a);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
