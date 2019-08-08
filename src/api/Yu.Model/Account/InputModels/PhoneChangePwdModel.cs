namespace Yu.Model.Account.InputModels
{
    public class PhoneChangePwdModel
    {

        public string PhoneNumber { get; set; }

        public string ValidCode { get; set; }

        public string NewPassword { get; set; }

        public string ConfirmNewPassword { get; set; }
    }
}
