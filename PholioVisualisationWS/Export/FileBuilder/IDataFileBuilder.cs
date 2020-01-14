namespace PholioVisualisation.Export.FileBuilder
{
    public interface IFileBuilderWriter
    {
        byte[] GetHeader();
        byte[] GetBody();
    }

    public interface IBuilderHeaderWriter
    {
        byte[] GetHeader();
    }

    public interface IBuilderBodyWriter
    {
        byte[] GetBody();
    }
}
