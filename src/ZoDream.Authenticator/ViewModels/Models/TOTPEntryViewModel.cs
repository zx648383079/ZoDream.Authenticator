using OtpNet;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Web;
using ZoDream.Shared.Database;

namespace ZoDream.Authenticator.ViewModels.Models
{
    public class TOTPEntryViewModel : EntryItemViewModel, ITOTPEntryEntity
    {
        public string Secret { get; set; } = string.Empty;

        public string Url { get; set; } = string.Empty;

        public string Algorithm { get; set; } = "sha1";

        public int Period { get; set; } = 6;

        public int Digits { get; set; } = 30;

        public TOTPEntryViewModel()
        {
            
        }


        public string TryGetCode(out int timeout) 
        {
            var instance = new Totp(Base32Encoding.ToBytes(Secret), Digits, Algorithm.ToLower() switch
            {
                "sha512" => OtpHashMode.Sha512,
                "sha256" => OtpHashMode.Sha256,
                _ => OtpHashMode.Sha1,
            }, Period);
            var now = DateTime.UtcNow;
            timeout = instance.RemainingSeconds(now); // 动态码剩余多少秒
            return instance.ComputeTotp(now);
        }

        public static bool TryParse(string url, [NotNullWhen(true)] out TOTPEntryViewModel? model)
        {
            // otpauth://totp/zodream?secret=1&issuer=localhost&period=6&algorithm=sha1&digits=30
            if (!url.StartsWith("otpauth://") || !Uri.TryCreate(url, UriKind.Absolute, out var uri))
            {
                model = null;
                return false;
            }
            model = new();
            model.Account = uri.AbsolutePath[1..];
            var queries = HttpUtility.ParseQueryString(uri.Query);
            model.Secret = queries.Get(nameof(Secret).ToLower()) ?? string.Empty;
            model.Title = model.Url = queries.Get("issuer") ?? string.Empty;
            model.Algorithm = queries.Get(nameof(Algorithm).ToLower()) ?? string.Empty;
            if (int.TryParse(queries.Get(nameof(Period).ToLower()), out var res))
            {
                model.Period = res;
            }
            if (int.TryParse(queries.Get(nameof(Digits).ToLower()), out res))
            {
                model.Digits = res;
            }
            return true;
        }
    }
}
