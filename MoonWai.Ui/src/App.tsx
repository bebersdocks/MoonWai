import React from 'react';
import { BrowserRouter, Routes, Route } from "react-router-dom";
import { Login } from './components/Login';
import { Register } from './components/Register';
import './resources/scss/main.scss';

let router =
  <BrowserRouter>
  <Routes>
    <Route index element={<Login />} />
    <Route path="login" element={<Login />} />
    <Route path="register" element={<Register />} />
  </Routes>
  </BrowserRouter>

function App() {
  return (
    <div className='app'>
      {router}
    </div>
  );
};

export default App;
