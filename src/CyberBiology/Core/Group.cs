using System;

namespace CyberBiology.Core
{
    [Flags]
    public enum Group
    {
        Alone = 1,
        HasPrev = 2,
        HasNext = HasPrev * 2,
        Both = HasPrev | HasNext
    }
}