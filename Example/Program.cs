
using Game.Application.Base.Mediator;
using Game.Application.Command.Models.Req.Sample;

var a = Mediator.Send<SampleReq, string>(new SampleReq());

Console.WriteLine(a);