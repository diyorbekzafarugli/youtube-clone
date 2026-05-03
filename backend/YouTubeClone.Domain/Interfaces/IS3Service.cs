namespace YouTubeClone.Domain.Interfaces;

public interface IS3Service
{
    // Presigned URL — frontend to'g'ridan-to'g'ri R2 ga yuklaydi
    Task<string> GeneratePresignedUploadUrlAsync(
        string fileName,
        string contentType,
        bool isVideo,
        CancellationToken cancellationToken = default);

    // Multipart upload — katta fayllar uchun
    Task<string> InitiateMultipartUploadAsync(
        string fileName,
        string contentType,
        bool isVideo,
        CancellationToken cancellationToken = default);

    Task<string> GeneratePresignedPartUrlAsync(
        string key,
        string uploadId,
        int partNumber,
        bool isVideo,
        CancellationToken cancellationToken = default);

    Task<string> CompleteMultipartUploadAsync(
        string key,
        string uploadId,
        List<(int PartNumber, string ETag)> parts,
        bool isVideo,
        CancellationToken cancellationToken = default);

    Task<string> AbortMultipartUploadAsync(
        string key,
        string uploadId,
        bool isVideo,
        CancellationToken cancellationToken = default);

    Task<string> UploadThumbnailAsync(
        Stream fileStream,
        string fileName,
        string contentType,
        CancellationToken cancellationToken = default);

    // O'chirish
    Task DeleteVideoAsync(string key, CancellationToken cancellationToken = default);
    Task DeleteThumbnailAsync(string key, CancellationToken cancellationToken = default);
}