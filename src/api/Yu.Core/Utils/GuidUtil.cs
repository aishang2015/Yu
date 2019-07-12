using Microsoft.EntityFrameworkCore.ValueGeneration;
using System;

namespace Yu.Core.Utils
{
    public static class GuidUtil
    {
        private static ValueGenerator<Guid> _generator = new SequentialGuidValueGenerator();

        public static Guid NewSquentialGuid()
        {
            return _generator.Next(null);
        }
    }
}
