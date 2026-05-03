using MediatR;
using YouTubeClone.Application.Features.Comments.DTOs;
using YouTubeClone.Shared;

namespace YouTubeClone.Application.Features.Comments.Queries.GetVideoComments;

public record GetVideoCommentsQuery : IRequest<PagedResult<CommentDto>>
{
    public Guid VideoId { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}