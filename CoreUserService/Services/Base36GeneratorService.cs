using System;
using System.Text;
using CoreUserService.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace CoreUserService.Services
{
    /// <summary>
    /// IBase36GeneratorService implementation used to generate base 36 strings
    /// </summary>
    public class Base36GeneratorService : IBase36GeneratorService
    {
        //Base 36 character array
        private static readonly char[] Base36Chars ="0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

        /// <summary>
        /// ILogger property
        /// </summary>
        protected ILogger Logger { get; set; }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="logger">The ILogger implementation</param>
        public Base36GeneratorService(ILogger<Base36GeneratorService> logger)
        {
            //Set logger to injected instance
            Logger = logger;
        }

        /// <summary>
        /// Generates a 7 character, base 36 string
        /// </summary>
        /// <returns>7 character, base 36 string</returns>
        public string Generate()
        {
            var random = new Random();
            var sb = new StringBuilder(7);
            for (var i = 0; i < 7; i++)
            {
                sb.Append(Base36Chars[random.Next(36)]);
            }           
            return sb.ToString();
        }
    }
}
