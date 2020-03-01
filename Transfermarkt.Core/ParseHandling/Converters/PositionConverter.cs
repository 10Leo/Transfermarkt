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
    public class PositionConverter : IPositionConverter
    {
        private ILogger logger;

        public PositionConverter() : this(null) { }

        public PositionConverter(ILogger logger)
        {
            this.logger = logger;
        }

        public PositionValue Convert(string stringToConvert)
        {
            Position? p = null;
            try
            {
                p = ConvertersConfig.GetPosition(stringToConvert);
            }
            catch (KeyNotFoundException ex)
            {
                logger.LogException(LogLevel.Error, $"The string {stringToConvert} wasn't found on the config file.", ex);
            }
            catch (ArgumentNullException ex)
            {
                logger.LogException(LogLevel.Error, $"Null argument string {stringToConvert} passed.", ex);
            }
            return new PositionValue { Value = p };
        }
    }
}
