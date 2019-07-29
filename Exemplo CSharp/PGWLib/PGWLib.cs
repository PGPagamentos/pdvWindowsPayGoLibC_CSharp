using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static PGWLib.CustomObjects;
using static PGWLib.Enums;

namespace PGWLib
{
    public class PGWLib
    {

        public PGWLib()
        {
            System.IO.Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\PGWebLib\\");
            String path = "";
            path = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\PGWebLib\\";
            int ret = Interop.PW_iInit(path);

        }

        public int startTransaction(E_PWOPER operation, List<PW_Parameter> paramList)
        {
            
            int ret = (int)E_PWRET.PWRET_NODATA;
            ret = Interop.PW_iNewTransac((byte)operation);

            
            if (ret != (int)E_PWRET.PWRET_OK) return ret;

            foreach (PW_Parameter item in paramList)
            {
                ret = Interop.PW_iAddParam(item.parameterCode, item.parameterValue);
                Debug.Print(string.Format("CALLED iAddParam COM ÍNDICE {0}, VALOR {1} E RETORNO {2}", item.parameterName, item.parameterValue, ret.ToString()));
                if (ret != 0) return ret;
            }

            ret = executeTransaction();

            
            return ret;
        }

        public List<PW_Parameter> getTransactionResult()
        {
            List<PW_Parameter> ret = new List<PW_Parameter>();

            string message = string.Empty;

            foreach (E_PWINFO item in Enum.GetValues(typeof(E_PWINFO)).Cast<E_PWINFO>())
            {
                StringBuilder value = new StringBuilder(10000);
                int getInfoRet = Interop.PW_iGetResult((short)item, value, 10001);
                if (getInfoRet == 0) ret.Add(new PW_Parameter(item.ToString(), (ushort)item, value.ToString()));
                if (getInfoRet == 0 && item == E_PWINFO.PWINFO_RESULTMSG) message = value.ToString();

            }

            if (!string.IsNullOrEmpty(message))
            {
                FormDisplayMessage busyForm = new FormDisplayMessage(message);
                busyForm.Start();
                Thread.Sleep(3000);
                busyForm.Stop();

            }

            return ret;
        }

        public int confirmUndoTransaction(E_PWCNF transactionStatus, List<PW_Parameter> transactionResponse)
        {
            int ret = 99;

            string pszReqNum = string.Empty;
            string pszLocRef = string.Empty;
            string pszExtRef = string.Empty;
            string pszVirtMerch = string.Empty;
            string pszAuthSyst = string.Empty;

            foreach (PW_Parameter item in transactionResponse)
            {
                switch (item.parameterCode)
                {
                    case (ushort)E_PWINFO.PWINFO_REQNUM:
                        pszReqNum = item.parameterValue;
                        break;

                    case (ushort)E_PWINFO.PWINFO_AUTLOCREF:
                        pszLocRef = item.parameterValue;
                        break;

                    case (ushort)E_PWINFO.PWINFO_AUTEXTREF:
                        pszExtRef = item.parameterValue;
                        break;

                    case (ushort)E_PWINFO.PWINFO_VIRTMERCH:
                        pszVirtMerch = item.parameterValue;
                        break;

                    case (ushort)E_PWINFO.PWINFO_AUTHSYST:
                        pszAuthSyst = item.parameterValue;
                        break;

                    default:
                        break;
                }
            }

            ret = Interop.PW_iConfirmation((uint)transactionStatus, pszReqNum, pszLocRef, pszLocRef, pszVirtMerch, pszAuthSyst);

            return ret;
        }

        public int getInputFromPP(ref string userTypedValue, E_PWUserDataMessages messageToDisplay, int minLength, int MaxLength){

            StringBuilder value = new StringBuilder(10000);
            short ret = 0;

            ret = Interop.PW_iPPGetUserData((short)messageToDisplay, (short)minLength, (short)MaxLength, 30, value);
            Debug.Print(string.Format("CALLED iPPGetUserData COM RETORNO {0}", ret.ToString()));
            if (ret != 0) return (int)ret;

            userTypedValue = value.ToString();
            Debug.Print(string.Format("E VALOR {0}", userTypedValue));

            ret = (short)loopPP();
            if (ret != 0) return (int)ret;
            
            return (int)ret;

        }

        public int displayOnPP(string messageToDisplay, int timeout)
        {
            short internalRet = Interop.PW_iPPDisplay(messageToDisplay);
            if (internalRet != 0) return (int)internalRet;

            internalRet = (short)loopPP();
            if (internalRet == 0) Thread.Sleep(timeout);
            internalRet = (short)Interop.PW_iPPDisplay("");
            internalRet = (short)loopPP();

            return internalRet;
        }

        public int waitForCardRemoval()
        {
            int ret = 0;
            ret = Interop.PW_iPPRemoveCard();
            Debug.Print(string.Format("CALLED iPPRemoveCard COM RETORNO {0}", ret.ToString()));
            if (ret == (int)E_PWRET.PWRET_OK) ret = loopPP();
            return ret;
        }

        private int executeTransaction()
        {
            bool keepGoing = true;

            int ret = (int)E_PWRET.PWRET_NODATA;

            while (keepGoing)
            {
                PW_GetData[] structParam = new PW_GetData[10];
                short paramAmount = 10;
                ret = Interop.PW_iExecTransac(structParam, ref paramAmount);
                Debug.Print(string.Format("CALLED iExecTransac COM RETORNO {0}", ret.ToString()));
                switch (ret)
                {
                    case (int)E_PWRET.PWRET_MOREDATA:
                        int ret2 = showCorrespondingWindow(structParam);
                        if (ret2 != (int)E_PWRET.PWRET_OK) return ret2;
                        break;
                    default:
                        return ret;
                }
            }
            
            return ret;
        }

        private int showCorrespondingWindow(PW_GetData[] expectedData)
        {
            int ret = 0;
            ushort index = 0;

            foreach (PW_GetData item in expectedData)
            {
                ret = 0;
                switch (item.bTipoDeDado)
                {
                    case 0:
                        Debug.Print(string.Format("ITEM COM VALOR ZERO"));
                        return ret;

                    case (int)E_PWDAT.PWDAT_BARCODE:
                        ret = getTypedDataFromUser(item);
                        if(ret == 0) return ret;
                        break;

                    case (int)E_PWDAT.PWDAT_CARDINF:
                        if (item.ulTipoEntradaCartao == 1)
                        {
                            PW_GetData temp = item;
                            temp.wIdentificador = (ushort)E_PWINFO.PWINFO_CARDFULLPAN;
                            ret = getTypedDataFromUser(temp);
                        }
                        else
                        {
                            ret = Interop.PW_iPPGetCard(index);
                            if (ret == (int)E_PWRET.PWRET_OK) ret = loopPP();
                        }
                        if (ret == 0) return ret;
                        break;

                    case (int)E_PWDAT.PWDAT_CARDONL:
                        ret = Interop.PW_iPPFinishChip(index);
                        Debug.Print(string.Format("CALLED iPPFinishChip COM RETORNO {0}", ret.ToString()));
                        if (ret == (int)E_PWRET.PWRET_OK) ret = loopPP();
                        if (ret == 0) return ret;
                        break;

                    case (int)E_PWDAT.PWDAT_CARDOFF:
                        ret = Interop.PW_iPPGoOnChip(index);
                        Debug.Print(string.Format("CALLED iPPGoOnChip COM RETORNO {0}", ret.ToString()));
                        if (ret == (int)E_PWRET.PWRET_OK) ret = loopPP();
                        if (ret == 0) return ret;
                        break;

                    case (int)E_PWDAT.PWDAT_MENU:
                        return getMenuFromUser(item);

                    case (int)E_PWDAT.PWDAT_PPCONF:
                        ret = Interop.PW_iPPConfirmData(index);
                        Debug.Print(string.Format("CALLED iPPConfirmData COM RETORNO {0}", ret.ToString()));
                        if (ret == (int)E_PWRET.PWRET_OK) ret = loopPP();
                        if (ret == 0) return ret;
                        break;

                    case (int)E_PWDAT.PWDAT_PPENCPIN:
                        ret = Interop.PW_iPPGetPIN(index);
                        Debug.Print(string.Format("CALLED iPPGetPIN COM RETORNO {0}", ret.ToString()));
                        if (ret == (int)E_PWRET.PWRET_OK) ret = loopPP();
                        if (ret == 0) return ret;
                        break;

                    case (int)E_PWDAT.PWDAT_PPENTRY:
                        ret = Interop.PW_iPPGetData(index);
                        Debug.Print(string.Format("CALLED iPPGetData COM RETORNO {0}", ret.ToString()));
                        if (ret == (int)E_PWRET.PWRET_OK) ret = loopPP();
                        if (ret == 0) return ret;
                        break;

                    case (int)E_PWDAT.PWDAT_PPREMCRD:
                        ret = Interop.PW_iPPRemoveCard();
                        Debug.Print(string.Format("CALLED iPPRemoveCard COM RETORNO {0}", ret.ToString()));
                        if (ret == (int)E_PWRET.PWRET_OK) ret = loopPP();
                        if (ret == 0) return ret;
                        break;

                    case (int)E_PWDAT.PWDAT_TYPED:
                        ret = getTypedDataFromUser(item);
                        return ret;

                    case (int)E_PWDAT.PWDAT_USERAUTH:
                        ret = getTypedDataFromUser(item);
                        return ret;

                    default:
                        break;
                }
                index++;
            }
            return ret;
        }

        private int getMenuFromUser(PW_GetData expectedData)
        {
            bool userAborted = false;
            string value = string.Empty;
            FormMenu window = new FormMenu(expectedData);
            window.ShowDialog(ref userAborted, ref value);

            if (userAborted) return (int)E_PWRET.PWRET_CANCEL;

            return Interop.PW_iAddParam(expectedData.wIdentificador, value);
        }

        private int loopPP()
        {
            
            FormDisplayMessage busyForm = new FormDisplayMessage();
            bool formOpened = false;
            int ret = 99;
            do
            {
                Thread.Sleep(200);
                StringBuilder displayMessage = new StringBuilder(1000);
                Debug.Print(string.Format("CALLED iPPEventLoop", displayMessage.ToString(), ret.ToString()));
                ret = Interop.PW_iPPEventLoop(displayMessage, (uint)1000);
                Debug.Print(string.Format("CALLED iPPEventLoop COM MESSAGE {0} E RETORNO {1}", displayMessage.ToString(), ret.ToString()));

                if (ret == (int)E_PWRET.PWRET_DISPLAY)
                {
                    if (!formOpened)
                    {
                        formOpened = true;
                        busyForm = new FormDisplayMessage(displayMessage.ToString());
                        busyForm.Start();
                    }
                    else
                    {
                        busyForm.ChangeText(displayMessage.ToString());
                    }
                    
                }

            } while (ret == (int)E_PWRET.PWRET_NOTHING ||
            ret == (int)E_PWRET.PWRET_DISPLAY);

            busyForm.Stop();

            return ret;
        }

        private int getTypedDataFromUser(PW_GetData expectedData)
        {
            bool userAborted = false;
            string value = string.Empty;
            FormTypedData window = new FormTypedData(expectedData);
            window.ShowDialog(ref userAborted, ref value);

            if (userAborted) return (int)E_PWRET.PWRET_CANCEL;

            int ret = 99;
            ret = Interop.PW_iAddParam(expectedData.wIdentificador, value);
            
            Debug.Print(string.Format("CALLED iAddParam COM ÍNDICE {0}, VALOR {1} E RETORNO {2}", ((E_PWINFO)expectedData.wIdentificador).ToString(), value, ret.ToString()));

            return ret;

        }
    }
}
