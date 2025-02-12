namespace EnvironmentVolunteer.Core.ApiModels
{
    public class AppSettings
    {
        public Jwt Jwt { get; set; }
        public Admin Admin { get; set; }
        public string AllCharacters { get; set; }
        public Images Images { get; set; }
        public Transaction Transaction { get; set; }
        public SecretKey SecretKey { get; set; }
    }

    public class Jwt
    {
        public string Key { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int AccessTokenExpiresTime { get; set; }
        public int RefreshTokenExpiresTime { get; set; }
    }

    public class Admin
    {
        public int OtpMaxAttempted { get; set; }
        public int MaxAccessFailedAttempts { get; set; }
        public int OtpExpires { get; set; }
        public int RememberMe { get; set; }
        public int FailAccountLock { get; set; }
        public int GapOtp { get; set; }
    }

    public class Template
    {
        public string Folder { get; set; }
        public Directory Directory { get; set; }
    }

    public class Directory
    {
        public string ResetPassword { get; set; }
        public string ResetPasswordUrl { get; set; }
        public string VerifyRegistration { get; set; }
        public string AdminVerifyRegistrationUrl { get; set; }
        public string VerifySignUpUserUrl { get; set; }
        public string ChangeEmail { get; set; }
        public string ChangeEmailUrl { get; set; }
        public string SubscriptionNotification { get; set; }
        public string PaymentNotification { get; set; }
        public string BusinessMemberRegistration { get; set; }
        public string BusinessMemberRegistrationUrl { get; set; }
        public string SendOtpToEmail { get; set; }
        public string NotPaidInvoice { get; set; }
        public string OncePaidInvoice { get; set; }
        public string RecurringPaidInvoice { get; set; }
        public string PaymentUrl { get; set; }
        public string VerifyInvoiceUrl { get; set; }
    }

    public class Images
    {
        public Default Default { get; set; }
    }

    public class Default
    {
        public string Logo { get; set; }
        public string Cover { get; set; }
        public string PrimaryColor { get; set; }
        public string SecondaryColor { get; set; }
    }

    public class Transaction
    {
        public int ExpiredTime { get; set; }
    }

    public class SecretKey
    {
        public string Key { get; set; }
    }
}
