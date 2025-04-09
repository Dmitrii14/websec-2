import React from 'react';
import styles from './ErrorMessage.module.css';

const ErrorMessage = () => {
  return (
    <div>
      <p className={styles.p}>Комната полная</p>
    </div>
  );
};

export default ErrorMessage;