import { useState } from 'react';
import { useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';
import { useAppSelector, useAppDispatch } from '../app/hooks';
import { registerAsync } from '../slices/authSlice';
import { selectMessage } from '../slices/messageSlice';

export function Register() {
  const dispatch = useAppDispatch();

  const { t, i18n } = useTranslation();

  const message = useAppSelector(selectMessage);
  
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [passwordAgain, setPasswordAgain] = useState('');
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
    else if (password != passwordAgain) {
      setValidationMessage(t('Errors.PasswordsDontMatch'));
      setValid(false);
    }
    else {
      setValidationMessage('');
      setValid(true);
    }
  };

  function onChangePasswordAgain(e: any) {
    const passwordAgain = e.target.value;
    setPasswordAgain(passwordAgain);

    if (password != passwordAgain) {
      setValidationMessage(t('Errors.PasswordsDontMatch'));
      setValid(false);
    }
    else {
      setValidationMessage('');
      setValid(true);
    }
  };

  function handleRegister(e: any) {
    e.preventDefault();
    setLoading(true);
    dispatch(registerAsync(username, password));
  };

  return (
    <div className="form" >

      <div className="form__header">
        <span>{t('Ui.WelcomeAnon')}</span>
      </div>

      <div className="form--register">
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
        
        <div className="form__input">
          <input type="password" name="passwordAgain" placeholder={t('Ui.RepeatPassword')} value={passwordAgain} onChange={onChangePasswordAgain} />
        </div>

        <button className="btn btn-primary btn-block" disabled={!valid || loading} onClick={handleRegister}>
          <span>{t('Ui.Register')}</span>
        </button>

      </div>

      <Link className="nav-link" to="/login">{t('Ui.AlreadyHaveAccount')}</Link>
    </div>
  );
}
