using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace CMSC312_Project
{
    class MainClass
    {

        public static void Main(string[] args)
        {
            
            string filename = "";

            
            Random random = new Random();
            int z;
            int countId = 0;

            
            Process process = new Process();

            MemoryManager m = new MemoryManager();

            //list of processes for each of the main states
            List<Process> NewList = new List<Process>();
            List<Process> ReadyList = new List<Process>();
            List<Process> WaitingList = new List<Process>();

            //the entire list of processes
            List<Process> FullList = new List<Process>();

            //list for SJF scheduler made
            List<Process> SortedReadyList = new List<Process>();

            //Prompt user for what they would like
            Console.WriteLine("Please choose one of the following options:");
            Console.WriteLine("1: Template1");
            Console.WriteLine("2: Template2");
            Console.WriteLine("3: Template3");
            Console.WriteLine("4: Template4");
            Console.WriteLine("5: Template5");
            Console.WriteLine("6: A number of random processes");

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


                    //make the process with the given filename and add to the list
                    process = process.MakeP(filename, i);
                    countId = i;

                    NewList.Add(process);
                    FullList.Add(process);

                    
                }
            }

            //how many random templates
            else if (z == 6)
            {
                Console.WriteLine("How many processes would you like:");
                int j = int.Parse(Console.ReadLine());
                for (int i = 0; i < j; i++)
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
                    countId = i;

                    NewList.Add(process);
                    FullList.Add(process);

                }
            }

            //goes through and see which process can be fit in
            foreach (var p in NewList)
            {
                if (m.canAddMem(p.memReq))
                {
                    p.State = "READY";
                    ReadyList.Add(p);
                    SortedReadyList.Add(p);
                    m.memTotal = m.memTotal + p.memReq;

                    Console.WriteLine("Process {0} was added to ready list.", p.id);
                }

                else
                {
                    Console.WriteLine("Process {0} cannot be added due to insufficient memory.", p.id);
                }
            }

            //sort the list by the total number of loops the process must run
            SortedReadyList.Sort((x, y) => x.totalLoops.CompareTo(y.totalLoops));

            //updates new list
            foreach (var p in SortedReadyList)
            {
                Console.WriteLine("Process {0} Total Loops {1}", p.id, p.totalLoops);
                NewList.Remove(p);
            }

            //run scheduler
            SJF(ReadyList, NewList, WaitingList, FullList, countId + 1, m);

            if (SortedReadyList.Count == 0 && NewList.Count == 0 && WaitingList.Count == 0)
            {
                Console.WriteLine("All processors processes");
            }

        }

        //***************************************************************************
        //calculate
        //input: process, waiting list, and ready list
        //output: boolean value for if the process finished calculating without interruption
        //****************************************************************************
        //
        //At each loop a number will be randomly generated and if below 10 then interrrupt
        //otherwise continue looping until done or interrupted.
        //***************************************************************************
        public static bool calculate(Process pr, List<Process> waitingList, List<Process> readyList)
        {
            Random random = new Random();

            Console.WriteLine("Process {0} starting Calc Loop. {1} left.", pr.id, pr.Steps[pr.location].Loops);

            //go through each loop
            while (pr.Steps[pr.location].Loops > 0)
            {
                int randNum = random.Next(100);
                if (randNum < 10)
                {
                    //state is now waiting
                    pr.State = "WAITING";
                    Console.WriteLine("\n\nInterrupt Process {0} Calc Loop. {1} left...\n\n", pr.id, pr.Steps[pr.location].Loops);

                    //add a step for the interrupt
                    pr.Steps.Insert(pr.location, new PStep("I/O", 10));
                    pr.totalLoops = pr.totalLoops + 10;

                    //process is added to the waiting list
                    waitingList.Add(pr);
                    waitingList.Sort((x, y) => x.totalLoops.CompareTo(y.totalLoops));
                    Console.WriteLine("Process {0} is now: {1}", pr.id, pr.State);
                    return false;


                }
                else
                {
                    pr.Steps[pr.location].Loops--;
                    pr.totalLoops--;
                }

            }
            Console.WriteLine("Process {0} ending Calc Loop.", pr.id);
            return true;

            
                

            
        }
        //*******************************************************************************
        //SJFThread
        //input: Process pr, List<Process> ReadyList, List<Process> NewList, List<Process> WaitingList, List<Process> FullList, int countId, MemoryManager m, Semaphore semaphore
        //output: void
        //*******************************************************************************
        //The method that the thread will run each time with a processor.
        //Depending on the command the processor is currently at it will run accordingly.
        //CALCULATE - loop through the loops unless interrupted
        //I/O - send process to wait list
        //FORK - creates a child process and sends parent to wait list
        //CS_START - updates the semaphore to be in critical section
        //CS_END - updates the semaphore to end critical section

        //If the process is at the end of its steps then terminate.
        //*******************************************************************************


        public static void SJFThread(Process pr, List<Process> ReadyList, List<Process> NewList, List<Process> WaitingList, List<Process> FullList, int countId, MemoryManager m, Semaphore semaphore)
        {
            
            if (pr.location == pr.Steps.Count)
            {
                pr.State = "TERMINATED";
                Console.WriteLine("Process {0} is now: {1}\n", pr.id, pr.State);
                m.memTotal = m.memTotal - pr.memReq;

                if (pr.parent != null)
                {
                    pr.parent.children.Remove(pr);
                    Console.WriteLine("Child process {0} of parent process {1} has been {2} on its own.", pr.id, pr.parent.id, pr.State);

                }

                if (pr.children.Count > 0)
                {
                    foreach (var child in pr.children)
                    {
                        ReadyList.Remove(child);
                        WaitingList.Remove(child);
                        NewList.Remove(child);
                        child.State = "TERMINATED";
                        Console.WriteLine("Child process {0} is now: {1}\n", child.id, child.State);
                        m.memTotal = m.memTotal - child.memReq;
                    }
                }
            }

            //while the counter doesnt excede the number of instructions
            while (pr.location < pr.Steps.Count)
            {


                //if command is the starting point of critical section
                if (pr.Steps[pr.location].Command == "CS_START")
                {
                    
                    //check to make sure none of the processes have inCS as true
                    if (semaphore.WaitOne(TimeSpan.FromSeconds(4)))
                    {
                        bool goCS = true;
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
                            Console.WriteLine("Process {0} is now: {1}\n\n", pr.id, pr.State);
                            ReadyList.Add(pr);
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                        
                    
                }

                //if command is the end point of critical section set false to inCS
                else if (pr.Steps[pr.location].Command == "CS_END")
                {
                    semaphore.Release();
                    pr.inCS = false;
                    Console.WriteLine("Process {0} ending critical section.", pr.id);
                }

                //if CALCULATE run the loop iterating each necessary variables
                else if (pr.Steps[pr.location].Command == "CALCULATE")
                {
                    bool calculated = calculate(pr, WaitingList, ReadyList);
                    if (!calculated)
                    {
                        break;
                    }

                    //if end of instructions terminate
                    if (pr.location == (pr.Steps.Count - 1))
                    {
                        pr.State = "TERMINATED";
                        Console.WriteLine("Process {0} is now: {1}\n", pr.id, pr.State);
                        m.memTotal = m.memTotal - pr.memReq;

                        if (pr.parent != null)
                        {
                            pr.parent.children.Remove(pr);
                            Console.WriteLine("Child process {0} of parent process {1} has been {2} on its own.", pr.id, pr.parent.id, pr.State);

                        }

                        if (pr.children.Count > 0)
                        {
                            foreach (var child in pr.children)
                            {
                                ReadyList.Remove(child);
                                WaitingList.Remove(child);
                                NewList.Remove(child);
                                child.State = "TERMINATED";
                                Console.WriteLine("Child process {0} is now: {1}\n", child.id, child.State);
                                m.memTotal = m.memTotal - child.memReq;
                            }
                        }

                        break;
                    }


                }

                //else if I/O then send to waitlist
                else if (pr.Steps[pr.location].Command == "I/O")
                {
                    //state is now waiting
                    pr.State = "WAITING";

                    //process is added to the waiting list
                    WaitingList.Add(pr);
                    Console.WriteLine("Process {0} is now: {1}", pr.id, pr.State);
                    WaitingList.Sort((x, y) => x.totalLoops.CompareTo(y.totalLoops));



                    break;

                }
                else if (pr.Steps[pr.location].Command == "FORK")
                {
                    fork(pr, countId, ReadyList, WaitingList, NewList, FullList, m);
                }

                pr.location++;
                pr.State = "RUNNING";


            }
        }
        //****************************************************************************************
        //SJF
        //input: ReadyList, NewList, WaitingList, FullList, int countId, MemoryManager m
        //output: none
        //****************************************************************************************
        //This is the scheduler method for running the processes in Shortest Job First way.
        //Constantly updates the ready list to be in order.
        //Constantly runs while the threads are active or the ready, new, or wait lists have processes
        //Runs the threads with the processes in order.
        //
        //However, I was unable to finish and at its current state is unstable with multiple bugs and errors that it has that I could not fix.
        //****************************************************************************************
        public static void SJF(List<Process> ReadyList, List<Process> NewList, List<Process> WaitingList, List<Process> FullList, int countId, MemoryManager m)
        {
            Process pr0 = new Process();
            Process pr1 = new Process();
            Random random = new Random();
            Semaphore semaphore = new Semaphore(1, 1);

            
            Thread thr1 = new Thread(() => SJFThread(pr1, ReadyList, NewList, WaitingList, FullList, countId, m, semaphore));


            //run main loop while readylist waitinglist and new list is not empty and while the thread is alive
            while (ReadyList.Count != 0 || WaitingList.Count != 0 || NewList.Count != 0 || thr1.IsAlive)
            {
                //updates and adds any processes that can be added to memory
                if (NewList.Count > 0)
                {
                    List<Process> removeList = new List<Process>();
                    foreach (var p in NewList)
                    {
                        if (m.canAddMem(p.memReq))
                        {
                            p.State = "READY";
                            Console.WriteLine("Process {0} is now {1}", p.id, p.State);
                            ReadyList.Add(p);
                            removeList.Add(p);
                        }
                    }
                    foreach (var r in removeList)
                    {
                        NewList.Remove(r);
                    }
                }

                //if thread is not active and there are processes in the readylist
                ////take the process out of the list and run it through the thread
                if (ReadyList.Count > 0 && !thr1.IsAlive)
                {
                    //get the first item and remove it from the readylist
                    pr0 = ReadyList[0];
                    ReadyList.Remove(pr0);

                    pr0.State = "RUNNING";
                    Console.WriteLine("Process {0} is now: {1}", pr0.id, pr0.State);
                    if(!thr1.IsAlive)
                    {
                        thr1 = new Thread(() => SJFThread(pr0, ReadyList, NewList, WaitingList, FullList, countId, m, semaphore));
                        thr1.Start();
                    }
                    
                }
                //while the threads are running if the waiting list is not empty
                //     run through the i/o loops and place back into ready list when done
                else if (WaitingList.Count > 0)
                {
                    pr0 = WaitingList[0];
                    Console.WriteLine(0);

                    //weird error where pr0 would become null randomly placed and if statement so it would not crash
                    if(pr0 != null)
                    {
                        while (pr0.Steps[pr0.location].Loops > 0)
                        {
                            pr0.Steps[pr0.location].Loops--;
                            pr0.totalLoops--;
                        }

                        WaitingList.RemoveAt(0);

                        pr0.State = "READY";
                        pr0.location++;
                        ReadyList.Add(pr0);
                        ReadyList.Sort((x, y) => x.totalLoops.CompareTo(y.totalLoops));

                        Console.WriteLine("Process {0} ended I/O loop", pr0.id);
                        Console.WriteLine("Process {0} is now {1}", pr0.id, pr0.State);
                    }
                    
                }


                //prints statement to show that all the processes are done running 
                if (ReadyList.Count == 0 && NewList.Count == 0 && WaitingList.Count == 0 && !thr1.IsAlive)
                {
                    Console.WriteLine("All processors processes");
                }



            }
        }

        //****************************************************************************************************
        //fork
        //input: pr, countId, ReadyList, WaitList, NewList, FullList, MemoryManager m
        //output: none
        //****************************************************************************************************
        //Makes a new child process based on a previously made template
        //If the child can fit add to the ready list otherwise add to the new list.
        //Add the parent to the waitlist.
        //****************************************************************************************************

        public static void fork(Process pr, int countId, List<Process> ReadyList, List<Process> WaitList, List<Process> NewList, List<Process> FullList, MemoryManager m)
        {

            //Make Child Process
            Console.WriteLine("Fork Child");
            Process newPr = new Process();
            countId++;

            newPr = newPr.MakeP("ChildTemplate.txt", countId);
            

            newPr.parent = pr;

            pr.children.Add(newPr);

            //place child process in readylist if memory allows
            if (m.canAddMem(newPr.memReq))
            {
                newPr.State = "READY";
                Console.WriteLine("Process {0} is now {1}.\n", newPr.id, newPr.State);

                ReadyList.Add(newPr);
                FullList.Add(newPr);
            }
            //else place in the new list
            else
            {
                NewList.Add(newPr);
                FullList.Add(newPr);
            }

            //adds a step and sends the process to the waitlist
            pr.Steps.Insert(pr.location+1, new PStep("I/O", 100));
            pr.totalLoops = pr.totalLoops + 10;

        }


    }

    //*************************************************************************************************
    //Object MemeoryManager
    //  -int memMax - the maximum number of memory
    //  -int memTotal - the current amount of memory
    //*************************************************************************************************
    //This class keeps track of the memory and allows memory to be used
    //*************************************************************************************************
    public class MemoryManager
    {
        public int memMax { get; }
        public int memTotal { get; set; }

        public MemoryManager()
        {
            memMax = 100;
            memTotal = 0;
        }

        //if you can add the memory add it and return true
        public bool canAddMem(int memReq)
        {
            if (memTotal + memReq <= memMax)
            {
                memTotal = memTotal + memReq;
                return true;
            }
                
            else
                return false;
        }


    }

    //Class that holds the individual step of the process
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

    //*********************************************************************************
    //Process
    //  -List<PSteps> steps - list of commands the process must run
    //  -string State - the current state of the process
    //  -bool inCS - boolean value that says if the current process is in critical section
    //  -int location - a pointer that points to the current pStep in the list
    //  -int id - the process id
    //  -int totalLoops - the total number of loops this process must go through (for the shortest job first scheduler)
    //  -int memReq - the memory required to run this process
    //  -int Process parent - holds the reference to the parent processor
    //  -List<Process> children - hold the list of references to the children
    //**********************************************************************************
    //  Methods
    //      -MakeP - makes the process based on the the file and its id will be passed down
    //**********************************************************************************
    //


    public class Process
    {
        public List<PStep> Steps { get; set; }
        public string State { get; set; }
        public bool inCS { get; set; }
        public int location { get; set; }
        public int id { get; set; }
        public int totalLoops { get; set; }
        public int memReq { get; set; }
        public Process parent { get; set; }
        public List<Process> children { get; set; }

        public Process()
        {
            Steps = new List<PStep>();
            State = "NEW";
            inCS = false;
            location = 0;
            id = 0;
            totalLoops = 0;
            memReq = 0;
            parent = null;
            children = new List<Process>();
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

            process.memReq = random.Next(25, 50);
            

            return process;
        }

    }
}
