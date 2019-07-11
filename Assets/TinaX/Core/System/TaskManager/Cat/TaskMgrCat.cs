using TinaX.Task;
using CatLib;


namespace TinaX
{
    public class XTask : Facade<ITaskManager>
    {
        public static ITaskManager I
        {
            get
            {
                return Instance;
            }
        }
    }
}

namespace TinaX.Cat
{
    public class TaskProvider : IServiceProvider
    {
        public void Init()
        {
            
        }

        public void Register()
        {
            App.Singleton<ITaskManager, TaskManager>();
        }
    }
}