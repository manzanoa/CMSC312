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

If the user wishes to have a random set of processes from varying templates they may choose 6 and the process will randomly make j processes.

The program will output the steps each process has taken:
	-for example: if process 1 is newly created the program will print
		-”Process 1 is now: NEW”
	-if process 2 is placed in the ready queue
		-”Process 2 is now: READY”
	-if running CALCULATE loops
		-”Process (x) running calc loop (number of loops)
		-same for I/O loops
	-if process is in waiting list with WAITING state
		-”Process (x) is now: WAITING”
	-if process is out of ready queue and in running state
		-”Process (x) is now: RUNNING”
	-if process is terminating
		-”Process (x) is now: TERMINATE”

The program will also output and show that the processes are in order for SJF before running the scheduler.


This program uses the following classes:

MainClass: holds the main loop and the scheduling method
      -public static void SJF(List<Process> ReadyList): takes the already sorted list and runs it with the Shortest Job First scheduling
            -if a process is being added to the readylist (most likely after the I/O loop) it will sort by totalLoops (putting the newly inserted Process from I/O to the front of the list)
	
      -public static bool calculate(Process pr, List<Process> waitingList, List<Process> readyList): takes the process(pr) and loops through
		-at each iteration a random number will be generated and if below 10 the process will be interrupted and placed in the waitlist
			-will also return false if interrupted
		-returns true if the process runs through the entire loop without interrupting
	
      -public static void SJFThread(Process pr, List<Process> ReadyList, List<Process> NewList, List<Process> WaitingList, List<Process> FullList, int countId, MemoryManager m, Semaphore semaphore): the method the thread will run concurrently
	   -This is in charge of doing the following:
		-If the current command is CALCULATE
			-run the calculate loop
		-If the current command is I/O
			-send the process to the Waitlist and change state to WAITING
		-If the current command is FORK
			-create a child process and determine if child will enter new or ready list
			-send parent process to waitlist
		-If the current command is CS_START
			-start the critical section
			-I suspect the semaphores i tried to use are responsible for the threads staying stuck and not continuing
		-If the current command is CS_END
			-end the critical section
	
	-public static void RoundRobin(List<Process> ReadyList, List<Process> NewList, List<Process> WaitingList, int countId, MemoryManager m)
		-It should do the same as SJF method except without sorted lists or needing to find the shortest job first
	-public static void RRThread(Process pr, List<Process> ReadyList, List<Process> NewList, List<Process> WaitingList, int countId, MemoryManager m, Semaphore s )
		-Does the same as SJFThread and serves the same purpose but in Round Robin.
	
	-public static void terminate(Process pr, MemoryManager m, List<Process> ReadyList, List<Process> WaitingList, List<Process> NewList )
		-recursively terminates the process and its children for cascading termination
        
  


Process:  meant to simulate a process, the program will create processes to run through the loops until they are terminated.
       -public List<PStep> Steps: the list of instructions the process has to complete
       -public string State: the state of the process
       -public bool inCS: a boolean that determines if the process is currently under critical section
       -public int location: acts as a pointer to see what the next instruction is
       -public int id: the process id
       -public int memReq: the memory it needs to run
       -public int totalLoops: the total number of loops this process has to run
       -public int priority: the higher the number the closer it should maybe be to the front
       -Methods:
          -Public Process MakeP(String filename, int x): Makes the individual process based on the file from filename and assigns an id to the process (x).
  
  

  PStep: houses each instruction for the process
      -Command: Instruction type (CALCULATE, I/O, or FORK(in the future))
      -Loops: the number of loops needed to run through for this instruction
	
  MemoryManager: keeps track of the memory and checks if the processes can fit in
	-memMax: the maximum number of memory allowed
	-memTotal: the current number of memory
	-Methods:
	   -Public bool canAddMem(int memReq): checks if the process can be added based on the memory required for the process and updates accordingly
