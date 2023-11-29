using RFIDRead;
using RFIDRead.Services;
using RFIDRead.Services.Interfaz;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
       // services.AddTransient<IReadCom, ReadCom>();
        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();
