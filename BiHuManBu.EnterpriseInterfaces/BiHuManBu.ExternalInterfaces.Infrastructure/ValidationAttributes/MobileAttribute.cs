using System.ComponentModel.DataAnnotations;

namespace BiHuManBu.ExternalInterfaces.Infrastructure.ValidationAttributes
{
    /// <summary>
    /// 验证是否为手机号
    /// </summary>
    public class MobileAttribute : RequiredAttribute
    {
        public MobileAttribute()
        {
            ErrorMessage = "手机号格式不正确";
        }

        public override bool IsValid(object value)
        {
            if (value is string)
            {
                var mobile = value as string;
                return FormatHelper.IsMobile(mobile);
            }
            else
            {
                return false;
            }
        }
    }
}
