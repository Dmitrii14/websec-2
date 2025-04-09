import React from 'react';
import styles from './PlayerInformation.module.css';

const PlayerInformation = ({ playerName, starCount, onLeave }) => {
  return (
    <div className={styles.container}>
      <div className={styles.infoContainer}>
        <div>
          <span>Игрок:</span>
          <span>{playerName}</span>
        </div>
        <div>
          <span>Рейтинг:</span>
          <span>{starCount}</span>
        </div>
      </div>
      <button className={styles.button} onClick={onLeave}>Выйти</button>
    </div>
  );
};

export default PlayerInformation;