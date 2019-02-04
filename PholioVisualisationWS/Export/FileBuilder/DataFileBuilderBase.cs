using PholioVisualisation.Export.File;

namespace PholioVisualisation.Export.FileBuilder
{
   
    public class DataFileBuilderBase<T> : IDataFileBuilder<T>
    {
        // Dependencies        
        protected IFileBuilder<T> FileBuilder;
        protected IFileBuilderWriter<T> FileBuilderWriter;

        public DataFileBuilderBase(IFileBuilder<T> fileBuilder, IFileBuilderWriter<T> fileBuilderWriter)
        {
            FileBuilder = fileBuilder;
            FileBuilderWriter = fileBuilderWriter;
        }

        public T GetDataFile()
        {
            FileBuilder.AddContent(FileBuilderWriter.GetHeader());

            FileBuilder.AddContent(FileBuilderWriter.GetBody());

            return FileBuilder.GetFileContent();
        }
    }
}
