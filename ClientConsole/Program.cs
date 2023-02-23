using Grpc.Net.Client;
using MagicOnion.Client;
using TodoApi.Shared;
// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

var channel = GrpcChannel.ForAddress("https://localhost:5001");