using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PGWLib
{
    public class CustomObjects
    {
        public class PW_Parameter
        {
            public string parameterName;
            public ushort parameterCode;
            public string parameterValue;

            public override string ToString()
            {
                return string.Format("{0}({1}): {2}", parameterName, parameterCode, parameterValue);
            }

            public PW_Parameter()
            {

            }

            public PW_Parameter(string Name, ushort Code, string Value)
            {
                parameterName = Name;
                parameterCode = Code;
                parameterValue = Value;
            }
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct PW_Operations
        {
            byte bOperType;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 21)]
            string szText;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 21)]
            string szValue;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct PW_GetData
        {
            public ushort wIdentificador;
            public byte bTipoDeDado;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 84)]
            public string szPrompt;
            public byte bNumOpcoesMenu;
            public PW_Menu stMenu;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 41)]
            public string szMascaraDeCaptura;
            public byte bTiposEntradaPermitidos;
            public byte bTamanhoMinimo;
            public byte bTamanhoMaximo;
            public int ulValorMinimo;
            public int ulValorMaximo;
            public byte bOcultarDadosDigitados;
            public byte bValidacaoDado;
            public byte bAceitaNulo;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 41)]
            public string szValorInicial;
            public byte bTeclasDeAtalho;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 84)]
            public string szMsgValidacao;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 84)]
            public string szMsgConfirmacao;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 84)]
            public string szMsgDadoMaior;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 84)]
            public string szMsgDadoMenor;
            public byte bCapturarDataVencCartao;
            public int ulTipoEntradaCartao;
            public byte bItemInicial;
            public byte bNumeroCapturas;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 84)]
            public string szMsgPrevia;
            public byte bTipoEntradaCodigoBarras;
            public byte bOmiteMsgAlerta;
            public byte bIniciaPelaEsquerda;
            public byte bNotificarCancelamento;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct PW_Menu
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 41)]
            public string szTexto1;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 41)]
            public string szTexto2;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 41)]
            public string szTexto3;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 41)]
            public string szTexto4;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 41)]
            public string szTexto5;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 41)]
            public string szTexto6;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 41)]
            public string szTexto7;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 41)]
            public string szTexto8;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 41)]
            public string szTexto9;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 41)]
            public string szTexto10;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 41)]
            public string szTexto11;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 41)]
            public string szTexto12;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 41)]
            public string szTexto13;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 41)]
            public string szTexto14;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 41)]
            public string szTexto15;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 41)]
            public string szTexto16;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 41)]
            public string szTexto17;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 41)]
            public string szTexto18;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 41)]
            public string szTexto19;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 41)]
            public string szTexto20;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 41)]
            public string szTexto21;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 41)]
            public string szTexto22;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 41)]
            public string szTexto23;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 41)]
            public string szTexto24;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 41)]
            public string szTexto25;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 41)]
            public string szTexto26;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 41)]
            public string szTexto27;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 41)]
            public string szTexto28;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 41)]
            public string szTexto29;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 41)]
            public string szTexto30;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 41)]
            public string szTexto31;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 41)]
            public string szTexto32;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 41)]
            public string szTexto33;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 41)]
            public string szTexto34;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 41)]
            public string szTexto35;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 41)]
            public string szTexto36;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 41)]
            public string szTexto37;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 41)]
            public string szTexto38;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 41)]
            public string szTexto39;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 41)]
            public string szTexto40;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string szValor1;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string szValor2;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string szValor3;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string szValor4;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string szValor5;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string szValor6;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string szValor7;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string szValor8;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string szValor9;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string szValor10;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string szValor11;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string szValor12;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string szValor13;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string szValor14;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string szValor15;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string szValor16;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string szValor17;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string szValor18;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string szValor19;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string szValor20;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string szValor21;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string szValor22;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string szValor23;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string szValor24;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string szValor25;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string szValor26;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string szValor27;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string szValor28;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string szValor29;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string szValor30;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string szValor31;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string szValor32;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string szValor33;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string szValor34;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string szValor35;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string szValor36;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string szValor37;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string szValor38;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string szValor39;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string szValor40;
        }
    }
}
