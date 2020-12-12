using System.IO;
using System.Net;
using System.Text;

namespace AdventOfCode
{
    /// <summary>
    /// Input fetching helper class
    /// </summary>
    public static class InputFetcher
    {
        #region Constants
        /// <summary>
        /// Session cookie file
        /// </summary>
        private const string COOKIE = @"Input\cookie.txt";
        /// <summary>
        /// Base Advent of Code URL
        /// </summary>
        private const string BASE_URL = "https://adventofcode.com";
        #endregion
        
        #region Static methods
        /// <summary>
        /// Gets the associated input file, or fetches it from the AoC website if needed
        /// </summary>
        /// <param name="year">Event year</param>
        /// <param name="day">Problem day</param>
        /// <returns>The Input file for the problem</returns>
        public static string EnsureInput(int year, int day)
        {
            //Check for the input file
            FileInfo inputFile = new($"Input/{year}/day{day:D2}.txt");
            if (!inputFile.Exists)
            {
                //Make sure the directory exists
                if (!inputFile.Directory?.Exists ?? false)
                {
                    inputFile.Directory!.Create();
                }
                
                //Get input and write to file
                string input = GetInput(year, day);
                using StreamWriter writer = inputFile.CreateText();
                writer.Write(input);
                #if DEBUG
                //Additionally write to project if in debug
                writer.Flush();
                inputFile.CopyTo(@$"..\..\..\Input\{year}\{inputFile.Name}", true);
                #endif

                //Return the fetched input
                return input;
            }

            using StreamReader reader = inputFile.OpenText();
            return reader.ReadToEnd();
        }

        /// <summary>
        /// Fetches the input from the AoC website
        /// </summary>
        /// <param name="year">Event year</param>
        /// <param name="day">Problem day</param>
        /// <returns>The input for the problem</returns>
        private static string GetInput(int year, int day)
        {
            //Create request
            HttpWebRequest request = WebRequest.CreateHttp($"{BASE_URL}/{year}/day/{day}/input");
            request.Method = "GET";
            request.Headers.Add("cookie", $"session={File.ReadAllText(COOKIE)}");
            //Get response
            using HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using Stream responseStream = response.GetResponseStream();
            using StreamReader responseReader = new(responseStream, Encoding.UTF8);
            return responseReader.ReadToEnd().Trim();
        }
        #endregion
    }
}
