using System.Threading.Tasks;

namespace SiliconStudio.Paradox.Shaders.Compiler
{
    public struct TaskOrResult<T>
    {
        public readonly T Result;
        public readonly Task<T> Task;

        public static implicit operator TaskOrResult<T>(T result)
        {
            return new TaskOrResult<T>(result);
        }

        public static implicit operator TaskOrResult<T>(Task<T> task)
        {
            return new TaskOrResult<T>(task);
        }

        public TaskOrResult(Task<T> task)
        {
            Result = default(T);
            Task = task;
        }

        public TaskOrResult(T result)
        {
            Result = result;
            Task = null;
        }

        public T WaitForResult()
        {
            if (Task != null)
                return Task.Result;

            return Result;
        }

        public T GetCurrentResult()
        {
            if (Task != null)
                return Task.IsCompleted ? Task.Result : default(T);

            return Result;
        }
    }
}