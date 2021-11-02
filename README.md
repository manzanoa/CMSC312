# CMSC312

You should be able to run the file by just typing in the name of the executable.

If you need to change anything in the file just make sure you compile it either in the IDE or by using

  csc Program.cs
  
on the command line.

Make sure your using and IDE that runs C#.


This program takes the following as input:

-a number int(z) that represents the user's choice of what template to run
-a number int(j) that represents how many processes of said template

NOTE: The program only asks this once.

If the user wishes to have a random set of processes from varying templates they may choose 6 and the process will randomly make 10 processes.

This program uses the following classes:

MainClass: holds the main loop and the scheduling method
      -public static void SJF(List<Process> ReadyList): takes the already sorted list and runs it with the Shortest Job First scheduling
            -if a process is being added to the readylist (most likely after the I/O loop) it will sort by totalLoops (putting the newly inserted Process from I/O to the front of the list)
  


Process:  meant to simulate a process, the program will create processes to run through the loops until they are terminated.
       -public List<PStep> Steps: the list of instructions the process has to complete
       -public string State: the state of the process
       -public bool inCS: a boolean that determines if the process is currently under critical section
       -public int location: acts as a pointer to see what the next instruction is
       -public int id: the process id
       -public int totalLoops: the total number of loops this process has to run
       -Methods:
          -Public Process MakeP(String filename, int x): Makes the individual process based on the file from filename and assigns an id to the process (x).
  
  

  PStep: houses each instruction for the process
      -Command: Instruction type (CALCULATE, I/O, or FORK(in the future))
      -Loops: the number of loops needed to run through for this instruction
