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
            var csvLines = File.ReadAllLines(@"C:\Users\gmale\source\repos\EO\EO\WavelengthData.csv").Select(x => x.Split(Environment.NewLine));
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

            Console.WriteLine("Input a Wavelength");
            string input = Console.ReadLine();

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
