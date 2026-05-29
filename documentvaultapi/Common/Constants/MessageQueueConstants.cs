namespace documentvaultapi.Common.Constants
{
    public static class MessageQueueConstants
    {

        //UM to Document Storage
        public const string UM_APPLICATION_MAP = "usermanagement_documentstorage_application_map";
        //ACK
        public const string UM_APPLICATION_MAP_ACK = "usermanagement_documentstorage_application_map_ack";

        //Document Upload
        //public const string DOCUMENT_UPLOAD_QUEUE = "documentstorage_document_upload";
        //public const string DOCUMENT_UPLOAD_ACK = "documentstorage_document_upload_ack";
        public const string DOCUMENT_UPLOAD_QUEUE = "documentstorage_document_upload1";
        public const string DOCUMENT_UPLOAD_ACK = "documentstorage_document_upload1_ack";

        public const string WBJIT_BILLING_BILL_RECV = "wbjit_billing_cts_bill_send_to_treasury";
        public const string  WBJIT_BILLING_BILL_STATUS = "wbjit_cts_billing_bill_status";
        public const string  WBJIT_BILLING_FAILED_BEN = "wbjit_cts_billing_failed_beneficiary";
        public const string  WBJIT_BILLING_SUCCESS_BEN = "wbjit_cts_billing_success_beneficiary";
        public const string  WBJIT_BILLING_FAILED_PFMS_LOT = "wbjit_cts_billing_pfms_failed";
        // public const string  JIT_SCHEME_CONFIG = "wbjit_jit_cts_scheme_configuration";
        public const string  WBJIT_BILLING_SCHEME_CONFIG = "wbjit_billing_cts_scheme_config_master";
        public const string  BILLING_JIT_ALLOTMENT = "wbjit_billing_cts_ddo_allotment";
        public const string  WBJIT_BILLING_ACTIVE_HOA_MASTER = "wbjit_billing_cts_active_hoa_master";
        public const string  WBJIT_BILLING_MOTHER_SANCTION = "wbjit_billing_cts_mother_sanction_allocation";
        public const string  WBJIT_BILLING_OBJECTED_BILL = "wbjit_cts_billing_objected_bill";
        public const string  WBJIT_BILLING_TOKEN = "wbjit_cts_billing_token";
        public const string  WBJIT_BILLING_VOUCHER = "wbjit_cts_billing_voucher";
        public const string  WBJIT_BILLING_DDO_ALLOTMENT = "wbjit_billing_cts_ddo_allotment";
        public const string WBJIT_BILLING_DDO_ALLOTMENT_ACTUAL = "wbjitcts_billing_ddo_allotment_actual_amount";
        public const string WBJIT_CTS_IFSC_CODE = "wbjit_cts_ifsc_code";

        // public const string  BILLING_BILL_RECV = "billing_cts_bill_send_to_treasury";
        public const string  BILLING_BILL_STATUS = "cts_billing_bill_status";
        public const string BILLING_FAILED_BEN = "cts_billing_failed_beneficiary";
        public const string BILLING_SUCCESS_BEN = "cts_billing_success_beneficiary";
        public const string BILLING_FAILED_PFMS_LOT = "cts_billing_pfms_failed";
        public const string BILLING_ACTIVE_HOA_MASTER = "billing_cts_active_hoa_master";
        public const string BILLING_MOTHER_SANCTION = "billing_cts_mother_sanction_allocation";
        public const string BILLING_OBJECTED_BILL = "cts_billing_objected_bill";
        public const string BILLING_TOKEN = "cts_billing_token";
        public const string BILLING_VOUCHER = "cts_billing_voucher";
        public const string BILLING_DDO_ALLOTMENT = "billing_cts_ddo_allotment";
        public const string E_BILLING_BILL_RECV = "ebilling_to_treasury";
        public const string E_BILLING_DDO_ALLOTMENT_ACTUAL = "cts_billing_ddo_allotment_actual_amount";
        public const string BILLING_DDO_ALLOTMENT_WITHDRAWAL = "billing_cts_ddo_allotment_withdrawl";
        public const string WBJIT_BILLING_DDO_ALLOTMENT_WITHDRAWAL = "wbjit_billing_cts_ddo_allotment_withdrawl";
        public const string WBJIT_PFMS_FILE_STATUS = "wbjit_cts_billing_pfms_file_status_details";
       

        //Master to CTS Exchange
        public const string MASTER_TO_CTS_FINANCIAL_YEAR = "master_to_cts_financial_year";
        public const string MASTER_TO_CTS_TREASURY = "master_to_cts_treasury";
        public const string MASTER_TO_CTS_DDO = "master_to_cts_ddo";
        public const string MASTER_TO_CTS_MAJOR_HAED = "master_to_cts_major_head";
        public const string MASTER_TO_CTS_MINOR_HEAD = "master_to_cts_minor_head";
        public const string MASTER_TO_CTS_RBI_IFSC_STOCK = "master_to_cts_rbi_ifsc_stock";
        public const string MASTER_TO_CTS_DETAIL_HEAD = "master_to_cts_detail_head";
        public const string MASTER_TO_CTS_SCHEME_HEAD = "master_to_cts_scheme_head";
        public const string MASTER_TO_CTS_SCHEME_TYPE = "master_to_cts_scheme_type";
        public const string MASTER_TO_CTS_SUB_DETAIL_HEAD = "master_to_cts_sub_detail_head";
        public const string MASTER_TO_CTS_DEPARTMENT = "master_to_cts_department";
        public const string MASTER_TO_CTS_ACTIVE_HOA_MST = "master_to_cts_active_hoa_mst";
        public const string MASTER_TO_CTS_SUB_MAJOR_HEAD = "master_to_cts_sub_major_head";
        public const string MASTER_TO_CTS_SUB_SCHEME_TYPE = "master_to_cts_sub_scheme_type";

        //Ack Consumer 
        public const string WBJIT_CTS_BILLING_BILL_STATUS_ACK = "wbjit_cts_billing_bill_status_ack";
        public const string WBJIT_CTS_BILLING_FAILED_BENEFICIARY_ACK = "wbjit_cts_billing_failed_beneficiary_ack";
        public const string WBJIT_CTS_BILLING_OBJECTED_BILL_ACK = "wbjit_cts_billing_objected_bill_ack";
        public const string WBJIT_CTS_BILLING_PFMS_FAILED_ACK = "wbjit_cts_billing_pfms_failed_ack";
        public const string WBJIT_CTS_BILLING_PFMS_FILE_STATUS_DETAILS_ACK = "wbjit_cts_billing_pfms_file_status_details_ack";
        public const string WBJIT_CTS_BILLING_SUCCESS_BENEFICIARY_ACK = "wbjit_cts_billing_success_beneficiary_ack";
        public const string WBJIT_CTS_BILLING_TOKEN_ACK = "wbjit_cts_billing_token_ack";
        public const string WBJIT_CTS_BILLING_VOUCHER_ACK = "wbjit_cts_billing_voucher_ack";
        public static string JIT_IFSC_ACK = "jit_ifsc_ack";

        //now not send from treasury end
        public const string WBJIT_CTS_BILLING_CHALLAN_ACK = "wbjit_cts_billing_challan_ack";
        public const string WBJIT_CTS_BILLING_DDO_ALLOTMENT_ACTUAL_AMOUNT_ACK = "wbjit_cts_billing_ddo_allotment_actual_amount_ack";

        public const string UM_APPLICATION = "um_application_map";



    }
}
