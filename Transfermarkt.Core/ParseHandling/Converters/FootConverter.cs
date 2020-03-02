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
            //TODO: remove try catch inside IConverters? exceptions are being handled in the class that calls this function

            //Foot? p = null;

            //try
            //{
            //    p = ConvertersConfig.GetFoot(stringToConvert);
            //}
            //catch (KeyNotFoundException ex)
            //{
            //    logger.LogException(LogLevel.Error, $"The string {stringToConvert} wasn't found on the config file.", ex);
            //}
            //catch (ArgumentNullException ex)
            //{
            //    logger.LogException(LogLevel.Error, $"Null argument string {stringToConvert} passed.", ex);
            //}

            return new FootValue { Value = ConvertersConfig.GetFoot(stringToConvert) };
        }
    }
}
