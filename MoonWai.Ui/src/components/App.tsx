import { BrowserRouter, Routes, Route } from 'react-router-dom';

import { useAppSelector } from '../app/hooks';
import { selectUser } from '../slices/authSlice';

import Navigation from './Navigation';

import { Board } from '../pages/Board'
import { Login } from '../pages/Login';
import { Register } from '../pages/Register';

import '../resources/scss/main.scss';

function App() {
  const user = useAppSelector(selectUser);

  const routes =
    <Routes>
      <Route index element={<Board boardPath={user?.defaultBoardPath ?? 'b'} />} />
      <Route path="login" element={<Login />} />
      <Route path="register" element={<Register />} />
    </Routes>

  return (
    <BrowserRouter>
      <div className='app'>
        <Navigation />
        {routes}
      </div>
    </BrowserRouter>
  );
};

export default App;
