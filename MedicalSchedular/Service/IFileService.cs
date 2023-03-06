namespace MedicalScheduler.Service
{
    public interface IFileService
    {
        void DeleteFile(string imageUrl);
        Task<string> UploadFile(IFormFile file);
    }
}
