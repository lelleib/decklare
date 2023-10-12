using DbgLib;

IDbgRuntime runtime = new DominionDbgSample.Implemented.TestDbgRuntime(); // DLL written by hand
DbgEnvironmentBase env = new DominionDbgSample.Generated.DbgEnvironment(runtime); // DLL compiled from Dbg description
env.Run();
