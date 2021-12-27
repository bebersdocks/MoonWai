import { useState } from 'react';
import { useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';
import { useAppSelector, useAppDispatch } from '../app/hooks';
import { loginAsync, selectUser } from '../slices/authSlice';
import { selectMessage } from '../slices/messageSlice';

export function Login() {
  const dispatch = useAppDispatch();

  const { t, i18n } = useTranslation();

  const message = useAppSelector(selectMessage);
  
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [trusted, setTrusted] = useState(false);
  const [loading, setLoading] = useState(false);
  const [valid, setValid] = useState(false);
  const [validationMessage, setValidationMessage] = useState('');
  
  function onChangeUsername(e: any) {
    const username = e.target.value;
    setUsername(username);

    if (!username) {
      setValidationMessage(t('Errors.UsernameCantBeEmpty'));
      setValid(false);
    }
    else {
      setValidationMessage('');
      setValid(true);
    }
  };

  function onChangePassword(e: any) {
    const password = e.target.value;
    setPassword(password);

    let minPasswordLength = 8;

    if (!password) {
      setValidationMessage(t('Errors.PasswordCantBeEmpty'));
      setValid(false);
    }
    else if (password.length < minPasswordLength) {
      let errorMsg = t('Errors.PasswordlengthCantBeLessThan')
      errorMsg = errorMsg.replace('{0}', minPasswordLength.toString());
      setValidationMessage(errorMsg);
      setValid(false);
    }
    else {
      setValidationMessage('');
      setValid(true);
    }
  };

  function onChangeTrusted(e: any) {
    const trusted = e.target.value;
    setTrusted(trusted);
  }

  function handleLogin(e: any) {
    e.preventDefault();
    setLoading(true);
    setTimeout(() => setLoading(false), 5000);
    dispatch(loginAsync(username, password, trusted));
  };

  return (
    <div className="form" >

      <div className="form__header">
        <span>{t('Ui.WelcomeBack')}</span>
      </div>

      <div className="form--login">
        {(validationMessage || message) && (
          <div className="form__message">
              {validationMessage || message}
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
            <input type="checkbox" id="trusted" onChange={onChangeTrusted} defaultChecked={trusted} />
            <label htmlFor="trusted">{t('Ui.Trusted')}</label>
          </div>

          <button className="btn btn-primary btn-block" disabled={!valid || loading} onClick={handleLogin}>
            <span>{t('Ui.Login')}</span>
          </button>
        </div>
      </div>

      <Link className="nav-link" to="/register">{t('Ui.DontHaveAccount')}</Link>
    </div>
  );
}
