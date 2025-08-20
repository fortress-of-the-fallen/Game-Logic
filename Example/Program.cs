
using Game.Application.Base.Mediator;
using Game.Application.Command.Models.Req.Sample;

var a = Mediator.Send<SampleReq, Task<string>>(new SampleReq());

Console.WriteLine(await a);