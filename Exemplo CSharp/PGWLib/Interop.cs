using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static PGWLib.CustomObjects;

namespace PGWLib
{
    public class Interop
    {

        [DllImport("PGWebLib.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern short PW_iInit(string pszWorkingDir);

        [DllImport("PGWebLib.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern short PW_iNewTransac(byte bOper);

        [DllImport("PGWebLib.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern short PW_iAddParam(ushort wParam, string pszValue);

        [DllImport("PGWebLib.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern short PW_iExecTransac([Out] PW_GetData[] vstParam, ref short piNumParam);

        [DllImport("PGWebLib.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern short PW_iGetResult(short iInfo, StringBuilder pszData, uint ulDataSize);

        [DllImport("PGWebLib.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern short PW_iConfirmation(uint ulResult, string pszReqNum, string pszLocRef,
                                               string pszExtRef, string pszVirtMerch, string pszAuthSyst);

        [DllImport("PGWebLib.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern short PW_iIdleProc();

        [DllImport("PGWebLib.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern short PW_iGetOperations(byte bOperType, ref PW_Operations[] vstOperations, ref short piNumOperations);

        [DllImport("PGWebLib.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern short PW_iPPEventLoop( StringBuilder pszDisplay, uint ulDisplaySize);

        [DllImport("PGWebLib.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern short PW_iPPAbort();

        [DllImport("PGWebLib.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern short PW_iPPGetCard(ushort uiIndex);

        [DllImport("PGWebLib.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern short PW_iPPGetPIN(ushort uiIndex);

        [DllImport("PGWebLib.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern short PW_iPPGetData(ushort uiIndex);

        [DllImport("PGWebLib.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern short PW_iPPGetUserData(short uiMessageId, short bMinLen, short bMaxLen, short iToutSec, StringBuilder pszData);

        [DllImport("PGWebLib.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern short PW_iPPGoOnChip(ushort uiIndex);

        [DllImport("PGWebLib.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern short PW_iPPFinishChip(ushort uiIndex);

        [DllImport("PGWebLib.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern short PW_iPPConfirmData(ushort uiIndex);

        [DllImport("PGWebLib.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern short PW_iPPRemoveCard();

        [DllImport("PGWebLib.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern short PW_iPPDisplay(string pszMsg);

        [DllImport("PGWebLib.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern short PW_iPPWaitEvent(ref uint pulEvent);

        [DllImport("PGWebLib.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern short PW_iPPGenericCMD(ushort uiIndex);

        [DllImport("PGWebLib.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern short PW_iPPPositiveConfirmation(ushort uiIndex);



    }
}
