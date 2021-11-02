using System;
using System.Collections.Generic;
using System.IO;

namespace CMSC312_Project
{
    class MainClass
    {

        public static void Main(string[] args)
        {
            
            string filename = "";

            //select a random number between 1 and 5 and make the corresponding process
            Random random = new Random();
            int z;

            
            Process process = new Process();

            //readylist list made
            List<Process> ReadyList = new List<Process>();

            
            //list for SJF made
            List<Process> SortedReadyList = new List<Process>();

            //Prompt user for what they would like
            Console.WriteLine("Please choose one of the following options:");
            Console.WriteLine("1: Template1");
            Console.WriteLine("2: Template2");
            Console.WriteLine("3: Template3");
            Console.WriteLine("4: Template4");
            Console.WriteLine("5: Template5");
            Console.WriteLine("6: 10 random processes");

            z = int.Parse(Console.ReadLine());

            //if user chose a template how many of said template
            if (z > 0 && z < 6)
            {
                Console.WriteLine("How many processes would you like:");
                int j = int.Parse(Console.ReadLine());

                for (int i = 0; i < j; i++)
                {
                    
                    if (z == 1)
                    {
                        filename = "Template1.txt";
                    }
                    else if (z == 2)
                    {
                        filename = "Template2.txt";
                    }
                    else if (z == 3)
                    {
                        filename = "Template3.txt";
                    }
                    else if (z == 4)
                    {
                        filename = "Template4.txt";
                    }
                    else if (z == 5)
                    {
                        filename = "Template5.txt";
                    }
                    

                    //make the process with the given filename
                    process = process.MakeP(filename, i);
                    Console.WriteLine("Process {0} is now: {1}\n", process.id, process.State);



                    //add process(es) to readylists
                    //Readylist will be used for the future Round Robin
                    ReadyList.Add(process);

                    //SortedReadyList will be used for the Shortest Job First
                    SortedReadyList.Add(process);

                }
            }
            else if (z == 6)
            {
                for (int i = 0; i < 10; i++)
                {
                    z = random.Next(1, 5);


                    if (z == 1)
                    {
                        filename = "Template1.txt";
                    }
                    else if (z == 2)
                    {
                        filename = "Template2.txt";
                    }
                    else if (z == 3)
                    {
                        filename = "Template3.txt";
                    }
                    else if (z == 4)
                    {
                        filename = "Template4.txt";
                    }
                    else if (z == 5)
                    {
                        filename = "Template5.txt";
                    }
                    else
                    {
                        Console.WriteLine("Number generator screwed up now the program will crash.");
                    }

                    //make the process with the given filename
                    process = process.MakeP(filename, i);
                    Console.WriteLine("Process {0} is now: {1}\n", process.id, process.State);



                    //add process(es) to readylists
                    //Readylist will be used for the future Round Robin
                    ReadyList.Add(process);

                    //SortedReadyList will be used for the Shortest Job First
                    SortedReadyList.Add(process);

                }
            }

            

            //sort the list by the total number of loops the process must run
            SortedReadyList.Sort((x, y) => x.totalLoops.CompareTo(y.totalLoops));

            foreach (var p in SortedReadyList)
            {
                Console.WriteLine("Process {0} Total Loops {1}", p.id, p.totalLoops);
            }

            Console.WriteLine("\n\nUsing Shortest Job First\n\n");
            SJF(SortedReadyList);

        }
        //method for running the processes in SJF way
        public static void SJF(List<Process> ReadyList)
        {
            List<Process> WaitingList = new List<Process>();
            Process pr = new Process();
            Process wpr = new Process();

            //run main loop while readylist is not empty
            while (ReadyList.Count != 0)
            {
                //get the first item and remove it from the readylist
                pr = ReadyList[0];
                ReadyList.RemoveAt(0);

                pr.State = "RUNNING";
                Console.WriteLine("Process {0} is now: {1}", pr.id, pr.State);

                //if the process has reached the end of its instructions terminate
                if (pr.location == pr.Steps.Count)
                {
                    pr.State = "TERMINATED";
                    Console.WriteLine("Process {0} is now: {1}", pr.id, pr.State);
                    Console.WriteLine("\n");
                }

                //while the counter doesnt excede the number of instructions
                while (pr.location < pr.Steps.Count)
                {


                    //if command is the starting point of critical section
                    if (pr.Steps[pr.location].Command == "CS_START")
                    {
                        //check to make sure none of the processes have inCS as true
                        Boolean goCS = true;
                        foreach (var p in ReadyList)
                        {
                            if (p.inCS)
                            {
                                goCS = false;
                                break;
                            }
                        }

                        if (goCS)
                        {
                            foreach (var p in WaitingList)
                            {
                                if (p.inCS)
                                {
                                    goCS = false;
                                    break;
                                }
                            }
                        }

                        if (goCS)
                        {
                            //if there is one with true return to ready list and break loop
                            pr.inCS = true;
                            Console.WriteLine("Process {0} Entering Critical Section.", pr.id);
                        }
                        else
                        {
                            Console.WriteLine("Process {0} Cannot enter critical section", pr.id);
                            pr.State = "READY";
                            Console.WriteLine("Process {0} is now: {1}", pr.id, pr.State);
                            ReadyList.Add(pr);
                            break;
                        }
                    }

                    //if command is the end point of critical section set false to inCS
                    else if (pr.Steps[pr.location].Command == "CS_END")
                    {
                        pr.inCS = false;
                        Console.WriteLine("Process {0} ending critical section.", pr.id);
                    }

                    //if CALCULATE run the loop iterating each necessary variables
                    else if (pr.Steps[pr.location].Command == "CALCULATE")
                    {
                        Console.WriteLine("Process {0} starting Calc Loop {1}.", pr.id, pr.Steps[pr.location].Loops);

                        //go through each loop
                        while (pr.Steps[pr.location].Loops > 0)
                        {
                            pr.Steps[pr.location].Loops--;
                            pr.totalLoops--;
                        }
                        Console.WriteLine("Process {0} ending Calc Loop.", pr.id);

                        //if end of instructions terminate
                        if (pr.location == (pr.Steps.Count - 1))
                        {
                            pr.State = "TERMINATED";
                            Console.WriteLine("Process {0} is now: {1}", pr.id, pr.State);
                            Console.WriteLine("\n");
                            break;
                        }


                    }

                    //else if I/O then wait loops
                    else if (pr.Steps[pr.location].Command == "I/O")
                    {
                        //state is now wiating
                        pr.State = "WAITING";

                        //process is added to the waiting list
                        WaitingList.Add(pr);
                        Console.WriteLine("Process {0} is now: {1}", pr.id, pr.State);

                        //start the waiting loop
                        Console.WriteLine("Process {0} starting I/O Loop {1}.", pr.id, pr.Steps[pr.location].Loops);
                        while (WaitingList[0].Steps[WaitingList[0].location].Loops > 0)
                        {
                            WaitingList[0].Steps[WaitingList[0].location].Loops--;
                            WaitingList[0].totalLoops--;
                        }
                        Console.WriteLine("Process {0} ending I/O Loop.", pr.id);

                        //process state is ready
                        WaitingList[0].State = "READY";
                        Console.WriteLine("Process {0} is now: {1}\n\n", WaitingList[0].id, WaitingList[0].State);

                        //add process to the ready list
                        ReadyList.Add(WaitingList[0]);

                        //remove process from the waiting list
                        WaitingList.RemoveAt(0);

                        //sort the readylist by totalLoops
                        ReadyList.Sort((x, y) => x.totalLoops.CompareTo(y.totalLoops));

                        pr.location++;

                        break;

                    }

                    pr.location++;
                    pr.State = "RUNNING";
                }




            }
        }


    }

    

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

    public class Process
    {
        public List<PStep> Steps { get; set; }
        public string State { get; set; }
        public bool inCS { get; set; }
        public int location { get; set; }
        public int id { get; set; }
        public int totalLoops { get; set; }

        public Process()
        {
            Steps = new List<PStep>();
            State = "NEW";
            inCS = false;
            location = 0;
            id = 0;
            totalLoops = 0;
        }

        public Process MakeP(string filename, int x)
        {
            
            Process process = new Process();
            process.id = x;
            Random random = new Random();

            Console.WriteLine("Process {0} is now: {1}", process.id, process.State);

            //make the path for the file
            String path = Path.Combine(Directory.GetCurrentDirectory(), filename);


            //loop through file
            foreach (string line in System.IO.File.ReadLines(path))
            {

                String[] data = line.Split(' ');
                int min = Int32.Parse(data[1]);
                int max = Int32.Parse(data[2]);
                int i = random.Next(min, max);
                process.totalLoops = process.totalLoops + i;



                //add data to the list as PStep
                process.Steps.Add(new PStep(data[0], i));
            }
            //end loop


            process.State = "READY";
            

            return process;
        }
    }
}
