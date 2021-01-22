using System.Activities;
using System.ComponentModel;

namespace FormulatedAutomation.UiPathProfiler.Activities
{
    public class CreateProfile : CodeActivity
    {
        [Category("Input")]
        [RequiredArgument]
        public InArgument<string> OutputDir { get; set; }
        protected override void Execute(CodeActivityContext context)
        {
            string csvPath = OutputDir.Get(context);
            Profiler profiler = new Profiler(csvPath);
            profiler.WriteProfile();
        }

    }
}
