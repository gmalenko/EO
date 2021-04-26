using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace EO
{
    class Program
    {
        public static void Main(string[] args)
        {
            /*  Here is what the application is doing
             *  1. Looping though CSV file
             *  2. Putting the contents of the file inside of the waveLengthDictionary with the wave length as the key and a list of parts as values
             *      Why?  Instead of looping though each indivdual wavelength and have n number of calculations we are cutting down on the amount of calculations because there are quite of few duplicates in the sample data.
             *  3. Asking the user to input the wave length they want to search for
             *  4. Loop though the waveLengthDictionary.  Convert the keys into a proper start and end values, and perform calculations. If there is a match, copy those matching entries to the resultPart list
             *  5. Loop though resultPartList and print results
             * 
             *   Changes that could help out some bugs and readability
             *   1. Check the input from the user can be converted from string to double
             *   2. Create a function for lines 57-87 and 131-142.  Essentially its doing the same thing
             * 
             */

            var csvLines = File.ReadAllLines(Directory.GetCurrentDirectory() + @"\WavelengthData.csv").Select(x => x.Split(Environment.NewLine));
            var waveLengthDictionary = new Dictionary<string, List<Part>>();
            var resultPartList = new List<Part>();


            //traversing csv
            foreach (var line in csvLines)
            {
                var part = new Part();
                var entryList = line[0].Split(",").ToList();
                foreach (var entry in entryList)
                {
                    if (part.Id == Guid.Empty)
                    {
                        part.Id = Guid.NewGuid();
                        part.Name = entry;
                    }
                    else
                    {
                        var tempWaveLength = new Wavelength();
                        var tempList = new List<string>();
                        if (!entry.Contains("-"))
                        {
                            tempList.Add(entry);
                            tempWaveLength.SingleReading = true;
                        }
                        else
                        {
                            tempList = entry.Split("-").Where(x => x.Trim() != "").OrderBy(x => Convert.ToDouble(x.Trim())).ToList();
                        }
                        foreach (var temp in tempList)
                        {
                            if (tempWaveLength.Start == 0)
                            {
                                if (temp != "")
                                {
                                    tempWaveLength.Start = Convert.ToDouble(temp);
                                }
                                else
                                {
                                    tempWaveLength.Start = 0;
                                }
                            }
                            else
                            {
                                if (temp != "")
                                {
                                    tempWaveLength.End = Convert.ToDouble(temp);
                                }
                                else
                                {
                                    tempWaveLength.End = int.MaxValue;
                                }
                            }
                        }


                        if (tempWaveLength.End == 0 && tempWaveLength.SingleReading == false)
                        {
                            tempWaveLength.End = int.MaxValue;
                        }

                        part.WavelengthList.Add(tempWaveLength);
                        //dictionary things are here
                        var tempPartList = new List<Part>();
                        var key = "";
                        if (!tempWaveLength.SingleReading)
                        {
                            key = tempWaveLength.Start.ToString() + "-" + tempWaveLength.End.ToString();
                        }
                        else
                        {
                            key = tempWaveLength.Start.ToString();
                        }
                        if (waveLengthDictionary.TryGetValue(key, out tempPartList))
                        {
                            tempPartList.Add(part);
                        }
                        else
                        {
                            tempPartList = new List<Part>();
                            tempPartList.Add(part);
                            waveLengthDictionary.Add(key, tempPartList);
                        }
                        //end dictionary stuff here

                    }
                }
            }

            //Console actions
            Console.WriteLine("Input a Wavelength");
            string input = Console.ReadLine();
            // end of console actions


            var inputDouble = Convert.ToDouble(input);




            //now loop though wavelength dicionary and find out if its between the values
            foreach (var tempWaveLengthEntry in waveLengthDictionary)
            {
                var waveLengthkey = tempWaveLengthEntry.Key.Split("-");
                var waveLengthParam = new Wavelength();
                foreach (var temp in waveLengthkey)
                {
                    if (waveLengthParam.Start == 0)
                    {
                        waveLengthParam.Start = Convert.ToDouble(temp);
                    }
                    else
                    {
                        waveLengthParam.End = Convert.ToDouble(temp);
                    }
                }
                if (waveLengthParam.End == 0)
                {
                    waveLengthParam.SingleReading = true;
                }
                if (inputDouble >= waveLengthParam.Start && inputDouble <= waveLengthParam.End && waveLengthParam.SingleReading == false)
                {
                    resultPartList.AddRange(tempWaveLengthEntry.Value);
                }
                else if (waveLengthParam.SingleReading == true && inputDouble == waveLengthParam.Start)
                {
                    resultPartList.AddRange(tempWaveLengthEntry.Value);
                }
                else
                {
                    //no matches
                }
            }

            //now loop though result list and print out results
            var sb = new StringBuilder();
            if (resultPartList.Count > 0)
            {
                sb.AppendLine("Part        Wavelength");
                foreach (var result in resultPartList)
                {
                    if (result.WavelengthList[0].End == int.MaxValue) //just putting this back to the original format
                    {
                        sb.AppendLine($"{result.Name}     {result.WavelengthList[0].Start} - ");
                    }
                    else if (result.WavelengthList[0].SingleReading)
                    {
                        sb.AppendLine($"{result.Name}     {result.WavelengthList[0].Start}");
                    }
                    else if (!result.WavelengthList[0].SingleReading)
                    {
                        sb.AppendLine($"{result.Name}     {result.WavelengthList[0].Start} - {result.WavelengthList[0].End}");
                    }
                }
                Console.WriteLine(sb.ToString());
                Console.WriteLine($"Total Matches: {resultPartList.Count}");
            }
            else
            {
                Console.WriteLine("No Results Found");
            }

            Console.WriteLine("Press Enter to End");
            Console.ReadLine();
        }

    }
}
