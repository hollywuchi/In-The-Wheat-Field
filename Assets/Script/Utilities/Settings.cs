using System.Data;
using System.Reflection;
using System.Runtime.InteropServices;

public class Settings
{
    // const 常量，别的脚本无法更改该值
    public const float fadeDuration = 0.35f;
    public const float targetColor = 0.45f;

    // 时间相关
    public const float secondThresHold = 0.1f; // 数值越小时间越快
    public const int secondHold = 59;
    public const int minuteHold = 59;
    public const int hourHold = 23;
    public const int dayHold = 10;  // 一个月有多少天
    public const int seasonHold = 3;
}
