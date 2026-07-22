using System;

interface ICommand
{
    void Execute();
}

class Light
{
    public void TurnOn()
    {
        Console.WriteLine("Light is ON");
    }

    public void TurnOff()
    {
        Console.WriteLine("Light is OFF");
    }
}

class LightOnCommand : ICommand
{
    private Light light;

    public LightOnCommand(Light light)
    {
        this.light = light;
    }

    public void Execute()
    {
        light.TurnOn();
    }
}

class LightOffCommand : ICommand
{
    private Light light;

    public LightOffCommand(Light light)
    {
        this.light = light;
    }

    public void Execute()
    {
        light.TurnOff();
    }
}

class RemoteControl
{
    private ICommand command;

    public void SetCommand(ICommand command)
    {
        this.command = command;
    }

    public void PressButton()
    {
        command.Execute();
    }
}

class Program
{
    static void Main()
    {
        Light light = new Light();

        ICommand onCommand = new LightOnCommand(light);
        ICommand offCommand = new LightOffCommand(light);

        RemoteControl remote = new RemoteControl();

        remote.SetCommand(onCommand);
        remote.PressButton();

        remote.SetCommand(offCommand);
        remote.PressButton();
    }
}