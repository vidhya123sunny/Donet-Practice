using System;

class Computer
{
    public string CPU { get; private set; }
    public string RAM { get; private set; }
    public string Storage { get; private set; }

    private Computer(Builder builder)
    {
        CPU = builder.CPU;
        RAM = builder.RAM;
        Storage = builder.Storage;
    }

    public void Display()
    {
        Console.WriteLine($"CPU: {CPU}");
        Console.WriteLine($"RAM: {RAM}");
        Console.WriteLine($"Storage: {Storage}");
    }

    public class Builder
    {
        public string CPU;
        public string RAM;
        public string Storage;

        public Builder SetCPU(string cpu)
        {
            CPU = cpu;
            return this;
        }

        public Builder SetRAM(string ram)
        {
            RAM = ram;
            return this;
        }

        public Builder SetStorage(string storage)
        {
            Storage = storage;
            return this;
        }

        public Computer Build()
        {
            return new Computer(this);
        }
    }
}

class Program
{
    static void Main()
    {
        Computer computer = new Computer.Builder()
                                .SetCPU("Intel i7")
                                .SetRAM("16GB")
                                .SetStorage("512GB SSD")
                                .Build();

        computer.Display();
    }
}