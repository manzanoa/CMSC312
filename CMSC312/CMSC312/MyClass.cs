using System;
using System.IO;
namespace CMSC312
{
    public class PStep
    {
        public string Command { get; set; }
        public int Loops { get; set; }
        public PStep(string command, int loops)
        {
            Command = command;
            Loops = loops;
        }
        
    }
    public class MyClass
    {
        public MyClass()
        {
            string filename = "Template1.txt";
            List<PStep> Process = new List<PStep>();
            Random random = new Random();
            //read file
            StreamReader sr = new StreamReader(filename);
            String line = sr.ReadLine();


            //loop through file
            while (line != null)
            {
                
                String[] data = line.Split(' ');
                int min = Int64.Parse(data[1]);
                int max = Int64.Parse(data[2]);

                //add data to the list as PStep
                Process.Add(new PStep(data[0], random.Next(min, max));
                line = sr.ReadLine();
            }
            //end loop

            //run main loop
            foreach (var step in Process)
            {
                Console.WriteLine("{0}, {1}", step.Command, step.Loops)
            }
            //if calc print loop # left and calc
            //else if i/o print loop number and i/o
            //end

        }
    }
}
