import React from 'react';
import ReactDOM from 'react-dom';
import { Provider } from 'react-redux';

import { store } from './app/store';
import { getBoardsAsync } from './slices/catalogSlice';
import App from './components/App';
import './utils/i18n';
import * as serviceWorker from './serviceWorker';

store.dispatch(getBoardsAsync());

ReactDOM.render(
  <React.StrictMode>
    <Provider store={store}>
      <App />
    </Provider>
  </React.StrictMode>,
  document.getElementById('root')
);

serviceWorker.unregister();
