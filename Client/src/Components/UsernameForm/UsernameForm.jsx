import React, { useState } from "react";
import styles from "./UsernameForm.module.css";

const UsernameForm = ({ onSubmit }) => {
  const [name, setName] = useState("");

  const handleSubmit = (e) => {
    e.preventDefault();
    if (name.trim()) {
      onSubmit(name);
    }
  };

  return (
    <form onSubmit={handleSubmit} className={styles.container}>
      <input
        type="text"
        placeholder="Введите имя"
        value={name}
        onChange={(e) => setName(e.target.value)}
        className={styles.input}
      />
      <button type="submit" className={styles.button}>Играть</button>
    </form>
  );
};

export default UsernameForm;