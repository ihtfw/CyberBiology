using System;

namespace CyberBiology.Core.Enums
{
    public static class CheckResultUtils
    {
        public static readonly int Count;

        static CheckResultUtils()
        {
            Count = Enum.GetValues(typeof(CheckResult)).Length;
        }
    }
}