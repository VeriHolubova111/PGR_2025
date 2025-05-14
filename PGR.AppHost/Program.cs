var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.PGR_ApiService>("apiservice");

builder.AddProject<Projects.PGR_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
