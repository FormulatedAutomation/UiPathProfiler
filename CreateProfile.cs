using System.Activities;
using System.ComponentModel;

namespace FormulatedAutomation.UiPathProfiler.Activities
{
    public class CreateProfile : CodeActivity
    {
        [Category("Input")]
        [RequiredArgument]
        public InArgument<string> OutputDir { get; set; }

        [Category("Output")]
        public OutArgument<string> ProcessCSVPath { get; set; }

        [Category("Output")]
        public OutArgument<string> SoftwareListCSVPath{ get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            string csvPath = OutputDir.Get(context);
            Profiler profiler = new Profiler(csvPath);
            Profiler.OutputFiles outputFiles = profiler.WriteProfile();
            ProcessCSVPath.Set(context, outputFiles.ProcessListPath);
            SoftwareListCSVPath.Set(context, outputFiles.SoftwareListPath);
        }

    }
}
