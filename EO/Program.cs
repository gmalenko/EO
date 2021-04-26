using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EO
{
    class Program
    {
        public static void Main(string[] args)
        {
            var csvLines = File.ReadAllLines(@"C:\Users\gmale\source\repos\EO\EO\WavelengthData.csv").Select(x => x.Split(Environment.NewLine));
            var partList = new List<Part>();
            var singleReadingDictionary = new Dictionary<double, List<Part>>();
            var rangeReadingDictionary = new Dictionary<double, List<Part>>();
            var waveLengthDictionary = new Dictionary<string, List<Part>>();


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
                        var tempList = entry.Split("-").Where(x => x.Trim() != "").OrderBy(x => Convert.ToDouble(x.Trim())).ToList();
                        var tempWaveLength = new Wavelength();
                        if (tempList.Count == 1)
                        {
                            tempWaveLength.SingleReading = true;
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
                                    tempWaveLength.End = 99999;
                                }
                            }

                        }
                        part.WavelengthList.Add(tempWaveLength);
                        // new stuff here
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
                partList.Add(part);
            }

            Console.ReadLine();
            //Console.WriteLine("Hello World!");
        }

    }
}
