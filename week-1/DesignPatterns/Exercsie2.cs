using System;

interface IDocument
{
    void Open();
}

class WordDocument : IDocument
{
    public void Open()
    {
        Console.WriteLine("Word Document Opened");
    }
}

class PdfDocument : IDocument
{
    public void Open()
    {
        Console.WriteLine("PDF Document Opened");
    }
}

class ExcelDocument : IDocument
{
    public void Open()
    {
        Console.WriteLine("Excel Document Opened");
    }
}

abstract class DocumentFactory
{
    public abstract IDocument CreateDocument();
}

class WordFactory : DocumentFactory
{
    public override IDocument CreateDocument()
    {
        return new WordDocument();
    }
}

class PdfFactory : DocumentFactory
{
    public override IDocument CreateDocument()
    {
        return new PdfDocument();
    }
}

class ExcelFactory : DocumentFactory
{
    public override IDocument CreateDocument()
    {
        return new ExcelDocument();
    }
}

class Program
{
    static void Main()
    {
        DocumentFactory factory;

        factory = new WordFactory();
        factory.CreateDocument().Open();

        factory = new PdfFactory();
        factory.CreateDocument().Open();

        factory = new ExcelFactory();
        factory.CreateDocument().Open();
    }
}