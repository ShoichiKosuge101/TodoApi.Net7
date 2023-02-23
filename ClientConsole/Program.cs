using Grpc.Core;
using Grpc.Net.Client;
using MagicOnion.Client;
using Shared.Model.Services;
// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

// for not .NET Standard2.1
// var channel = new Channel("localhost", 5001, new SslCredentials());

var channel = GrpcChannel.ForAddress("https://localhost:5001");

// Client Impl
// create proxy to call the server transparently
var client = MagicOnionClient.Create<IMyFirstService>(channel);
// call the server side method using proxy
var result = await client.SumAsync(123, 456);
Console.WriteLine($"Result: {result}");