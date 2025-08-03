using System.Data;
using System.Reflection;
using System.Runtime.InteropServices;

public class Settings
{
    // const 常量，别的脚本无法更改该值
    public const float itemFadeDuration = 0.35f;
    public const float targetColor = 0.45f;

    // 时间相关
    public const float secondThresHold = 0.1f; // 数值越小时间越快
    public const int secondHold = 59;
    public const int minuteHold = 59;
    public const int hourHold = 23;
    public const int dayHold = 10;  // 一个月有多少天
    public const int seasonHold = 3;

    public const float FadeDuration = 0.5f;

    public const int reapCount = 2;

    public const float gridCellSize = 1f;
    public const float gridCellDiagonalSize = 1.41f;

    public const float pixelSize = 0.05f;   // 如果像素大小为20*20占一个格子，所以一个格子的一个像素点大小为0.05
    public const float animationBreakTime = 5f; 
}
