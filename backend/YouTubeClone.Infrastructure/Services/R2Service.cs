using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using YouTubeClone.Domain.Interfaces;

namespace YouTubeClone.Infrastructure.Services;

public class R2Service : IS3Service
{
    private readonly IAmazonS3 _s3Client;
    private readonly ILogger<R2Service> _logger;
    private readonly string _videoBucket;
    private readonly string _thumbnailBucket;
    private readonly string _endpoint;

    public R2Service(
        IAmazonS3 s3Client,
        IConfiguration configuration,
        ILogger<R2Service> logger)
    {
        _s3Client = s3Client;
        _logger = logger;
        _videoBucket = configuration["CloudflareR2:VideoBucketName"]!;
        _thumbnailBucket = configuration["CloudflareR2:ThumbnailBucketName"]!;
        _endpoint = configuration["CloudflareR2:Endpoint"]!;
    }

    private string GetBucket(bool isVideo) =>
        isVideo ? _videoBucket : _thumbnailBucket;

    private string GenerateKey(string fileName, bool isVideo)
    {
        var folder = isVideo ? "videos" : "thumbnails";
        var ext = Path.GetExtension(fileName);
        return $"{folder}/{Guid.NewGuid()}{ext}";
    }

    // Presigned URL — kichik fayllar uchun (100MB gacha)
    public async Task<string> GeneratePresignedUploadUrlAsync(
        string fileName,
        string contentType,
        bool isVideo,
        CancellationToken cancellationToken = default)
    {
        var key = GenerateKey(fileName, isVideo);
        var bucket = GetBucket(isVideo);

        _logger.LogInformation("Generating presigned URL for: {Key}", key);

        var request = new GetPreSignedUrlRequest
        {
            BucketName = bucket,
            Key = key,
            Verb = HttpVerb.PUT,
            Expires = DateTime.UtcNow.AddMinutes(15),
            ContentType = contentType
        };

        var url = await _s3Client.GetPreSignedURLAsync(request);

        _logger.LogInformation("Presigned URL generated: {Key}", key);

        // Key ni ham qaytaramiz — DB ga saqlanadi
        return $"{url}|{key}";
    }

    // Multipart — katta fayllar uchun (100MB+)
    public async Task<string> InitiateMultipartUploadAsync(
        string fileName,
        string contentType,
        bool isVideo,
        CancellationToken cancellationToken = default)
    {
        var key = GenerateKey(fileName, isVideo);
        var bucket = GetBucket(isVideo);

        _logger.LogInformation("Initiating multipart upload: {Key}", key);

        var request = new InitiateMultipartUploadRequest
        {
            BucketName = bucket,
            Key = key,
            ContentType = contentType
        };

        var response = await _s3Client.InitiateMultipartUploadAsync(request, cancellationToken);

        _logger.LogInformation("Multipart upload initiated: {UploadId}", response.UploadId);

        // UploadId va Key ni qaytaramiz
        return $"{response.UploadId}|{key}";
    }

    public async Task<string> GeneratePresignedPartUrlAsync(
        string key,
        string uploadId,
        int partNumber,
        bool isVideo,
        CancellationToken cancellationToken = default)
    {
        var bucket = GetBucket(isVideo);

        _logger.LogInformation("Generating presigned part URL: {Key} Part: {PartNumber}", key, partNumber);

        var request = new GetPreSignedUrlRequest
        {
            BucketName = bucket,
            Key = key,
            Verb = HttpVerb.PUT,
            Expires = DateTime.UtcNow.AddMinutes(15),
            UploadId = uploadId,
            PartNumber = partNumber
        };

        return await _s3Client.GetPreSignedURLAsync(request);
    }

    public async Task<string> CompleteMultipartUploadAsync(
        string key,
        string uploadId,
        List<(int PartNumber, string ETag)> parts,
        bool isVideo,
        CancellationToken cancellationToken = default)
    {
        var bucket = GetBucket(isVideo);

        _logger.LogInformation("Completing multipart upload: {Key}", key);

        var request = new CompleteMultipartUploadRequest
        {
            BucketName = bucket,
            Key = key,
            UploadId = uploadId,
            PartETags = parts.Select(p => new PartETag(p.PartNumber, p.ETag)).ToList()
        };

        var response = await _s3Client.CompleteMultipartUploadAsync(request, cancellationToken);

        var url = $"{_endpoint}/{bucket}/{key}";

        _logger.LogInformation("Multipart upload completed: {Url}", url);

        return url;
    }

    public async Task<string> AbortMultipartUploadAsync(
        string key,
        string uploadId,
        bool isVideo,
        CancellationToken cancellationToken = default)
    {
        var bucket = GetBucket(isVideo);

        _logger.LogInformation("Aborting multipart upload: {Key}", key);

        var request = new AbortMultipartUploadRequest
        {
            BucketName = bucket,
            Key = key,
            UploadId = uploadId
        };

        await _s3Client.AbortMultipartUploadAsync(request, cancellationToken);

        _logger.LogInformation("Multipart upload aborted: {Key}", key);

        return key;
    }

    public async Task<string> UploadThumbnailAsync(
        Stream fileStream,
        string fileName,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        var key = GenerateKey(fileName, isVideo: false);

        _logger.LogInformation("Uploading thumbnail: {Key}", key);

        var request = new PutObjectRequest
        {
            BucketName = _thumbnailBucket,
            Key = key,
            InputStream = fileStream,
            ContentType = contentType,
            DisablePayloadSigning = true
        };

        await _s3Client.PutObjectAsync(request, cancellationToken);

        var url = $"{_endpoint}/{_thumbnailBucket}/{key}";

        _logger.LogInformation("Thumbnail uploaded: {Url}", url);

        return url;
    }

    public async Task DeleteVideoAsync(
        string key,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting video: {Key}", key);

        await _s3Client.DeleteObjectAsync(_videoBucket, key, cancellationToken);

        _logger.LogInformation("Video deleted: {Key}", key);
    }

    public async Task DeleteThumbnailAsync(
        string key,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting thumbnail: {Key}", key);

        await _s3Client.DeleteObjectAsync(_thumbnailBucket, key, cancellationToken);

        _logger.LogInformation("Thumbnail deleted: {Key}", key);
    }
}