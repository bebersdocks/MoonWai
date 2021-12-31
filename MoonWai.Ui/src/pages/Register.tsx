import axios from 'axios';
import { useState } from 'react';
import { useTranslation } from 'react-i18next';
import { Link, useNavigate } from 'react-router-dom';

import { useAppDispatch, useAppSelector } from '../app/hooks';
import { authSuccess, selectUser } from '../slices/authSlice';

export function Register() {
  const dispatch = useAppDispatch();
  const navigate = useNavigate();

  const { t, i18n } = useTranslation();

  const user = useAppSelector(selectUser);

  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [passwordAgain, setPasswordAgain] = useState('');
  const [loading, setLoading] = useState(false);
  const [message, setMessage] = useState('');
  const [valid, setValid] = useState(false);

  async function registerAsync() {
    setLoading(true);
  
    const data = {
      username: username,
      password: password,
    };

    await axios
      .post('api/auth/register', data)
      .then(response => {
        localStorage.setItem('user', JSON.stringify(response.data));
        dispatch(authSuccess(response.data));
        if (user)
          navigate('/' + user.defaultBoardPath);
      })
      .catch((err) => {
        if (err.response && err.response.data && err.response.data.errorIdStr)
          setMessage(t('Errors.' + err.response.data.errorIdStr));
        else
          setMessage(err.message || err.toString());
      });
  
    setLoading(false);
  };

  const minPasswordLength = 8;
  const isValid = (username: string, password: string, passwordAgain: string) =>
    !(!username || !password || password.length < minPasswordLength || !passwordAgain || password != passwordAgain);

  function onChangeUsername(e: React.ChangeEvent<HTMLInputElement>) {
    const username = e.target.value;
    setUsername(username);
    setValid(isValid(username, password, passwordAgain));

    if (!username)
      setMessage(t('Errors.UsernameCantBeEmpty'));
    else
      setMessage('');
  };

  function onChangePassword(e: React.ChangeEvent<HTMLInputElement>) {
    const password = e.target.value;
    setPassword(password);
    setValid(isValid(username, password, passwordAgain));

    if (!password)
      setMessage(t('Errors.PasswordCantBeEmpty'));
    else if (password.length < minPasswordLength)
      setMessage(t('Errors.PasswordlengthCantBeLessThan').replace('{0}', minPasswordLength.toString()));
    else if (password != passwordAgain)
      setMessage(t('Errors.PasswordsDontMatch'));
    else
      setMessage('');
  };

  function onChangePasswordAgain(e: React.ChangeEvent<HTMLInputElement>) {
    const passwordAgain = e.target.value;
    setPasswordAgain(passwordAgain);
    setValid(isValid(username, password, passwordAgain));

    if (password != passwordAgain)
      setMessage(t('Errors.PasswordsDontMatch'));
    else
      setMessage('');
  };

  return (
    <div className="form" >

      <div className="form__header">
        <span>{t('Ui.WelcomeAnon')}</span>
      </div>

      <div className="form--register">
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
        
        <div className="form__input">
          <input type="password" name="passwordAgain" placeholder={t('Ui.RepeatPassword')} value={passwordAgain} onChange={onChangePasswordAgain} />
        </div>

        <button className="btn btn-primary btn-block" disabled={!valid || loading} onClick={registerAsync}>
          <span>{t('Ui.Register')}</span>
        </button>

      </div>

      <Link className="nav-link" to="/login">{t('Ui.AlreadyHaveAccount')}</Link>
      
    </div>
  );
}
