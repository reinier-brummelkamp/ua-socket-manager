# ua-socket-manager

Socket manager component for the Solitude framework.


## Server side files / setup

1. Add the following 

```c#
    app.UseWebSockets();
    app.UseUaSocketManager<MySocketMiddleware>("/socket", new SockJSOptions() { MaxResponseLength = 4096 });
```

to your `startup.cs` file in the `Configure` method *before*

```c#
    app.UseMvc(routes =>
    {
        routes.MapRoute(
            name: "default",
            template: "{controller=Home}/{action=Index}/{id?}");
    });
```

2. Copy the following files from the `server_side` folder to your prefered folder for serverside code 

| File                                      | Notes                                                                       |
| ----------------------------------------- | --------------------------------------------------------------------------- |
| \server_side\UaSocketManager.cs           | Socket Manager singleton, used to send messages and manage the client pool. |
| \server_side\UaSocketManagerMiddleware.cs | Middleware (Socket server), used to receive, parse and process messages.    |
| \server_side\MySocketMiddleware.cs        | Example of a middleware implementation.                                     |
