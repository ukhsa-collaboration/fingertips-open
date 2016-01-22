using System;
using Ckan.DataTransformation;
using PholioVisualisation.PholioObjects;
using DIResolver;

namespace CkanConsoleApplication
{
    public interface IProgram
    {
        void UploadData();
        void WriteUploadComplete();
    }

    public class Program : IProgram
    {
        private DateTime start = DateTime.Now;

        private readonly IProfileUploader _profileUploader;

        public Program(IProfileUploader profileUploader)
        {
            _profileUploader = profileUploader;
        }

        public void UploadData()
        {
            const int profileId = ProfileIds.Phof;
            _profileUploader.UploadProfile(profileId);
        }

        static void Main(string[] args)
        {
            IoC.Register();
            var program = IoC.Container.GetInstance<IProgram>();

            try
            {
                program.UploadData();
            }
            catch (Exception ex)
            {
                Console.WriteLine("#EXCEPTION: " + ex.GetType().Name);
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }

            program.WriteUploadComplete();
        }

        public void WriteUploadComplete()
        {
            var timeTakenInMinutes = Math.Round((DateTime.Now - start).TotalMinutes, 1);
            Console.WriteLine("All data uploaded to CKAN in {0} mins", timeTakenInMinutes);
        }
    }
}
