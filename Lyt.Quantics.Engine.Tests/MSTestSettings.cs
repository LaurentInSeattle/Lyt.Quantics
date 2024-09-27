#if DEBUG
[assembly: DoNotParallelize]
#else
[assembly: Parallelize(Scope = ExecutionScope.MethodLevel)]
#endif
