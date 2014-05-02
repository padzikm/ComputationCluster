path=C:\Users\Kamil\Documents\GitHub\ComputationCluster\src\ComputationalCluster

start %path%\CommunicationServer\bin\Debug\CommunicationServer.exe 
choice /T 10 /C X /D X /N
start %path%\ComputationalNode\bin\Debug\ComputationalNode.exe
start %path%\TaskManager\bin\Debug\TaskManager.exe
start %path%\ComputationalClient\bin\Debug\ComputationalClient.exe