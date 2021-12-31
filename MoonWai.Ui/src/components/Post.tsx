export interface IPost {
  postId: number;
  message: string;
  respondentPostIds: Array<number>;
  createDt: Date;
}

export function Post(props: {post: IPost}) {
  const post = props.post;

  return (
    <div className="post">

      <div className="post__header">
        {new Date(post.createDt).toLocaleString('ru-RU')}&nbsp;&nbsp;
        <span className="post__id--right">#{post.postId}</span>
      </div>

      <div className="post__body">
        {post.message}

        {post.respondentPostIds.length > 0 && (
          post.respondentPostIds.map(i => <span key={i} className="post__id">#{i}&nbsp;&nbsp;</span>)
        )}
      </div>

    </div>
  );
}

export default Post;
