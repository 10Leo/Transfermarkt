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
    public class NationalityConverter : INationalityConverter
    {
        private ILogger logger;

        public NationalityConverter() : this(null) { }

        public NationalityConverter(ILogger logger)
        {
            this.logger = logger;
        }

        public NationalityValue Convert(string stringToConvert)
        {
            return new NationalityValue { Value = ConvertersConfig.GetNationality(stringToConvert) };
        }
    }
}
