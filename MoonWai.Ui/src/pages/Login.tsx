import axios from 'axios';
import { useState } from 'react';
import { useTranslation } from 'react-i18next';
import { Link, useNavigate } from 'react-router-dom';

import { useAppDispatch, useAppSelector } from '../app/hooks';
import { authSuccess, selectUser } from '../slices/authSlice';

export function Login() {
  const dispatch = useAppDispatch();
  const navigate = useNavigate();

  const { t, i18n } = useTranslation();

  const user = useAppSelector(selectUser);

  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [trusted, setTrusted] = useState(false);
  const [loading, setLoading] = useState(false);
  const [message, setMessage] = useState('');
  const [valid, setValid] = useState(false);
  
  async function loginAsync() {
    setLoading(true);
  
    const data = {
      username: username,
      password: password, 
      trusted: trusted,
    };

    await axios
      .post('api/auth/login', data)
      .then(response => {
        localStorage.setItem('user', JSON.stringify(response.data));
        dispatch(authSuccess(response.data));
        if (user)
          navigate(user.defaultBoardPath);
      })
      .catch((err) => {
        if (err.response && err.response.data && err.response.data.errorIdStr)
          setMessage(t('Errors.' + err.response.data.errorIdStr));
        else
          setMessage(err.message || err.toString());
      });
  
    setLoading(false);
  };

  const isValid = (username: string, password: string) => !(!username || !password);

  function onChangeUsername(e: React.ChangeEvent<HTMLInputElement>) {
    const username = e.target.value;
    setUsername(username);
    setValid(isValid(username, password));

    if (!username)
      setMessage(t('Errors.UsernameCantBeEmpty'));
    else
      setMessage('');
  };

  function onChangePassword(e: React.ChangeEvent<HTMLInputElement>) {
    const password = e.target.value;
    setPassword(password);
    setValid(isValid(username, password));

    if (!password)
      setMessage(t('Errors.PasswordCantBeEmpty'));
    else
      setMessage('');
  };

  return (
    <div className="form" >

      <div className="form__header">
        <span>{t('Ui.WelcomeBack')}</span>
      </div>

      <div className="form--login">
        {message && (
          <div className="form__message">
            {message}
          </div>
        )}

        <div className="form__input">
          <input type="text" name="username" placeholder={t('Ui.Username')} value={username} onChange={onChangeUsername} />
        </div>

        <div className="form__input">
          <input type="password" name="password" placeholder={t('Ui.Password')} value={password} onChange={onChangePassword} />
        </div>
        
        <div className="form__input--sided">
          <div>
            <input type="checkbox" id="trusted" onChange={e => setTrusted(e.target.checked)} defaultChecked={trusted} />
            <label htmlFor="trusted">{t('Ui.Trusted')}</label>
          </div>

          <button className="btn btn-primary btn-block" disabled={!valid || loading} onClick={loginAsync}>
            <span>{t('Ui.Login')}</span>
          </button>
        </div>
      </div>

      <Link className="nav-link" to="/register">{t('Ui.DontHaveAccount')}</Link>
      
    </div>
  );
}
