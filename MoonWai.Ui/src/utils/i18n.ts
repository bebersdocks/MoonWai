import i18n from 'i18next';
import { initReactI18next } from 'react-i18next';

import en from '../resources/translations/en.json'
import ru from '../resources/translations/ru.json'

const resources = {
  en: {
    translation: en
  },
  ru: {
    translation: ru
  }
};

i18n
  .use(initReactI18next)
  .init({
    resources: resources,
    lng: 'ru',
    interpolation: {
      escapeValue: false
    },
  });

export default i18n;
