import axios from 'axios';
import { useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';

import { useAppDispatch, useAppSelector } from '../app/hooks';
import { IThread, Thread } from '../components/Thread';

export function Board(props: {path: string}) {
  const dispatch = useAppDispatch();
  
  const { t, i18n } = useTranslation();

  const [threads, setThreads] = useState(new Array<IThread>());
  const [page, setPage] = useState(1);
  const [totalPages, setTotalPages] = useState(0);

  function getThreadsAsync() {  
    axios
      .get('api/boards/' + props.path)
      .then(response => { 
        setThreads(response.data.items);
        setTotalPages(response.data.pages); 
      });
  }

  useEffect(getThreadsAsync, []);

  return (
    <div className="board">
      {threads.map(t => <Thread key={t.threadId} thread={t} />)}
    </div>
  );
}

export default Board;
