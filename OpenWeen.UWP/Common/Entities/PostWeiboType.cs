using System.Collections.Generic;
using Windows.ApplicationModel.DataTransfer.ShareTarget;
using Windows.Storage;

namespace OpenWeen.UWP.Common.Entities
{
    internal enum PostWeiboType
    {
        NewPost,
        RePost,
        Comment,
    }

    internal interface IPostWeibo
    {
        PostWeiboType Type { get; }
    }

    internal interface IPostWeiboData : IPostWeibo
    {
        string Data { get; set; }
    }

    internal class PostWeibo : IPostWeibo
    {
        public PostWeiboType Type => PostWeiboType.NewPost;
    }

    internal class SharedPostWeibo : IPostWeiboData
    {
        public PostWeiboType Type => PostWeiboType.NewPost;
        public string Data { get; set; }
        public byte[] ImageData { get; internal set; }
        public ShareOperation Operation { get; internal set; }
        public IReadOnlyList<IStorageItem> ImageFiles { get; internal set; }
    }

    internal class RepostData : IPostWeiboData
    {
        private RepostData()
        {
        }

        public RepostData(long id, string data)
        {
            ID = id;
            Data = data;
        }

        public PostWeiboType Type => PostWeiboType.RePost;
        public long ID { get; set; }
        public string Data { get; set; }
    }

    internal class CommentData : IPostWeibo
    {
        private CommentData()
        {
        }

        public CommentData(long id)
        {
            ID = id;
        }

        public PostWeiboType Type => PostWeiboType.Comment;
        public long ID { get; set; }
    }

    internal class ReplyCommentData : IPostWeiboData
    {
        private ReplyCommentData()
        {
        }

        public ReplyCommentData(long id, long cid, string data)
        {
            ID = id;
            CID = cid;
            Data = data;
        }

        public PostWeiboType Type => PostWeiboType.Comment;
        public long ID { get; set; }
        public long CID { get; set; }
        public string Data { get; set; }
    }
}