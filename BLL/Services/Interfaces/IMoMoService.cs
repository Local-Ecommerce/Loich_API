﻿using BLL.Dtos.MoMo.CaptureWallet;
using BLL.Dtos.MoMo.IPN;

namespace BLL.Services.Interfaces
{
    public interface IMoMoService
    {
        /// <summary>
        /// Create Capture Wallet
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        MoMoCaptureWalletResponse CreateCaptureWallet(MoMoCaptureWalletRequest requestData);


        /// <summary>
        /// Process IPN
        /// </summary>
        /// <param name="momoIPNRequest"></param>
        /// <returns></returns>
        MoMoIPNResponse ProcessIPN(MoMoIPNRequest momoIPNRequest);


        /// <summary>
        /// Send Momo Payment Response To Client
        /// </summary>
        /// <param name="momoIPNRequest"></param>
        void SendMomoPaymentResponseToClient(MoMoIPNRequest momoIPNRequest);
    }
}