export interface IPost {
  postId: number;
  message: string;
  createDt: Date;
}

export function Post(props: {post: IPost}) {
  const post = props.post;

  return (
    <div className="post">

      <div className="post__header">
        &gt;&gt;{post.postId}
      </div>

      <div className="post__body">
        {post.message}
      </div>

    </div>
  );
}

export default Post;
