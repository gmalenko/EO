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
                        var tempList = entry.Split("-").OrderByDescending(x => x.Trim()).ToList();
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
                        string abdc = "";
                        //put parts inside of dictionary
                        //if single reading then put them in singleDictionary
                        if (tempWaveLength.SingleReading)
                        {

                        }
                        else
                        {

                        }



                    }
                }
                partList.Add(part);
            }

            Console.ReadLine();
            //Console.WriteLine("Hello World!");
        }

    }
}
