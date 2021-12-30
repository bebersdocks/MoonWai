import axios from 'axios';
import { useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';

import { useAppDispatch, useAppSelector } from '../app/hooks';
import { selectUser, logoutAsync } from '../slices/authSlice';
import { getBoardsAsync } from '../slices/catalogSlice';

export function Navigation() {
  const dispatch = useAppDispatch();
  const { t, i18n } = useTranslation();

  const user = useAppSelector(selectUser);

  let navAuth;

  if (user)
    navAuth = 
      <div className="nav__auth">
        <Link className="nav-link" to="/login">Logout</Link>
      </div>
  else
    navAuth = 
      <div className="nav__auth">
        <Link className="nav-link" to="/login">Login</Link>
        <Link className="nav-link" to="/register">Register</Link>
      </div>

  return (
    <div className="nav">
      {navAuth}
    </div>
  );
}

export default Navigation;
