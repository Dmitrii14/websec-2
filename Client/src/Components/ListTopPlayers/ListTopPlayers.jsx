import React from "react";
import styles from "./ListTopPlayers.module.css";

const ListTopPlayers = ({ players }) => {
  return (
    <div className={styles.container}>
      <h3 className={styles.title}>Топ игроков</h3>
      <ul className={styles.ul}>
        {players.map((player, index) => (
          <li key={index} className={styles.player}>
            <span>{player.username}</span>
            <span>{player.rating}</span>
          </li>
        ))}
      </ul>
    </div>
  );
};

export default ListTopPlayers;