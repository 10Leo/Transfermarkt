using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Transfermarkt.Core.Actors;
using Transfermarkt.Core.Contracts;
using Transfermarkt.Core.ParseHandling.Contracts;
using Transfermarkt.Core.ParseHandling.Contracts.Converter;
using Transfermarkt.Logging;

namespace Transfermarkt.Core.ParseHandling.Converters
{
    public class FootConverter : IFootConverter
    {
        private ILogger logger;

        public FootConverter() : this(null) { }

        public FootConverter(ILogger logger)
        {
            this.logger = logger;
        }

        public FootValue Convert(string stringToConvert)
        {
            return new FootValue { Value = ConvertersConfig.GetFoot(stringToConvert) };
        }
    }
}
