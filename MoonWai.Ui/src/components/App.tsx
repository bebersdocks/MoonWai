import { BrowserRouter, Routes, Route } from 'react-router-dom';

import { Board } from '../pages/Board'
import { Login } from '../pages/Login';
import { Register } from '../pages/Register';;

import Navigation from './Navigation';

import '../resources/scss/main.scss';

let routes =
  <Routes>
    <Route index element={<Login />} />
    <Route path="login" element={<Login />} />
    <Route path="register" element={<Register />} />
  </Routes>

function App() {
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
