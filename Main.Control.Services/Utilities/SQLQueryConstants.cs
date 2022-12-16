using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main.Control.Services.Utilities
{
    public class SQLQueryConstants
    {

        public const string GetAppReconcilationPayment = @"Select Transaction_Ref_Id as AppTxnId,Create_Time_Stamp as CreatedTimeStamp  from etf_Payment_Transactions 
where Create_Time_Stamp >= @CurrentDateTimeStart and Create_Time_Stamp <=  @CurrentDateTimeEnd and Transaction_Status='SUCCESS'
and  Is_Deleted =0";

        public const string GetAPIReconcilationPayment = @"Select  Transaction_Reference_Id as ApiTxnId,Transaction_Amount as  TransactionAmount,Email_Address as Email,Create_Time_Stamp as CreatedTimeStamp,Api_Call_Status as APICallStatus from Span_Api_Request_Response_Log where Span_Product_Id = @ProductId and Api_Call_Status in ('Refunded','Voided','REFUND','Success') and 
Create_Time_Stamp >= @CurrentDateTimeStart and Create_Time_Stamp <= @CurrentDateTimeEnd
and Payment_Approval_Code is not null and Is_Deleted = 0";

        public const string GetETTAppReconcilationPayment = @"Select Transaction_Reference_Id as AppTxnId,Create_Time_Stamp as CreatedTimeStamp  from ee_Payment_Transactions where Create_Time_Stamp >= @CurrentDateTimeStart and Create_Time_Stamp <=  @CurrentDateTimeEnd and Payment_Status='Success' and Order_Type
!='AUTHCARD' and  Is_Deleted =0";

        public const string GetEEAppReconcilationPayment = @"Select REPLACE(convert(nvarchar(100), Transaction_Id),'-','')as AppTxnId,Created_Time_Stamp as CreatedTimeStamp
from tax_Payment_Transactions where Created_Time_Stamp >= @CurrentDateTimeStart and
Created_Time_Stamp <=  @CurrentDateTimeEnd and Transaction_Status = 'Success' and Is_Deleted = 0";



        public const string GetChargeBackTypeInPaymentRefundLog = @"Select Charge_Back_Type as ApiCallStatus, Email_Address as Email from Span_Payment_Refund_Log Where REPLACE(Transaction_Reference_Id,'-','') = @TransactionRefId and Charge_Back_Type in ('REFUND','VOID') and Is_Deleted = 0";
    }
}
