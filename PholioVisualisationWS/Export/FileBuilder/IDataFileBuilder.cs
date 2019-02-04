namespace PholioVisualisation.Export.FileBuilder
{
    public interface IDataFileBuilder<T>
    {
        T GetDataFile();
    }

    public interface IFileBuilderWriter<T>
    {
        T GetHeader();
        T GetBody();
    }

    public interface IBuilderHeaderWriter<T>
    {
        T GetHeader();
    }

    public interface IBuilderBodyWriter<T>
    {
        T GetBody();
    }
}
