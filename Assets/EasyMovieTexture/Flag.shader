Shader "Custom/Flag" {

    Properties

    {

        _MainTex("MainTex",2D)="White"{}

        _MainColor("MainColor",color)=(1,1,1,1)

        _ScaleX("ScaleX",float)=1

        _ScaleZ("ScaleZ",float)=1

        _Slant("Slant",Range(0,3))=1

        _SpeedX("Speed",float)=1

        _SpeedZ("Speed",float)=1

    }

    SubShader

    {

        pass

        {

             ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha

              CGPROGRAM

              #pragma vertex vert 

              #pragma fragment frag 

              #include "UnityCG.cginc"

              struct v2f

              {

                    float4 pos:POSITION;

                    float2 uv:TEXCOORD0;

               };

               sampler2D _MainTex;

               float4 _MainTex_ST;

               fixed4 _MainColor;

               float _ScaleX;

               float _ScaleZ;

               float _SpeedX;

               float _SpeedZ;

               float _Slant;

               v2f vert(appdata_base v)

               {

                     v2f o;

                     float x = (1 - (v.vertex.x + 5)/10);

                     v.vertex.y += _ScaleX * x * sin(v.vertex.x + _Time.z*_SpeedX);

                     v.vertex.y += _ScaleZ * x * sin(v.vertex.z + _Time.y*_SpeedZ);

                     v.vertex.z += _Slant * x;

                     o.pos = UnityObjectToClipPos(v.vertex);

                     o.uv = TRANSFORM_TEX(v.texcoord,_MainTex);

                      return o;

                 }

                 fixed4 frag(v2f i):COLOR

                 {

                       fixed4 color = tex2D(_MainTex,i.uv);
                       return color*_MainColor;

                  }

             ENDCG

             }

      }

}