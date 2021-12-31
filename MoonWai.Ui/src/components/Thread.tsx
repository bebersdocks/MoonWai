import { IPost, Post } from "./Post";

export interface IThread {
  threadId: number;
  parentId: number;
  title: string;
  posts: Array<IPost>;
  postsCount: number;
  createDt: Date;
}

export function Thread(props: {thread: IThread}) {
  const thread = props.thread;
  const threadPost = thread.posts[0];
  
  return (
    <div className="thread">

      <div className="thread__header">
        {new Date(thread.createDt).toLocaleString('ru-RU')}&nbsp;&nbsp;
        <span className="post__id">#{thread.threadId}</span>
      </div>

      <div className="thread__body">
        {threadPost && threadPost.message}
      </div>

      {thread.posts
        .slice(1)
        .map(p => <Post key={p.postId} post={p} />)}
    </div>
  );   
}

export default Thread;
