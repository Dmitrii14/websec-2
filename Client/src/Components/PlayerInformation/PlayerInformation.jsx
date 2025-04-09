import React from 'react';
import styles from './PlayerInformation.module.css';

const PlayerInformation = ({ playerName, starCount, onLeave }) => {
  return (
    <div className={styles.container}>
      <span>Игрок:</span>
      <span>{playerName}</span>
      <span>Рейтинг:</span>
      <span>{starCount}</span>
      <button className={styles.button} onClick={onLeave}>Выйти</button>
    </div>
  );
};

export default PlayerInformation;