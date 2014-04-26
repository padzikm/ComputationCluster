using System;
using UCCTaskSolver;

namespace DvrpUtils
{
    class DVRPTaskSolver : TaskSolver
    {
        public override string Name
        {
            get { throw new NotImplementedException(); }
        }

        public override event UnhandledExceptionEventHandler ErrorOccured;

        public override event TaskSolver.ComputationsFinishedEventHandler ProblemDividingFinished;

        public override event TaskSolver.ComputationsFinishedEventHandler ProblemSolvingFinished;

        public override event TaskSolver.ComputationsFinishedEventHandler SolutionsMergingFinished;

        public DVRPTaskSolver(byte[] problemData) : base(problemData) { }        

        public override byte[][] DivideProblem(int threadCount)
        {
            throw new NotImplementedException();
        }        

        public override void MergeSolution(byte[][] solutions)
        {
            throw new NotImplementedException();
        }        

        public override byte[] Solve(byte[] partialData, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }
    }
}
